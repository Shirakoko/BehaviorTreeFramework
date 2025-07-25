using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class GhostController : MonoBehaviour
{
    // 当前行为状态
    public enum GhostBehavior
    {
        Patrol,
        Chase,
        Flee
    }

    [Header("行为设置")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float fleeSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float safeDistance = 7f;
    [SerializeField] private float waitTimeAtPatrolPoint = 2f;

    [Header("精灵设置")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite fleeSprite;

    [Header("巡逻设置")]
    [SerializeField] private List<Transform> patrolPointTransforms = new List<Transform>();

    // 当前行为状态
    private GhostBehavior currentBehavior = GhostBehavior.Patrol;
    public GhostBehavior CurrentBehavior => currentBehavior;

    private CircleCollider2D circleCollider;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BTRunner behaviorTree;
    private List<Vector2> patrolPoints;
    private Vector2 currentDirection; // 当前移动方向
    private float currentVelocity; // 当前移动速度
    private bool isBouncing = false; // 是否正在反弹
    private float bounceTimer = 0f; // 反弹计时器
    private const float BOUNCE_DURATION = 0.2f; // 反弹持续时间
    private Transform initialTranform; // 初始变换
    
    private Vector3 initialPosition;

    public Vector2 CurrentDirection
    {
        get => currentDirection;
        set => currentDirection = value.normalized; // 确保方向向量是单位向量
    }

    public float CurrentVelocity
    {
        get => currentVelocity;
        set => currentVelocity = value;
    }

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialTranform = GetComponent<Transform>();

        // 保存初始Transform信息
        initialPosition = transform.position;

        // 设置碰撞器
        circleCollider.isTrigger = false; // 改为非触发器，用于物理碰撞
        circleCollider.radius = 0.5f;

        // 设置刚体
        rb.gravityScale = 0f; // 无重力
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 冻结旋转
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 连续碰撞检测
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 插值移动

        // 转换巡逻点Transform为Vector2列表
        patrolPoints = new List<Vector2>();
        foreach (Transform point in patrolPointTransforms)
        {
            if (point != null)
            {
                patrolPoints.Add(point.position);
            }
        }

        // 设置初始精灵
        SetSprite(normalSprite);

        // 构建行为树
        BuildBehaviorTree();

        // 订阅游戏初始化事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameInitialized.AddListener(ResetToInitialState);
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameInitialized.RemoveListener(ResetToInitialState);
        }
    }

    private void ResetToInitialState()
    {
        transform.position = initialPosition;
        currentDirection = Vector2.right;
        currentVelocity = 0f;
        isBouncing = false;
        bounceTimer = 0f;
        rb.velocity = Vector2.zero;
        SetSprite(normalSprite);
        currentBehavior = GhostBehavior.Patrol;
    }

    private void BuildBehaviorTree()
    {
        // 创建行为树构建器
        BTBuilder builder = new BTBuilder();

        // 构建行为树
        behaviorTree = builder
            .Selector() // 选择器
                // 如果玩家处于能量豆状态且在安全距离内，优先逃跑
                .Sequence()
                    .Condition(GhostSimulator.IsPowerModeAndWithinSafeDistance(transform, safeDistance))
                    .Action(GhostSimulator.GhostFlee(transform, fleeSpeed, safeDistance))
                .End()
                // 如果玩家在检测范围内且非能量豆状态，追击玩家
                .Sequence()
                    .Condition(GhostSimulator.IsNotPowerModeAndWithinRange(transform, detectionRange))
                    .Action(GhostSimulator.GhostChase(transform, chaseSpeed, detectionRange))
                .End()
                // 默认行为：巡逻
                .Action(GhostSimulator.GhostPatrol(transform, patrolPoints, patrolSpeed, waitTimeAtPatrolPoint))
            .End()
            .Build();
    }

    private void Update()
    {
        if (isBouncing)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer >= BOUNCE_DURATION)
            {
                isBouncing = false;
                bounceTimer = 0f;
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            behaviorTree.ExecuteBT();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 计算反弹方向
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bounceDirection = Vector2.Reflect(currentDirection, normal);
            
            // 应用反弹力
            rb.velocity = bounceDirection * currentVelocity;
            
            // 设置反弹状态
            isBouncing = true;
            
            // 更新当前方向
            currentDirection = bounceDirection.normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.IsPowerModeActive())
            {
                // 玩家处于能量豆状态，怪物被吃掉
                Debug.Log("PacmanPlayer Ate a ghost!");
                GameManager.Instance.AddScore(200);
                // 新生成一个怪物
                Respawn();
            }
            else
            {
                // 玩家被怪物碰到，失去生命
                Debug.Log("PacmanPlayer Lose Life!");
                GameManager.Instance.LoseLife();
                // 玩家重置回到原点
                collision.gameObject.GetComponent<PacmanPlayer>().ResetTransform();
            }
        }
    }

    private void Respawn()
    {
        ResetToInitialState();
    }

    // 切换行为状态
    public void SwitchBehavior(GhostBehavior newBehavior)
    {
    if (currentBehavior != newBehavior)
        {
            currentBehavior = newBehavior;
            // 根据新状态设置精灵
            switch (newBehavior)
            {
                case GhostBehavior.Flee:
                    SetSprite(fleeSprite);
                    break;
                case GhostBehavior.Patrol:
                case GhostBehavior.Chase:
                    SetSprite(normalSprite);
                    break;
            }
        }
    }

    // 设置Sprite
    private void SetSprite(Sprite sprite)
    {
        if (sprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
} 
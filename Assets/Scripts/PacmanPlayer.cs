using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PacmanPlayer : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("组件引用")]
    [SerializeField] private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;    // 普通状态精灵
    [SerializeField] private Sprite powerfulSprite;  // 能量状态精灵

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Vector2 currentDirection = Vector2.right;
    private Vector2 nextDirection = Vector2.right;
    private bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        
        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.4f;
        }
        else
        {
            Debug.LogError("PacmanPlayer: CircleCollider2D component is missing!");
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else
        {
            Debug.LogError("PacmanPlayer: Rigidbody2D component is missing!");
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // 订阅能量豆事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onPowerDotActivated.AddListener(OnPowerModeActivated);
            GameManager.Instance.onPowerDotDeactivated.AddListener(OnPowerModeDeactivated);
        }
        else
        {
            Debug.LogError("PacmanPlayer: GameManager.Instance为空，无法订阅事件！");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onPowerDotActivated.RemoveListener(OnPowerModeActivated);
            GameManager.Instance.onPowerDotDeactivated.RemoveListener(OnPowerModeDeactivated);
        }
    }

    private void OnPowerModeActivated()
    {
        if (spriteRenderer != null && powerfulSprite != null)
        {
            spriteRenderer.sprite = powerfulSprite;
        }
    }

    private void OnPowerModeDeactivated()
    {
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    private void Update()
    {
        // 获取输入
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 处理输入方向
        if (horizontal != 0 || vertical != 0)
        {
            Vector2 inputDirection = new Vector2(horizontal, vertical).normalized;
            nextDirection = inputDirection;
        }

        // 根据移动方向翻转Sprite
        if (spriteRenderer != null)
        {
            if (currentDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (currentDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        // 尝试转向
        if (CanMoveInDirection(nextDirection))
        {
            currentDirection = nextDirection;
        }
        // 如果无法转向，继续当前方向
        else if (!CanMoveInDirection(currentDirection))
        {
            rb.velocity = Vector2.zero; // 碰到墙时停止移动
            return;
        }

        // 移动
        Vector2 movement = currentDirection * moveSpeed;
        rb.velocity = movement;

        // 旋转角色面向移动方向
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        // 使用圆形碰撞器进行检测
        Vector2 origin = (Vector2)transform.position + direction * circleCollider.radius;
        float distance = 0.1f; // 检测距离

        Debug.DrawRay(origin, direction * distance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            LayerMask.GetMask("Wall")
        );

        return hit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Ghost"))
        {
            if (GameManager.Instance.IsPowerModeActive())
            {
                Debug.Log("PacmanPlayer: Ate a ghost!");
                Destroy(other.gameObject);
                GameManager.Instance.AddScore(200);
            }
            else
            {
                Debug.Log("PacmanPlayer: Got eaten by ghost!");
                GameManager.Instance.LoseLife();
                Respawn();
            }
        }
    }

    private void Respawn()
    {
        // 重置位置到出生点
        transform.position = Vector3.zero;
        currentDirection = Vector2.right;
        nextDirection = Vector2.right;
        rb.velocity = Vector2.zero;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
        }
    }
} 
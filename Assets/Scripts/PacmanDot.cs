using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PacmanDot : MonoBehaviour
{
    public enum DotType
    {
        Normal,     // 普通豆子
        Power       // 能量豆
    }

    [SerializeField] private DotType dotType = DotType.Normal;
    [SerializeField] private int scoreValue = 10;      // 普通豆子分数
    [SerializeField] private int powerScoreValue = 50; // 能量豆分数

    private CircleCollider2D circleCollider;

    public DotType Type => dotType;
    public int ScoreValue => dotType == DotType.Normal ? scoreValue : powerScoreValue;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError($"PacmanDot: CircleCollider2D component is missing on {gameObject.name}!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"PacmanDot: Trigger entered with {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");

        if (other.CompareTag("Player"))
        {
            Debug.Log($"{dotType} 豆子被吃掉");
            
            // 获取豆子在地图上的位置
            Vector2 position = transform.position;
            Vector2 mapPos = position - new Vector2(PacmanMapGenerator.Instance.mapOriginOffset.x, PacmanMapGenerator.Instance.mapOriginOffset.y);
            int x = Mathf.RoundToInt(mapPos.x);
            int y = Mathf.RoundToInt(mapPos.y);

            // 通知游戏管理器增加分数
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(ScoreValue);
                Debug.Log($"Score added: {ScoreValue}");
                
                // 如果是能量豆，触发特殊效果
                if (dotType == DotType.Power)
                {
                    GameManager.Instance.OnPowerDotEaten();
                }
            }
            else
            {
                Debug.LogError("GameManager.Instance is null!");
            }

            // 将当前位置设置为空
            if (PacmanMapGenerator.Instance != null)
            {
                PacmanMapGenerator.Instance.SetMapElementAt(x, y, PacmanMapGenerator.MapElement.Empty);
                // 随机生成新的豆子
                PacmanMapGenerator.Instance.SpawnRandomDot();
            }
            else
            {
                Debug.LogError("PacmanMapGenerator.Instance is null!");
            }

            // 销毁豆子
            Destroy(gameObject);
        }
    }
} 
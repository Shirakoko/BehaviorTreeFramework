using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("游戏设置")]
    [SerializeField] private int initialLives = 3;
    [SerializeField] private float powerDotDuration = 5f;

    public int InitialLives => initialLives;

    [Header("事件")]
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<int> onLivesChanged;
    public UnityEvent onPowerDotActivated;
    public UnityEvent onPowerDotDeactivated;

    private int currentScore = 0;
    private int currentLives;
    private bool isPowerModeActive = false;
    private float powerModeTimer = 0f;
    private Transform playerTransform;

    public Transform PlayerTransform => playerTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 获取玩家引用
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    private void Start()
    {
        currentLives = initialLives;
        onLivesChanged?.Invoke(currentLives);
        onScoreChanged?.Invoke(currentScore);
        
        // 订阅UI更新事件
        onScoreChanged.AddListener(score => UIManager.Instance.UpdateScore(score));
        onLivesChanged.AddListener(lives => UIManager.Instance.UpdateLives(lives));
    }

    private void Update()
    {
        if (isPowerModeActive)
        {
            powerModeTimer -= Time.deltaTime;
            if (powerModeTimer <= 0f)
            {
                DeactivatePowerMode();
            }
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        onScoreChanged?.Invoke(currentScore);
    }

    public void LoseLife()
    {
        currentLives--;
        onLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void OnPowerDotEaten()
    {
        ActivatePowerMode();
    }

    private void ActivatePowerMode()
    {
        Debug.Log("【激活】Power dot effect activated");
        isPowerModeActive = true;
        powerModeTimer = powerDotDuration;
        onPowerDotActivated?.Invoke();
    }

    private void DeactivatePowerMode()
    {
        Debug.Log("【失活】Power dot effect deactivated");
        isPowerModeActive = false;
        onPowerDotDeactivated?.Invoke();
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0f; // 暂停游戏
        UIManager.Instance.ShowGameOverPanel(currentScore);
    }

    public bool IsPowerModeActive() => isPowerModeActive;
} 
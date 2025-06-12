using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("面板")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject hudPanel;

    [Header("开始面板按钮")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitGameButton;

    [Header("游戏结束面板按钮")]
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button gameOverQuitButton;

    [Header("HUD元素")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("游戏结束面板")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterButtonEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void RegisterButtonEvents()
    {
        // 注册开始面板按钮事件
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGameClicked);
        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(OnQuitGameClicked);

        // 注册游戏结束面板按钮事件
        if (restartGameButton != null)
            restartGameButton.onClick.AddListener(OnRestartGameClicked);
        if (gameOverQuitButton != null)
            gameOverQuitButton.onClick.AddListener(OnQuitGameClicked);
    }

    private void OnDestroy()
    {
        // 清理按钮事件，防止内存泄漏
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClicked);
        if (quitGameButton != null)
            quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        if (restartGameButton != null)
            restartGameButton.onClick.RemoveListener(OnRestartGameClicked);
        if (gameOverQuitButton != null)
            gameOverQuitButton.onClick.RemoveListener(OnQuitGameClicked);
    }

    private void Start()
    {
        // 检查面板引用
        if (startPanel == null) Debug.LogError("开始面板未设置！");
        if (gameOverPanel == null) Debug.LogError("游戏结束面板未设置！");
        if (hudPanel == null) Debug.LogError("HUD面板未设置！");
        
        // 检查文本组件引用
        if (scoreText == null) Debug.LogError("得分文本未设置！");
        if (livesText == null) Debug.LogError("生命值文本未设置！");
        if (finalScoreText == null) Debug.LogError("最终得分文本未设置！");

        // 初始化显示
        scoreText.text = "Score: 0";
        livesText.text = $"Lives: {GameManager.Instance.InitialLives}";

        // 游戏开始时显示开始面板，暂停游戏
        ShowStartPanel();
        Time.timeScale = 0f;
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    public void ShowGameOverPanel(int finalScore)
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        hudPanel.SetActive(false);
        finalScoreText.text = $"Final Score: {finalScore}";
    }

    public void ShowHUD()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }

    // UI按钮事件处理函数
    public void OnStartGameClicked()
    {
        if (Time.timeScale == 0f)
        {
            ShowHUD();
            Time.timeScale = 1f;
        }
    }

    public void OnRestartGameClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitGameClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 
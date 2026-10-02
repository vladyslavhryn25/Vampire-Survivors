using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Health playerHealth;

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text timeText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;

    private const string BestScoreKey = "BestScore";

    private float survivedTime;

    public int Score { get; private set; }
    public int Kills { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (playerHealth == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) playerHealth = playerObject.GetComponent<Health>();
        }

        if (playerHealth == null)
            Debug.LogError("GameManager: Player Health is not assigned.", this);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.Died += GameOver;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.Died -= GameOver;
    }

    private void Start()
    {
        RefreshHud();
    }

    private void Update()
    {
        if (IsGameOver) return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            GameOver();
            return;
        }

        survivedTime += Time.deltaTime;
        SetText(timeText, FormatTime(survivedTime));
    }

    public void AddKill(int points)
    {
        if (IsGameOver) return;

        Kills++;
        Score += points;
        RefreshHud();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        bool newRecord = Score > best;

        if (newRecord)
        {
            best = Score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        string result = $"GAME OVER\n\nSCORE  {Score}\nKILLS  {Kills}\nTIME  {FormatTime(survivedTime)}\nBEST  {best}";
        if (newRecord) result += "\n\nNEW RECORD!";

        SetText(resultText, result);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void RefreshHud()
    {
        SetText(scoreText, $"SCORE: {Score}");
        SetText(killsText, $"KILLS: {Kills}");
        SetText(timeText, FormatTime(survivedTime));
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null) target.text = value;
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60:00}:{total % 60:00}";
    }
}

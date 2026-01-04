using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class GameManager_M2 : MonoBehaviour
{
    public static GameManager_M2 Instance;

    [Header("UI en juego")]
    public TextMeshProUGUI scoreText;

    // --- MECÁNICA ANTERIOR (se deja comentada por si se necesitara) ---
    // [Header("Panel Game Over")]
    // public GameObject gameOverPanel;
    // public TextMeshProUGUI gameOverScoreText;
    // ---------------------------------------------------------------

    [Header("Victoria")]
    public int scoreToWin = 20; // Ganar por tiempo

    private int score = 0;
    private bool isFinished = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();

        // --- MECÁNICA ANTERIOR ---
        // if (gameOverPanel != null) gameOverPanel.SetActive(false);
        // -------------------------
    }

    public void AddScore(int amount)
    {
        if (isFinished) return;

        score += amount;
        UpdateScoreUI();

        // Condición de victoria por score (si aplica)
        if (scoreToWin > 0 && score >= scoreToWin)
        {
            Win();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (isFinished) return;
        isFinished = true;

        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", score);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + score);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);

        // --- MECÁNICA ANTERIOR ---
        // if (gameOverPanel != null) gameOverPanel.SetActive(true);
        // if (gameOverScoreText != null) gameOverScoreText.text = "Score: " + score;
        // Time.timeScale = 0f;
        // -------------------------
    }

    public void Win()
    {
        if (isFinished) return;
        isFinished = true;

        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", score);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + score);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Victoria", LoadSceneMode.Single);
    }
}



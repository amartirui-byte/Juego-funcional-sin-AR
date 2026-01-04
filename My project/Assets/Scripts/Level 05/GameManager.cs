using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nivel")]
    public float levelDuration = 60f;      // duración total en segundos
    public int starGoal = 5;              // estrellas para ganar
    public int totalStarsInLevel = 10;    // para mostrar en UI

    [Header("UI")]
    public TextMeshProUGUI timeText; // para mostrar el tiempo del juego
    public TextMeshProUGUI starsText; // para mostrar en contador de estrellas
    public TextMeshProUGUI messageText;   // para mostrar "Has ganado" / "Has perdido"

    [Header("Dificultad")]
    public AutoScroller3 scroller;
    public ObstacleSpawner obstacleSpawner;
    public float scrollSpeedStart = 3f;
    public float scrollSpeedIncrement = 1f;

    float elapsed;
    int difficultyStage = 0;

    float timeRemaining;
    public int starsCollected = 0;
    bool gameEnded = false;
    public float TimeRemaining => timeRemaining;
    public GameObject retryButton;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timeRemaining = levelDuration;
        UpdateStarsUI();
        UpdateTimeUI();

        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }

        if (retryButton != null)
            retryButton.SetActive(false);

        Time.timeScale = 1f;
        elapsed = 0f;
        difficultyStage = 0;

        if (scroller != null)
            scroller.SetSpeed(scrollSpeedStart);
    }

    void Update()
    {
        if (gameEnded) return;

        // Contador de tiempo
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            CheckWinCondition();
        }

        // Tiempo transcurrido
        elapsed = levelDuration - timeRemaining;

        // Subida de dificultad (30s y 60s)
        UpdateDifficulty();

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timeText.text = "Tiempo: " + seconds.ToString();
        }
    }

    void UpdateStarsUI()
    {
        if (starsText != null)
        {
            starsText.text = $"Estrellas: {starsCollected}/{totalStarsInLevel}";
        }
    }

    public void AddStar()
    {
        starsCollected++;
        UpdateStarsUI();
    }

    void CheckWinCondition()
    {
        if (gameEnded) return;

        if (starsCollected >= starGoal)
        {
            Win();
        }
        else
        {
            GameOver();
        }
    }
    private void GuardarPuntuacionPartida() // Guarda la putuación de estrellas en los menús de transición
    {
        int puntosPartida = starsCollected; // 1 punto = 1 estrella (puedes cambiarlo si queréis)

        PlayerPrefs.SetInt("PuntuacionPartida", puntosPartida);

        int totalActual = PlayerPrefs.GetInt("Puntuacion", 0);
        PlayerPrefs.SetInt("Puntuacion", totalActual + puntosPartida);

        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;

        // El tiempo en 1 al cambiar de escena
        Time.timeScale = 1f;
        Debug.Log("HAS PERDIDO");
        GuardarPuntuacionPartida();

        // --- COMPORTAMIENTO ANTERIOR (por se necesita más adelante) ---
        // if (messageText != null)
        // {
        //     messageText.text = "Has perdido";
        //     messageText.gameObject.SetActive(true);
        // }
        //
        // if (retryButton != null)
        //     retryButton.SetActive(true);
        // -------------------------------------------------------------------

        // Escena global de derrota del proyecto
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }


    public void Win()
    {
        if (gameEnded) return;

        gameEnded = true;

        // El tiempo esté en 1 al cambiar de escena
        Time.timeScale = 1f;
        Debug.Log("HAS GANADO");
        GuardarPuntuacionPartida();

        // --- COMPORTAMIENTO ANTERIOR (por si se necesita más adelante) ---
        // if (messageText != null)
        // {
        //     messageText.text = "You Win!";
        //     messageText.gameObject.SetActive(true);
        // }
        // -------------------------------------------------------------------

        // Ahora delegamos la pantalla de victoria en la escena global "Victoria"
        CargarVictoria();
    }


    public void RestartLevel()
    {
        // Lo silenciamos por si se necesita más adelante
        // Time.timeScale = 1f;
        // Scene scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(scene.buildIndex);
    }

    void UpdateDifficulty()
    {
        if (elapsed >= 20f && difficultyStage == 0)
        {
            difficultyStage = 1;
            IncreaseDifficulty();
        }
        else if (elapsed >= 40f && difficultyStage == 1)
        {
            difficultyStage = 2;
            IncreaseDifficulty();
        }
    }

    void IncreaseDifficulty()
    {
        Debug.Log("Subiendo dificultad");

        if (scroller != null)
        {
            scrollSpeedStart += scrollSpeedIncrement;
            scroller.SetSpeed(scrollSpeedStart);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.MakeHarder(0.85f);
        }
    }

    private void CargarVictoria()
    {
        Debug.Log("Cargando escena: Victoria");
        SceneManager.LoadScene("Victoria", LoadSceneMode.Single);
    }
    // Anterior código para las escenas de victoria y derrota:
    // private void OnVictoriaSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (scene.name == "Victoria")
    //     {
    //         Debug.Log("Escena de Victoria cargada completamente.");
    //         SceneManager.sceneLoaded -= OnVictoriaSceneLoaded;
    //         StartCoroutine(EsperarVictoria());
    //     }
    // }

    // private System.Collections.IEnumerator EsperarVictoria()
    // {
    //     Debug.Log("Esperando 3 segundos de Victoria...");
    //     yield return new WaitForSecondsRealtime(3f);
    //     Debug.Log("Descargando escena Victoria");
    //     SceneManager.UnloadSceneAsync("Victoria");
    //     yield return null;
    //     Debug.Log("Reanudando juego con Time.timeScale = 1");
    //     Time.timeScale = 1f;
    //     Debug.Log("Procesando resultado EXITO");
    // }
}

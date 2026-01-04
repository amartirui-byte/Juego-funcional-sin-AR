using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;


public class TareaCables : MonoBehaviour
{
    [Header("Controles")]
    public static TareaCables InstanciaControl { get; private set; }
    private ControlMenuPrincipal controlMenuPrincipal;
    
    [Header("Sonidos")]
    public AudioClip musica;

    [Header("Parámetros")]
    public float levelDuration = 15f;      // duración total en segundos
    public int totalStarsInLevel = 6;    // para mostrar en UI


    [Header("Elementos de la escena")]
    public TextMeshProUGUI timeText; // para mostrar el tiempo del juego
    public TextMeshProUGUI starsText; // para mostrar en contador de estrellas
    public TextMeshProUGUI messageText;   // para mostrar "Has ganado" / "Has perdido"

    public AudioSource audioSource;
    public GameObject canvasBotonPlay;
    public int conexionesActuales = 0;
    public GameObject introPanel;        // Panel de la pantalla de inicio
    public GameObject startButtonImage;  // Imagen o texto "GAME START"

    public float introTime = 8f;         // Tiempo hasta mostrar "GAME START"
    public float gameStartShowTime = 1.5f; // Tiempo que se ve "GAME START" antes de empezar
    float timeRemaining;
    bool gameEnded = false;

    public float TimeRemaining => timeRemaining;



    private void Awake()
    {
        InstanciaControl = this;

    }

    private void OnDestroy()
    {
        if (InstanciaControl == this)
        {
            InstanciaControl = null;
        }
    }
    private void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        controlMenuPrincipal = ControlMenuPrincipal.InstanciaControl;
        audioSource.clip = musica;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        if (controlMenuPrincipal != null) audioSource.volume = controlMenuPrincipal.volumenMusica;
        audioSource.Play();
        StartCoroutine(IntroSequence());
        timeRemaining = levelDuration;
        UpdateStarsUI();
        UpdateTimeUI();

    }

    private void Update()
    {
        UpdateStarsUI();
        UpdateTimeUI();

        if (gameEnded) return;

        // Contador de tiempo
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame(conexionesActuales >= totalStarsInLevel);
        }
    }


    void UpdateStarsUI()
    {
        if (starsText != null)
        {
            starsText.text = $"Cables: {conexionesActuales}/{totalStarsInLevel}";
        }
    }
    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timeText.text = "Tiempo: " + seconds.ToString();
        }
    }

    public void ComprobarVictoria()
    {
        if (conexionesActuales >= totalStarsInLevel && !gameEnded)
        {
            EndGame(true);
        }
    }


    private void CargarVictoria()
    {
        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", conexionesActuales);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + conexionesActuales);
        PlayerPrefs.Save();
        Debug.Log("Cargando escena: Victoria");
        SceneManager.LoadScene("Victoria", LoadSceneMode.Single);
    }

    private void CargarDerrota()
    {
        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", conexionesActuales);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + conexionesActuales);
        PlayerPrefs.Save();
        Debug.Log("Cargando escena: GameOver");
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }
    // Función para lanzar espera de la escena de Victoria cuando se carga
    private void OnVictoriaSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Victoria")
        {
            Debug.Log("Escena de Victoria cargada completamente.");
            // Desuscribirse del evento de carga de escena
            SceneManager.sceneLoaded -= OnVictoriaSceneLoaded;
            // Desactivar el objeto de la escena actual
            StartCoroutine(EsperarVictoria());
        }
    }

    // Función para esperar antes de descargar la escena de Victoria
    IEnumerator EsperarVictoria()
    {

        Debug.Log("Esperando 3 segundos de Victoria...");
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("Descargando escena Victoria");
        SceneManager.UnloadSceneAsync("Victoria");
        yield return null;
        Debug.Log("Reanudando juego con Time.timeScale = 1");
        Time.timeScale = 1f;
        Debug.Log("Procesando resultado EXITO");
    }

    IEnumerator IntroSequence()
    {
        // Pausar el juego y preparar la intro
        Time.timeScale = 0f;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (startButtonImage != null)
            startButtonImage.SetActive(false);   // aún no se ve "GAME START"


        // Esperar x segundos en tiempo real (aunque el juego esté pausado)
        yield return new WaitForSecondsRealtime(introTime);

        // Mostrar "GAME START"
        if (startButtonImage != null)
            startButtonImage.SetActive(true);

        // Dejarlo un momento en pantalla
        yield return new WaitForSecondsRealtime(gameStartShowTime);

        // Empezar el juego de verdad
        StartGame();
    }

    // Si quieres, también puedes llamar a esto desde un botón
    public void StartGame()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        if (startButtonImage != null)
            startButtonImage.SetActive(false);
        Time.timeScale = 1f; // reanuda el juego


    }

    private void EndGame(bool victory)
    {
        if (gameEnded) return; // por seguridad
        gameEnded = true;

        // Opcional: mostrar mensaje en pantalla
        if (messageText != null)
            messageText.text = victory ? "¡Has ganado!" : "Has perdido";

        // Si quieres pausar lógica de juego pero permitir cambio de escena:
        // Time.timeScale = 1f; // mantenlo en 1 para que LoadScene funcione sin problemas

        if (victory)
            CargarVictoria();
        else
            CargarDerrota();
    }

}
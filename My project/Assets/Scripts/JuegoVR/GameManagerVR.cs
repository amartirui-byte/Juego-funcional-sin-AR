using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManagerVR : MonoBehaviour
{
    // Singleton
    public static GameManagerVR Instance { get; private set; }

    [Header("Transiciones")]
    public GameOverVR gameOverVR;
    public VictoryVR victoryVR;

    [Header("Enemigos")]
    public GameObject enemigoPrefab;
    public Transform[] spawnPoints;
    private int enemigosVivos = 0;

    [Header("Spawn Aleatorio")]
    public Transform jugador;
    public float radioSpawnMin = 180f;
    public float radioSpawnMax = 150f;
    public float alturaSpawn = 0f;

    [Header("Spawn settings")]
    public float intervaloSpawnInicial = 1f;
    public float intervaloSpawnMinimo = 1f;
    public float decrementoIntervalo = 1f;
    public int MaxEnemigos = 10;

    [Header("Jugador")]
    public int vidaJugador = 1;

    [Header("UI")]
    public Text contadorEnemigosText;

    public int EnemigosDestruidos = 0;
    private int enemigosTotales = 0;
    private float intervaloActual;
    private bool juegoTerminado = false;

    private void Awake()
    {
        // Inicializar singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        intervaloActual = intervaloSpawnInicial;

        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[3];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                GameObject sp = new GameObject("AutoSpawnPoint_" + i);
                sp.transform.position = new Vector3(i * 2f, 0f, 10f);
                spawnPoints[i] = sp.transform;
            }
            Debug.Log("Spawn points generados automáticamente.");
        }
        SpawnOleada();
        StartCoroutine(SpawnEnemigos());
        ActualizarUI();
    }
    private IEnumerator SpawnEnemigos()
    {
        while (!juegoTerminado && enemigosTotales < MaxEnemigos)
        {
            SpawnOleada();
            yield return new WaitForSeconds(intervaloActual);

            if (intervaloActual > intervaloSpawnMinimo)
                intervaloActual -= decrementoIntervalo;
        }
    }

    private void SpawnOleada()
    {
        if (enemigosTotales >= MaxEnemigos) return;
        if (enemigoPrefab == null || jugador == null) return;

        // Dirección aleatoria en plano horizontal
        Vector2 dir2D = Random.insideUnitCircle.normalized;

        // Distancia aleatoria dentro del anillo
        float distancia = Random.Range(radioSpawnMin, radioSpawnMax);

        // Posición final
        Vector3 posicionSpawn = jugador.position +
                                new Vector3(dir2D.x, 0f, dir2D.y) * distancia;

        posicionSpawn.y = alturaSpawn;

        Instantiate(enemigoPrefab, posicionSpawn, Quaternion.identity);

        enemigosVivos++;
        enemigosTotales++;
        ActualizarUI();
    }

    public void EnemigoDestruido()
    {
        EnemigosDestruidos++;
        ActualizarUI();

        if (EnemigosDestruidos >= MaxEnemigos)
            Victoria();
    }

    public void RecibirDaño(int cantidad)
    {
        vidaJugador -= cantidad;
        if (vidaJugador <= 0)
        {
            vidaJugador = 0;
            FinDelJuego();
        }
    }

    private void FinDelJuego()
    {
        juegoTerminado = true;
        Debug.Log("Juego terminado. Enemigos destruidos: " + EnemigosDestruidos + "/" + enemigosTotales);
    }

    public void GameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", EnemigosDestruidos);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + EnemigosDestruidos);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);

        // --- MECÁNICA ANTERIOR ---
        // if (gameOverPanel != null) gameOverPanel.SetActive(true);
        // if (gameOverScoreText != null) gameOverScoreText.text = "Score: " + score;
        // Time.timeScale = 0f;
        // -------------------------
    }
    public void Victoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        // Guardar puntuación para la escena común
        PlayerPrefs.SetInt("PuntuacionPartida", EnemigosDestruidos);
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") + EnemigosDestruidos);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Victoria", LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        if (contadorEnemigosText != null)
            contadorEnemigosText.text = "Enemigos: " + EnemigosDestruidos + "/" + MaxEnemigos;
    }
}

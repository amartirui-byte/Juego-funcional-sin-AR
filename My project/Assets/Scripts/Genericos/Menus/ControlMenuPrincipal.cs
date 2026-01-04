using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControlMenuPrincipal : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioClip musica;
    public AudioClip sonidoJuego;
    public AudioSource audioSource;

    [SerializeField, Range(0f, 1f)]
    public float volumenMusica = 1f;

    [Header("Orientación")]
    [Tooltip("Si está activado, en móvil fuerza orientación vertical (Portrait).")]
    [SerializeField] private bool forzarVerticalEnMovil = false;

    [Tooltip("Si está activado, fuerza Portrait en móvil en TODAS las escenas. Si no, solo en MenuPrincipal.")]
    [SerializeField] private bool forzarEnTodasLasEscenas = true;

    public static ControlMenuPrincipal InstanciaControl { get; private set; }

    public enum ModoJuego { Individual, Continuo }
    public ModoJuego modoActual;

    public enum ResultadoMinijuego { Exito, Derrota, Reiniciar, Menu, Salir }
    public ResultadoMinijuego resultadoMinijuego;

    private string escenaActual = "";
    public int indiceActual = 0;

    public Button BtnSalirJuego;
    public Button BtnPlay;
    public Button BtnPlay1;
    public Button BtnPlay2;
    public Button BtnPlay3;
    public Button BtnPlay4;
    public Button BtnPlay5;

    // Lista de escenas de minijuegos (orden correcto para modo Continuo)
    private readonly List<string> escenasMinijuegos = new List<string>()
    {
        "Amongus",          // Reconexión de circuitos (2D)
        "Mini Juego_Final", // Navegación entre asteroides (2D)
        "JuegoVR",          // Contrabandistas espaciales (VR)
        "JuegoAR",          // En busca de aliens (AR)
        "Level_05",         // Carrera hacia la Tierra (2D)
    };

    // Lista paralela con los nombres "bonitos" que se verán en el menú
    private readonly List<string> nombresBonitos = new List<string>()
    {
        "Reconexión de circuitos (2D)",
        "Navegación entre asteroides (2D)",
        "Contrabandistas espaciales (VR)",
        "En busca de aliens (AR)",
        "Carrera hacia la Tierra (2D)"
    };

    private void Awake()
    {
        if (InstanciaControl != null && InstanciaControl != this)
        {
            Destroy(gameObject);
            return;
        }

        InstanciaControl = this;
        DontDestroyOnLoad(gameObject);

        AplicarOrientacionSiProcede(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;

        InicializarMenu();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AplicarOrientacionSiProcede(scene.name);

        if (scene.name == "MenuPrincipal")
        {
            StartCoroutine(ReinicializarMenu());
        }
    }

    private void AplicarOrientacionSiProcede(string nombreEscena)
    {
        // Si NO quieres que este script toque la orientación, desactívalo con este bool.
        if (!Application.isMobilePlatform) return;

        // Horizontal en móvil:
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.AutoRotation;

        // Y fijamos una orientación por defecto (robusto en algunos móviles):
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private IEnumerator ReinicializarMenu()
    {
        yield return null;
        InicializarMenu();
        ReproducirMusicaMenuSiProcede();
    }

    private void InicializarMenu()
    {
        AplicarOrientacionSiProcede("MenuPrincipal");

        modoActual = ModoJuego.Individual;
        resultadoMinijuego = ResultadoMinijuego.Menu;

        PlayerPrefs.SetString("EscenaActual", "MenuPrincipal");
        PlayerPrefs.SetInt("IndiceMinijuego", 0);
        PlayerPrefs.Save();

        escenaActual = "MenuPrincipal";
        indiceActual = 0;

        AsignarBotones();
        ActivarBotones();
        AsignarFuncionesBotones();
        AsignarNombresBonitos();

        PrepararMusicaMenu();
        ReproducirMusicaMenuSiProcede();
    }

    private void PrepararMusicaMenu()
    {
        if (audioSource == null) return;

        audioSource.clip = musica;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volumenMusica;
    }

    private void ReproducirMusicaMenuSiProcede()
    {
        if (audioSource == null) return;
        if (!audioSource.isPlaying) audioSource.Play();
    }

    private void AsignarBotones()
    {
        BtnSalirJuego = BuscarBoton("BtnSalirJuego");
        BtnPlay = BuscarBoton("BtnPlay");
        BtnPlay1 = BuscarBoton("BtnPlay1");
        BtnPlay2 = BuscarBoton("BtnPlay2");
        BtnPlay3 = BuscarBoton("BtnPlay3");
        BtnPlay4 = BuscarBoton("BtnPlay4");
        BtnPlay5 = BuscarBoton("BtnPlay5");
    }

    private Button BuscarBoton(string nombre)
    {
        GameObject boton = GameObject.Find(nombre);
        return boton != null ? boton.GetComponent<Button>() : null;
    }

    private void AsignarNombresBonitos()
    {
        SetTMPText(BtnPlay1, nombresBonitos, 0);
        SetTMPText(BtnPlay2, nombresBonitos, 1);
        SetTMPText(BtnPlay3, nombresBonitos, 2);
        SetTMPText(BtnPlay4, nombresBonitos, 3);
        SetTMPText(BtnPlay5, nombresBonitos, 4);
    }

    private void SetTMPText(Button btn, List<string> textos, int index)
    {
        if (btn == null) return;
        if (textos == null || index < 0 || index >= textos.Count) return;

        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null) tmp.text = textos[index];
    }

    private void AsignarFuncionesBotones()
    {
        // Importante con DontDestroyOnLoad: evita listeners duplicados si se reinicializa el menú.
        LimpiarListeners(BtnPlay);
        LimpiarListeners(BtnPlay1);
        LimpiarListeners(BtnPlay2);
        LimpiarListeners(BtnPlay3);
        LimpiarListeners(BtnPlay4);
        LimpiarListeners(BtnPlay5);
        LimpiarListeners(BtnSalirJuego);

        if (BtnPlay != null) BtnPlay.onClick.AddListener(Click_JugarTodos);
        if (BtnPlay1 != null) BtnPlay1.onClick.AddListener(Click_Juego1);
        if (BtnPlay2 != null) BtnPlay2.onClick.AddListener(Click_Juego2);
        if (BtnPlay3 != null) BtnPlay3.onClick.AddListener(Click_Juego3);
        if (BtnPlay4 != null) BtnPlay4.onClick.AddListener(Click_Juego4);
        if (BtnPlay5 != null) BtnPlay5.onClick.AddListener(Click_Juego5);
        if (BtnSalirJuego != null) BtnSalirJuego.onClick.AddListener(Click_SalirJuego);
    }

    private void LimpiarListeners(Button btn)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
    }

    private void DesactivarBotones()
    {
        if (BtnPlay != null) BtnPlay.interactable = false;
        if (BtnPlay1 != null) BtnPlay1.interactable = false;
        if (BtnPlay2 != null) BtnPlay2.interactable = false;
        if (BtnPlay3 != null) BtnPlay3.interactable = false;
        if (BtnPlay4 != null) BtnPlay4.interactable = false;
        if (BtnPlay5 != null) BtnPlay5.interactable = false;
        if (BtnSalirJuego != null) BtnSalirJuego.interactable = false;
    }

    private void ActivarBotones()
    {
        if (BtnPlay != null) BtnPlay.interactable = true;
        if (BtnPlay1 != null) BtnPlay1.interactable = true;
        if (BtnPlay2 != null) BtnPlay2.interactable = true;
        if (BtnPlay3 != null) BtnPlay3.interactable = true;
        if (BtnPlay4 != null) BtnPlay4.interactable = true;
        if (BtnPlay5 != null) BtnPlay5.interactable = true;
        if (BtnSalirJuego != null) BtnSalirJuego.interactable = true;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Si quieres, aquí puedes manejar escape.
        }
    }

    public IEnumerator JugarMinijuego(string nombreEscena)
    {
        escenaActual = nombreEscena;

        if (audioSource != null) audioSource.Stop();
        DesactivarBotones();

        PlayerPrefs.SetString("EscenaActual", nombreEscena);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nombreEscena);
        yield return null;
    }

    public void Click_Juego1()
    {
        modoActual = ModoJuego.Individual;
        StartCoroutine(JugarMinijuego("Amongus"));
    }

    public void Click_Juego2()
    {
        modoActual = ModoJuego.Individual;
        StartCoroutine(JugarMinijuego("Mini Juego_Final"));
    }

    public void Click_Juego3()
    {
        modoActual = ModoJuego.Individual;
        StartCoroutine(JugarMinijuego("JuegoVR"));
    }

    public void Click_Juego4()
    {
        modoActual = ModoJuego.Individual;
        StartCoroutine(JugarMinijuego("JuegoAR"));
    }

    public void Click_Juego5()
    {
        modoActual = ModoJuego.Individual;
        StartCoroutine(JugarMinijuego("Level_05"));
    }

    public void Click_SalirJuego()
    {
        if (audioSource != null) audioSource.Stop();
        Application.Quit();
    }

    public void Click_JugarTodos()
    {
        modoActual = ModoJuego.Continuo;
        indiceActual = PlayerPrefs.GetInt("IndiceMinijuego");
        StartCoroutine(JugarMinijuego(escenasMinijuegos[indiceActual]));
    }

    public void SiguienteMinijuego()
    {
        indiceActual = PlayerPrefs.GetInt("IndiceMinijuego") + 1;

        if (indiceActual < escenasMinijuegos.Count)
        {
            escenaActual = escenasMinijuegos[indiceActual];
            PlayerPrefs.SetString("EscenaActual", escenaActual);
            PlayerPrefs.SetInt("IndiceMinijuego", indiceActual);
            PlayerPrefs.Save();

            SceneManager.LoadScene(escenasMinijuegos[indiceActual]);

            Debug.Log("[MenuPrincipal] Cargando escena: " + escenasMinijuegos[indiceActual]);
            Debug.Log("[MenuPrincipal] Indice actual: " + indiceActual);
            Debug.Log("[MenuPrincipal] Escena actual: " + escenaActual);
        }
        else
        {
            PlayerPrefs.SetInt("IndiceMinijuego", 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("EscenaFinal");
        }
    }

    public void ProcesarResultado(ResultadoMinijuego resultado)
    {
        resultadoMinijuego = resultado;

        if (resultado == ResultadoMinijuego.Exito)
        {
            SceneManager.LoadScene("YouWin");
        }
        else if (resultado == ResultadoMinijuego.Derrota)
        {
            SceneManager.LoadScene("GameOver");
        }
        else if (resultado == ResultadoMinijuego.Reiniciar)
        {
            StartCoroutine(JugarMinijuego(escenaActual));
        }
        else if (resultado == ResultadoMinijuego.Menu)
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
        else if (resultado == ResultadoMinijuego.Salir)
        {
            Application.Quit();
        }
    }
}

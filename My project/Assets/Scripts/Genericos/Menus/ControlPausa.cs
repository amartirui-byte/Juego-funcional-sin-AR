using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlPausa : MonoBehaviour
{


    [Header("Controles")]
    public static ControlPausa InstanciaControl { get; private set; }
    private GameObject controlEscena;
    //private ControlCardBoard controlCardboard;

    // Banderas
    private bool menuCargado = false;

    private AudioSource musica;
    private bool musicaOn = false;
    private float tiempoEscena;

    private void Awake()
    {
        if (InstanciaControl != null && InstanciaControl != this)
        {
            Destroy(gameObject);
            return;
        }
        InstanciaControl = this;
    }

    private void Start()
    {

        controlEscena = GameObject.FindGameObjectWithTag("ControlEscena");
        if (controlEscena != null)
        {
            musica = controlEscena.GetComponent<AudioSource>();
            Debug.Log("AudioSource obtenido del control de escena.");
        }
        else
        {
            Debug.LogError("Control de la escena no encontrado.");
        }


    }

    // Pausar el juego y cargar el menú de pausa
    public void PausarJuego()
    {

        // Si el menú ya está cargado, no hacemos nada
        if (!menuCargado)
        {


            if (musica != null && musica.isPlaying) {
                musicaOn = true;
                musica.Pause();
            }

            tiempoEscena = Time.timeScale;
            if (tiempoEscena != 0f) Time.timeScale = 0f;

            Debug.Log("Cargando MenuPausa...");
            // Pausar tiempo y cargar la escena del menú de pausa
            SceneManager.LoadScene("MenuPausa", LoadSceneMode.Additive);
            menuCargado = true;
            StartCoroutine(AsignarCamaraAlCanvas());

        }

    }

    // Reanudar el juego y descargar el menú de pausa
    public void ReanudarJuego()
    {

        if (musica != null && musicaOn == true) musica.Play();

        // Reanudar el tiempo y descargar la escena del menú de pausa
        Time.timeScale = tiempoEscena;
        SceneManager.UnloadSceneAsync("MenuPausa");
        menuCargado = false;

        Debug.Log("[ControlPausa] Pausa desactivada.");

    }

    // Reiniciar el juego
    public void ReiniciarJuego()
    {
        // Reiniciar el juego y cargar la escena actual
        PlayerPrefs.SetInt("PuntuacionPartida", 0);
        Time.timeScale = 1f;

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Volver al menú principal
    public void MenuPrincipal()
    {
        // Reiniciar el juego y cargar el menú principal
        PlayerPrefs.SetInt("PuntuacionPartida", 0);
        Time.timeScale = 1f;
        // Volver al menú principal
        SceneManager.LoadScene("MenuPrincipal");
    }


    // Asignar la cámara principal al canvas del menú de pausa
    private IEnumerator AsignarCamaraAlCanvas()
    {
        // Espera 1 frame para asegurar que la escena está cargada
        yield return null;

        // Asignar la cámara principal al canvas del menú de pausa
        Scene escenaPausa = SceneManager.GetSceneByName("MenuPausa");

        // Verifica si la escena del menú de pausa está cargada
        if (!escenaPausa.isLoaded)
        {
            Debug.LogError("[ControlPausa] La escena del menú de pausa no está cargada.");
            yield break;
        }

        // Recorre los objetos raíz de la escena del menú de pausa
        foreach (GameObject rootObj in escenaPausa.GetRootGameObjects())
        {
            // Busca el canvas en los objetos raíz
            Canvas canvas = rootObj.GetComponentInChildren<Canvas>(true);

            // Asigna la cámara principal al canvas si es ScreenSpaceCamera y el canvas no es null
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                if (Camera.main == null)
                {
                    Debug.LogError("[ControlPausa] No se ha encontrado una cámara principal en la escena. Creando una temporal.");
                    // Si no hay cámara principal, crea una temporal
                    GameObject tempCamera = new GameObject("MainCamera");
                    Camera camara = tempCamera.AddComponent<Camera>();
                    camara.tag = "MainCamera";
                    yield break;
                }
                // Asigna la cámara principal al canvas
                canvas.worldCamera = Camera.main;
                Debug.Log("[ControlPausa] Cámara principal asignada al canvas del menú de pausa.");
            }
        }
    }
}
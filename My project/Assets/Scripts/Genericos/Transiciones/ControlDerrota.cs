using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlDerrota : MonoBehaviour
{
    [Header("Elementos de la escena")]
    public TextMeshProUGUI TxtScore;
    public TextMeshProUGUI TxtScorePartida;

    [Header("Sonidos")]
    public AudioClip musica;
    public AudioSource audioSource;

    private ControlMenuPrincipal controlMenuPrincipal;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        controlMenuPrincipal = ControlMenuPrincipal.InstanciaControl;

        if (controlMenuPrincipal == null)
        {
            Debug.LogWarning("ControlMenuPrincipal no encontrado en escena de Derrota. Funcionará en modo local.");
        }

        // Textos de puntuación
        if (TxtScorePartida != null)
            TxtScorePartida.text = "SCORE PARTIDA: " + PlayerPrefs.GetInt("PuntuacionPartida");

        if (TxtScore != null)
            TxtScore.text = "SCORE TOTAL: " + PlayerPrefs.GetInt("Puntuacion");

        // Música
        if (audioSource != null && musica != null)
        {
            audioSource.clip = musica;
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            if (controlMenuPrincipal != null)
                audioSource.volume = controlMenuPrincipal.volumenMusica;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    // Click en el botón de reset
    public void Click_BtnReset()
    {
        // Ajustar puntuación siempre
        PlayerPrefs.SetInt("Puntuacion", PlayerPrefs.GetInt("Puntuacion") - PlayerPrefs.GetInt("PuntuacionPartida"));
        PlayerPrefs.SetInt("PuntuacionPartida", 0);
        PlayerPrefs.Save();

        if (controlMenuPrincipal != null)
        {
            // Flujo normal (modo continuo, etc.)
            controlMenuPrincipal.ProcesarResultado(ControlMenuPrincipal.ResultadoMinijuego.Reiniciar);
        }
        else
        {
            // Modo “local”: simplemente volvemos al menú principal
            SceneManager.LoadScene("MenuPrincipal");
        }
    }

    // Click en el botón de salir
    public void Click_BtnMenu()
    {
        if (controlMenuPrincipal != null)
        {
            controlMenuPrincipal.ProcesarResultado(ControlMenuPrincipal.ResultadoMinijuego.Menu);
        }
        else
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}

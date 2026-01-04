using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlExito : MonoBehaviour
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
            Debug.LogWarning("ControlMenuPrincipal no encontrado en escena de Éxito. Funcionará en modo local.");
        }

        if (TxtScorePartida != null)
            TxtScorePartida.text = "PUNTUACIÓN PARTIDA: " + PlayerPrefs.GetInt("PuntuacionPartida");

        if (TxtScore != null)
            TxtScore.text = "PUNTUACIÓN TOTAL: " + PlayerPrefs.GetInt("Puntuacion");

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

    // Click en el botón de continuar
    public void Click_BtnContinuar()
    {
        if (controlMenuPrincipal != null)
        {
            // Solo continuar al siguiente minijuego si estamos en modo Continuo
            if (controlMenuPrincipal.modoActual == ControlMenuPrincipal.ModoJuego.Continuo)
            {
                controlMenuPrincipal.SiguienteMinijuego();
            }
            else
            {
             // Si el juego se ha lanzado en modo Individual, volvemos al menú
                SceneManager.LoadScene("MenuPrincipal");
            }
        }
        else
        {
            // Sin ControlMenuPrincipal, comportamiento simple
            SceneManager.LoadScene("MenuPrincipal");
        }
    }

}


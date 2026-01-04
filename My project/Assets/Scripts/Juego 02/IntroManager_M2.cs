using UnityEngine;
using System.Collections;   // Necesario para las corutinas

public class IntroManager_M2 : MonoBehaviour
{
    public GameObject introPanel;        // Panel de la pantalla de inicio
    public GameObject startButtonImage;  // Imagen o texto "GAME START"
    public GameObject asteroidSpawner;   // Objeto que genera asteroides

    public float introTime = 8f;         // Tiempo hasta mostrar "GAME START"
    public float gameStartShowTime = 1.5f; // Tiempo que se ve "GAME START" antes de empezar

    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Pausar el juego y preparar la intro
        Time.timeScale = 0f;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (startButtonImage != null)
            startButtonImage.SetActive(false);   // aún no se ve "GAME START"

        if (asteroidSpawner != null)
            asteroidSpawner.SetActive(false);    // todavía no hay asteroides

        // Esperar 8 segundos en tiempo real (aunque el juego esté pausado)
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

        if (asteroidSpawner != null)
            asteroidSpawner.SetActive(true);

        Time.timeScale = 1f; // reanuda el juego
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryVR : MonoBehaviour
{
    public void LanzarVictoria()
    {
        Debug.Log("Victoria iniciada");
        Time.timeScale = 0f;

        Invoke(nameof(Salir), 3f);
    }

    private void Salir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

public void OnVictoriaSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Victoria")
        {
            Debug.Log("Escena de Victoria cargada completamente.");
            SceneManager.sceneLoaded -= OnVictoriaSceneLoaded;
            StartCoroutine(EsperarVictoria());
        }
    }

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
}

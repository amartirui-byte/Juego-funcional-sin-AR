using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverVR : MonoBehaviour
{
    public void LanzarGameOver()
    {
        Debug.Log("GameOver iniciado");
        Time.timeScale = 0f;

        Invoke(nameof(Salir), 3f);
    }

    private void Salir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void OnGameOverSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOver")
        {
            Debug.Log("Escena de GameOver cargada completamente.");
            SceneManager.sceneLoaded -= OnGameOverSceneLoaded;
            StartCoroutine(EsperarGameOver());
        }
    }

    IEnumerator EsperarGameOver()
    {
        Debug.Log("Esperando 3 segundos de GameOver...");
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("Descargando escena GameOver");
        SceneManager.UnloadSceneAsync("GameOver");
        yield return null;
        Debug.Log("Reanudando juego con Time.timeScale = 1");
        Time.timeScale = 1f;
        Debug.Log("Procesando resultado EXITO");
    }
}

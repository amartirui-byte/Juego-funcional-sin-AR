using System.Collections;
using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using Google.XR.Cardboard;
using UnityEngine.XR.Management;
#endif

public class PlayerVR : MonoBehaviour
{
    [Header("Vida del jugador")]
    public int vidaMax = 1;
    private int vidaActual;

    [Header("Game Over")]
    public GameOverVR gameOverVR;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform puntoDisparo;
    public float cadencia = 0.3f;
    private float tiempoUltimoDisparo;

    [Header("Sobrecalentamiento")]
    public int maxDisparosSeguidos = 8;
    public float cooldownTime = 1.5f;
    private int disparosSeguidos = 0;
    private bool enCooldown = false;

    [Header("Daño visual")]
    public CanvasGroup dañoOverlay;
    public float dañoDuracion = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;
    public AudioClip sonidoImpacto;
    public AudioClip sonidoSobrecalentado;

    [Header("Vibración")]
    public bool vibracion = true;

#if UNITY_ANDROID || UNITY_IOS
    private bool xrInitialized = false;
#endif

    private void Start()
    {
        vidaActual = vidaMax;

#if UNITY_ANDROID || UNITY_IOS
        StartCoroutine(InitXR());
#endif
    }

#if UNITY_ANDROID || UNITY_IOS
    private IEnumerator InitXR()
    {
        // En Editor/PC, aunque el build target sea Android, esto evita intentar Cardboard.
        if (!Application.isMobilePlatform)
            yield break;

        yield return UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoader();

        var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader;
        if (loader == null)
        {
            Debug.LogError("No se pudo inicializar ningún XR Loader (activeLoader = null).");
            yield break;
        }

        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
        xrInitialized = true;
    }
#endif

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!xrInitialized) return;

        Api.UpdateScreenParams();

        if (Api.IsTriggerPressed)
            IntentarDisparar();
#endif
        // En PC no hacemos rotación ni input aquí:
        // lo gestiona PlayerVRSimulator.
    }

    public void IntentarDisparar()
    {
        if (enCooldown) return;
        if (Time.time - tiempoUltimoDisparo < cadencia) return;

        Disparar();
    }

    private void Disparar()
    {
        if (projectilePrefab == null || puntoDisparo == null) return;

        Instantiate(projectilePrefab, puntoDisparo.position, puntoDisparo.rotation);
        tiempoUltimoDisparo = Time.time;

        Sonido(sonidoDisparo);
        Vibrar();

        disparosSeguidos++;

        if (disparosSeguidos >= maxDisparosSeguidos)
            StartCoroutine(EntrarCooldown());
    }

    private IEnumerator EntrarCooldown()
    {
        enCooldown = true;
        Sonido(sonidoSobrecalentado);

        yield return new WaitForSeconds(cooldownTime);

        disparosSeguidos = 0;
        enCooldown = false;
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;

        StartCoroutine(DañoVisual());
        Sonido(sonidoImpacto);
        Vibrar();

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
            return;
        }

        GameManagerVR.Instance.RecibirDaño(cantidad);
    }

    private void Morir()
    {
        Debug.Log("Jugador ha muerto");

        if (gameOverVR != null)
            gameOverVR.LanzarGameOver();
        else
            Debug.LogError("GameOverVR no asignado en PlayerVR");
    }

    private IEnumerator DañoVisual()
    {
        if (dañoOverlay == null) yield break;

        dañoOverlay.alpha = 1f;
        float t = 0f;

        while (t < dañoDuracion)
        {
            t += Time.deltaTime;
            dañoOverlay.alpha = Mathf.Lerp(1f, 0f, t / dañoDuracion);
            yield return null;
        }

        dañoOverlay.alpha = 0f;
    }

    private void Sonido(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    private void Vibrar()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibracion)
            Handheld.Vibrate();
#endif
    }
}

using TMPro;
using UnityEngine;

public class ControlMenuPausa : MonoBehaviour
{
    [Header("Controles")]
    public ControlPausa controlPausa;

    [Header("Componentes")]
    public TextMeshProUGUI textoPausa;

    [Header("Colores")]
    public int r = 255, g = 0, b = 0;
    public bool rojo = true, verde = false, azul = false;

    // Variables
    private float hue = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        controlPausa = ControlPausa.InstanciaControl;
        // Inicializa el texto de pausa
        textoPausa = FindFirstObjectByType<TextMeshProUGUI>();

        if (PlayerPrefs.GetString("EscenaActual") == "JuegoVRB"
            || PlayerPrefs.GetString("EscenaActual") == "JuegoAR")
        {
            if (Camera.main == null)
            {
                Debug.LogError("[ControlMenuPausa] No se ha encontrado la cámara principal.");

                // Añadir nueva cámara si no se encuentra la principal
                Camera camara = new GameObject("MainCamera").AddComponent<Camera>();
                camara.tag = "MainCamera";
                camara.enabled = true;

                return;
            }
            AdaptarCanvasParaVR();
        }
     
    }

    // Función para reanudar el juego
    public void Reanudar()
    {
        controlPausa.ReanudarJuego();
    }

    // Función para reiniciar el juego
    public void Reiniciar()
    {
        controlPausa.ReiniciarJuego();
    }

    // Función para salir al menú principal
    public void MenuPrincipal()
    {
        controlPausa.MenuPrincipal();
    }

    // Update is called once per frame
    void Update()
    {
        CambiarColorTexto();
    }

    // Función para cambiar el color del texto de pausa
    public void CambiarColorTexto()
    {
        hue += Time.deltaTime * 0.1f; // velocidad de cambio
        if (hue > 1f) hue = 0f;

        Color nuevoColor = Color.HSVToRGB(hue, 1f, 1f);
        textoPausa.color = nuevoColor;
    }

    private void AdaptarCanvasParaVR()
    {
        if (!TryGetComponent<Canvas>(out var canvas)) return;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        // Poner el menú delante del jugador
        Transform camara = Camera.main.transform;
        canvas.transform.SetParent(camara);

        // Ajustar la posición y rotación del canvas
        canvas.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 2f), Quaternion.identity);
        canvas.transform.localScale = Vector3.one * 0.0025f; // Ajusta según tamaño real

        // Orden de renderizado alto para estar por delante del HUD
        canvas.sortingOrder = 1000;
    }
}

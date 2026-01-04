using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthoFitToAspect : MonoBehaviour
{
    public float referenceOrthoSize = 5f;      // el size “correcto” en tu referencia
    public float referenceAspect = 16f / 9f;   // aspecto para el que lo diseñaste (Landscape)

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void OnEnable() => Apply();

    void Apply()
    {
        if (!cam.orthographic) return;

        float aspect = (float)Screen.width / Screen.height;

        // Si el móvil es “más estrecho” que 16:9, hacemos zoom out para que quepa el ancho
        if (aspect < referenceAspect)
            cam.orthographicSize = referenceOrthoSize * (referenceAspect / aspect);
        else
            cam.orthographicSize = referenceOrthoSize;
    }
}


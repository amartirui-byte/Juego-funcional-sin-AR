using UnityEngine;

public class AutoScroller3 : MonoBehaviour
{
    public float speed = 3f;
    public Transform[] tiles;             // BackgroundA, BackgroundB, BackgroundC...
    public Sprite earthSprite;
    public Transform earthTile;           // tile que contiene la Tierra (si aplica)

    private Camera cam;
    private SpriteRenderer[] srs;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[AutoScroller3] No hay Camera.main en la escena.");
            enabled = false;
            return;
        }

        // Cachear SpriteRenderers (más eficiente y nos permite usar bounds reales)
        srs = new SpriteRenderer[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            srs[i] = tiles[i].GetComponent<SpriteRenderer>();
            if (srs[i] == null)
            {
                Debug.LogError($"[AutoScroller3] El tile {tiles[i].name} no tiene SpriteRenderer.");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        float dx = speed * Time.deltaTime;
        Vector3 left = Vector3.left * dx;

        // Mover todos los fondos a la izquierda
        for (int i = 0; i < tiles.Length; i++)
            tiles[i].position += left;

        // Borde izquierdo visible de la cámara (ortográfica)
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Reciclar: cuando el borde derecho del tile ya está a la izquierda del borde de cámara
        for (int i = 0; i < tiles.Length; i++)
        {
            Transform t = tiles[i];

            // Si este tile es el de la Tierra, NO lo reciclamos
            if (earthTile != null && t == earthTile)
                continue;

            float tileRightEdge = srs[i].bounds.max.x;

            if (tileRightEdge < camLeft)
            {
                // Buscar el borde derecho más a la derecha entre TODOS los tiles
                float rightMostEdge = srs[0].bounds.max.x;
                for (int j = 1; j < tiles.Length; j++)
                {
                    float edge = srs[j].bounds.max.x;
                    if (edge > rightMostEdge) rightMostEdge = edge;
                }

                // Recolocar este tile pegándolo justo después del más a la derecha (sin hueco)
                float tileWidth = srs[i].bounds.size.x;
                float newCenterX = rightMostEdge + (tileWidth * 0.5f);

                t.position = new Vector3(newCenterX, t.position.y, t.position.z);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}





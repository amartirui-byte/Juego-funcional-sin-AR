using UnityEngine;

public class EarthBackgroundController : MonoBehaviour
{
    [Header("Referencias")]
    public AutoScroller3 scroller;
    public Sprite earthSprite;

    [Header("Cuándo mostrar la Tierra")]
    public float triggerTimeRemaining = 20f; // segundos restantes

    bool triggered = false;

    void Update()
    {
        if (triggered) return;
        if (GameManager.Instance == null) return;

        // Cuando el tiempo restante sea menor o igual al umbral
        if (GameManager.Instance.TimeRemaining <= triggerTimeRemaining)
        {
            ActivateEarthTile();
            triggered = true;
        }
    }

    void ActivateEarthTile()
    {
        if (scroller == null || scroller.tiles == null || scroller.tiles.Length == 0)
            return;

        // Encontrar el tile más a la derecha
        Transform rightmost = scroller.tiles[0];
        for (int i = 1; i < scroller.tiles.Length; i++)
        {
            if (scroller.tiles[i].position.x > rightmost.position.x)
                rightmost = scroller.tiles[i];
        }

        // Cambiar su sprite a la Tierra
        var sr = rightmost.GetComponent<SpriteRenderer>();
        if (sr != null && earthSprite != null)
        {
            sr.sprite = earthSprite;
        }

        // Avisar al AutoScroller de que este es el tile de la Tierra
        scroller.earthTile = rightmost;

        Debug.Log("Tile de la Tierra activado como último fondo");
    }
}


using UnityEngine;
using UnityEngine.UI;

public class ContadorEnemigosVR : MonoBehaviour
{
    [SerializeField] private Text contadorText;

    private void Awake()
    {
        if (contadorText == null)
        {
            Debug.LogError("ContadorEnemigosVR: Text no asignado en el inspector.");
        }
    }

    private void OnEnable()
    {
        Actualizar();
    }

    public void Actualizar()
    {
        if (GameManagerVR.Instance == null || contadorText == null) return;

        contadorText.text =
            $"Enemigos: {GameManagerVR.Instance.EnemigosDestruidos} / {GameManagerVR.Instance.MaxEnemigos}";
    }
}
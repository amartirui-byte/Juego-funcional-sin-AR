using TMPro;
using UnityEngine;

public class ControlHud : MonoBehaviour
{
    public static ControlHud InstanciaControl { get; private set; }
    public TextMeshProUGUI TxtScore;
    public TextMeshProUGUI TxtContador;
    public TextMeshProUGUI TxtTime;

    private void Awake()
    {
        InstanciaControl = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActualizarContador(string nombreContador, int contador)
    {
        TxtContador.text = nombreContador + ": " + contador;
    }

    public void ActualizarPuntos(string nombrePuntos, int puntos)
    {
        TxtScore.text = nombrePuntos + ": " + puntos;
    }

    public void ActualizarTiempo(float tiempo)
    {
        TxtTime.text = tiempo.ToString("f0");
    }

}

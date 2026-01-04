using UnityEngine;

public class SimpleExplosion_M2 : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.4f); // La explosión dura 0,4 segundos
    }
}

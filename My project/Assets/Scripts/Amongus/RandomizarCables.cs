using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizarCables : MonoBehaviour
{
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject cableActual = transform.GetChild(i).gameObject;
            GameObject otroCable = transform.GetChild(Random.Range(0, transform.childCount)).gameObject;

            Vector2 nuevaPosCableActual = otroCable.transform.position;
            Vector2 nuevaPosOtroCable = cableActual.transform.position;

            cableActual.transform.position = nuevaPosCableActual;
            otroCable.transform.position = nuevaPosOtroCable;
        }
    }
}
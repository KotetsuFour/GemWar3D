using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoMorph : MonoBehaviour
{
    [SerializeField] private GameObject[] appears;
    [SerializeField] private GameObject[] disappears;

    public void morph()
    {
        foreach (GameObject go in appears)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in disappears)
        {
            go.SetActive(false);
        }
    }
}

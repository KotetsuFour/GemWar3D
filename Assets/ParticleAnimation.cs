using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimation : MonoBehaviour
{
    private float duration;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().Play();
        duration = GetComponent<ParticleSystem>().main.duration;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimation : MonoBehaviour
{
    [SerializeField] private float duration;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().Play();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeSound : MonoBehaviour
{
    private bool started;
    public void playSound(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started && !GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

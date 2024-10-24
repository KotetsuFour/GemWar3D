using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SequenceMember : MonoBehaviour
{
    [SerializeField] private AudioSource sound_effect;
    [SerializeField] private Camera cam;
    [SerializeField] private OneTimeSound oneTimeSound;

    public abstract bool completed();

    public virtual void LEFT_MOUSE()
    {
        Debug.Log("stuck on base");
        //nothing
    }
    public virtual void RIGHT_MOUSE()
    {
        //nothing
    }
    public virtual void Z()
    {
        //nothing
    }
    public virtual void X()
    {
        //nothing
    }
    public virtual void A()
    {
        //nothing
    }
    public virtual void S()
    {
        //nothing
    }
    public virtual void UP()
    {
        //nothing
    }
    public virtual void LEFT()
    {
        //nothing
    }
    public virtual void DOWN()
    {
        //nothing
    }
    public virtual void RIGHT()
    {
        //nothing
    }
    public virtual void ENTER()
    {
        //nothing
    }
    public virtual void ESCAPE()
    {
        //nothing
    }
    public AudioSource getAudioSource(string soundName)
    {
        return getAudioSource(AssetDictionary.getAudio(soundName));
    }
    public AudioSource getAudioSource(AudioClip clip)
    {
        AudioSource ret = Instantiate(sound_effect);
        ret.clip = clip;
        ret.volume = StaticData.musicVolume;
        return ret;
    }
    public Camera getCamera()
    {
        return cam;
    }
    public void playOneTimeSound(AudioClip clip)
    {
        OneTimeSound once = Instantiate(oneTimeSound);
        once.GetComponent<AudioSource>().volume = StaticData.sfxVolume;
        once.playSound(clip);
    }
    public void playOneTimeSound(string soundName)
    {
        playOneTimeSound(AssetDictionary.getAudio(soundName));
    }

}

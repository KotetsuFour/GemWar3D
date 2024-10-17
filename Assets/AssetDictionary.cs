using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetDictionary : MonoBehaviour
{
    [SerializeField] private List<string> imageKeys;
    [SerializeField] private List<Sprite> imageValues;
    [SerializeField] private List<GameObject> modelValues;
    [SerializeField] private List<string> decoKeys;
    [SerializeField] private List<GameObject> decoValues;
    [SerializeField] private List<string> audioKeys;
    [SerializeField] private List<AudioClip> audioValues;
    [SerializeField] private List<string> copyrightAudioKeys;
    [SerializeField] private List<AudioClip> copyrightAudioValues;
    [SerializeField] private List<string> wepKeys;
    [SerializeField] private List<GameObject> wepValues;
    [SerializeField] private List<Sprite> portraits;

    private static Dictionary<string, Sprite> imageDictionary;
    private static List<GameObject> modelDictionary;
    private static Dictionary<string, GameObject> decoDictionary;
    private static Dictionary<string, AudioClip> audioDictionary;
    private static Dictionary<string, AudioClip> copyrightAudioDictionary;
    private static Dictionary<string, GameObject> weaponDictionary;
    private static Dictionary<string, Sprite> portraitDictionary;

    public static string PORTRAIT_NEUTRAL = "_neutral";
    public static string PORTRAIT_HAPPY = "_happy";
    public static string PORTRAIT_NERVOUS = "_nervous";
    public static string PORTRAIT_DARING = "_daring";
    public static string PORTRAIT_SAD = "_sad";
    public static string PORTRAIT_ANGRY = "_angry";
    public static string PORTRAIT_CONFUSED = "_confused";
    public static string PORTRAIT_TEASE = "_tease";
    public static string PORTRAIT_SKEPTIC = "_skeptic";

    private void Awake()
    {
        imageDictionary = new Dictionary<string, Sprite>();
        for (int q = 0; q < imageKeys.Count; q++)
        {
            imageDictionary.Add(imageKeys[q].Replace(".jpg", "").Replace(".png", ""), imageValues[q]);
        }
        modelDictionary = modelValues;
        decoDictionary = new Dictionary<string, GameObject>();
        for (int q = 0; q < decoKeys.Count; q++)
        {
            decoDictionary.Add(decoKeys[q], decoValues[q]);
        }
        audioDictionary = new Dictionary<string, AudioClip>();
        for (int q = 0; q < audioKeys.Count; q++)
        {
            audioDictionary.Add(audioKeys[q], audioValues[q]);
        }
        copyrightAudioDictionary = new Dictionary<string, AudioClip>();
        for (int q = 0; q < copyrightAudioKeys.Count; q++)
        {
            copyrightAudioDictionary.Add(copyrightAudioKeys[q], copyrightAudioValues[q]);
        }
        weaponDictionary = new Dictionary<string, GameObject>();
        for (int q = 0; q < wepKeys.Count; q++)
        {
            weaponDictionary.Add(wepKeys[q], wepValues[q]);
        }
        portraitDictionary = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in portraits)
        {
            portraitDictionary.Add(sprite.name.Replace(".png", ""), sprite);
        }
    }

    public static Sprite getImage(string key)
    {
        return imageDictionary[key];
    }
    public static GameObject getModel(int key)
    {
        return modelDictionary[key];
    }
    public static GameObject getDeco(string key)
    {
        return decoDictionary[key];
    }
    public static AudioClip getAudio(string key)
    {
        if ((StaticData.copyrightMusic && copyrightAudioDictionary.ContainsKey(key))
            || !audioDictionary.ContainsKey(key))
        {
            return copyrightAudioDictionary[key];
        }
        return audioDictionary[key];
    }
    public static GameObject getWeapon(string key)
    {
        return weaponDictionary[key];
    }
    public static Sprite getPortrait(string unitName)
    {
        return getPortrait(unitName, PORTRAIT_NEUTRAL);
    }
    public static Sprite getPortrait(string unitName, string expression)
    {
        return portraitDictionary[unitName.ToLower().Replace(' ', '_') + expression];
    }
}

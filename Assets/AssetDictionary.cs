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
    [SerializeField] private List<string> wepKeys;
    [SerializeField] private List<GameObject> wepValues;

    private static Dictionary<string, Sprite> imageDictionary;
    private static List<GameObject> modelDictionary;
    private static Dictionary<string, GameObject> decoDictionary;
    private static Dictionary<string, AudioClip> audioDictionary;
    private static Dictionary<string, GameObject> weaponDictionary;

    private void Start()
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
        weaponDictionary = new Dictionary<string, GameObject>();
        for (int q = 0; q < wepKeys.Count; q++)
        {
            weaponDictionary.Add(wepKeys[q], wepValues[q]);
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
        return audioDictionary[key];
    }
    public static GameObject getWeapon(string key)
    {
        return weaponDictionary[key];
    }
}

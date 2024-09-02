using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetDictionary : MonoBehaviour
{
    [SerializeField] private List<string> imageKeys;
    [SerializeField] private List<Sprite> imageValues;
    [SerializeField] private List<string> modelKeys;
    [SerializeField] private List<GameObject> modelValues;
    [SerializeField] private List<string> decoKeys;
    [SerializeField] private List<GameObject> decoValues;

    private static Dictionary<string, Sprite> imageDictionary;
    private static Dictionary<string, GameObject> modelDictionary;
    private static Dictionary<string, GameObject> decoDictionary;

    private void Start()
    {
        imageDictionary = new Dictionary<string, Sprite>();
        for (int q = 0; q < imageKeys.Count; q++)
        {
            imageDictionary.Add(imageKeys[q], imageValues[q]);
        }
        modelDictionary = new Dictionary<string, GameObject>();
        for (int q = 0; q < modelKeys.Count; q++)
        {
            modelDictionary.Add(modelKeys[q], modelValues[q]);
        }
        decoDictionary = new Dictionary<string, GameObject>();
        for (int q = 0; q < decoKeys.Count; q++)
        {
            decoDictionary.Add(decoKeys[q], decoValues[q]);
        }
    }

    public static Sprite getImage(string key)
    {
        return imageDictionary[key];
    }
    public static GameObject getModel(string key)
    {
        return modelDictionary[key];
    }
    public static GameObject getDeco(string key)
    {
        return decoDictionary[key];
    }
}

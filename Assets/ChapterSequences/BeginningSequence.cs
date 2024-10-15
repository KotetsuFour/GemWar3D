using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningSequence : Chapter
{
    [SerializeField] private MainMenu menu; //Prefab
    [SerializeField] private Cutscene openingScene; //Object in scene
    [SerializeField] private bool useCopyrightMusic;

    public int sequenceNum;
    public SequenceMember seqMem;

    public List<Unit> playerList;

    void Start()
    {
        StaticData.copyrightMusic = useCopyrightMusic;

        openingScene.gameObject.SetActive(true);
        openingScene.constructor(openingDialogue());
        seqMem = openingScene;
    }
    // Update is called once per frame
    void Update()
    {
        handleInput(seqMem);
        if (seqMem.completed())
        {
            sequenceNum++;
            if (sequenceNum == 1)
            {
                seqMem.gameObject.SetActive(false);
                seqMem = Instantiate(menu);
            }
        }
    }

    public string[] openingDialogue()
    {
        string rose = "Rose_Quartz null ";
        string pearl = "Pearl Pearl ";
        return new string[]
        {
            "$solidColor 0 0 0",
            "$sound rose-and-pearl-music start",
            "$sound rose-and-pearl-music loop",
            rose + "Pearl...",
            "$left Pearl",
            pearl + "Yes?",
            rose + "I'm going to stay and fight for this planet. You don't have to do this with me.",
            "$left Pearl " + AssetDictionary.PORTRAIT_NERVOUS,
            pearl + "But I want to!",
            rose + "I know you do.Please, please understand. If we lose, we'll be killed. And if we win, we can never go home.",
            pearl + "Why would I ever want to go home if you're here?",
            "$left Pearl",
            rose + "*laughs * My Pearl...",
            "$left Pearl " + AssetDictionary.PORTRAIT_HAPPY,
            pearl + "You're wonderful...",
            "$right Pink_Diamond",
            pearl + "My Diamond"
        };
    }
}

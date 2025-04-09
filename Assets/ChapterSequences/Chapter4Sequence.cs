using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter4Sequence : Chapter
{
    [SerializeField] private PreBattleMenu preparationsPrefab; //prefab
    [SerializeField] private GridMap gridmap; //object
    [SerializeField] private Cutscene introScene; //object
    [SerializeField] private Cutscene finalScene; //object
    [SerializeField] private SaveScreen saveScreenPrefab; //prefab
    [SerializeField] private ChapterTitle chapterTitlePrefab; //prefab

    private int sequenceNum;
    private SequenceMember seqMem;

    private List<Unit> playerList;

    public Unit[] player;
    public Unit[] enemy;
    public Unit[] ally;
    public Unit[] other;

    public static string CHAPTER_TITLE = "Chapter 3 - The Answer";
    public static int TURNPAR = 15;

    [SerializeField] private Material floor;
    [SerializeField] private Material grass;
    [SerializeField] private Material mountain;
    [SerializeField] private Material peak;
    [SerializeField] private Material cave;
    [SerializeField] private Material cliff;
    [SerializeField] private Material heal;

    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject house;
    [SerializeField] private GameObject village;

    [SerializeField] private Color quartzHair;
    [SerializeField] private Color quartzSkin;
    [SerializeField] private Color quartzEyes;
    [SerializeField] private Color quartzPalette4;
    [SerializeField] private Color quartzPalette5;
    [SerializeField] private Color quartzPalette6;
    [SerializeField] private Color quartzPalette7;


    // Start is called before the first frame update
    void Start()
    {
        
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
                introScene.gameObject.SetActive(true);
                introScene.constructor(getIntro());
                seqMem = introScene;
            }
            else if (sequenceNum == 2)
            {
                seqMem.gameObject.SetActive(false);
                Debug.Log("before prebattle");
                PreBattleMenu pbm = makePrepMenu();
                Debug.Log("after prebattle");
                seqMem = pbm;
            }
            else if (sequenceNum == 3)
            {
                seqMem.gameObject.SetActive(false);
                seqMem = makeChapter();
                gridmap.gameObject.SetActive(true);
            }
            else if (sequenceNum == 4)
            {
                seqMem.gameObject.SetActive(false);
                playerList = gridmap.player;
                turnsTaken = gridmap.turn;
                finalScene.gameObject.SetActive(true);
                finalScene.constructor(getEnding());
                seqMem = finalScene;
            }
            else if (sequenceNum == 5)
            {
                seqMem.gameObject.SetActive(false);
                finalize(playerList);
                SaveScreen save = Instantiate(saveScreenPrefab);
                seqMem = save;
            }
            else if (sequenceNum == 6)
            {
                goToChapter(-5);
            }
        }
    }

    public string[] getIntro()
    {
        string pink = "Pink_Diamond Pink_Diamond ";
        string blue = "Blue_Diamond Blue_Diamond ";
        string serp = "Serpentine Serpentine ";
        string rose = "Rose_Quartz Rose_Quartz";
//        string pearl = "Pearl Pearl ";
        string bismuth = "Bismuth Bismuth";
        string garnet = "Garnet Garnet";

        return new string[]
        {
            "$image moonbase",
            "$sound diamond-music start",
            "$sound diamond-music loop",
            "$right Pink_Diamond " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$left Serpentine " + AssetDictionary.PORTRAIT_NEUTRAL,
            serp + "Progress on the Ziggurat and Communication Hub is proceeding on schedule.",
            serp + "Furthermore, Pink Diamond's Heaven and Earth Beetles have settled into their respective dwellings.",
            serp + "The Bismuths are still having trouble finishing the arenas due to rebel attacks.",
            serp + "However, this is only a minor setback.",
            "$right Blue_Diamond " + AssetDictionary.PORTRAIT_NEUTRAL,
            blue + "Thank you for the report, Serpentine. That will be all.",
            "$left Serpentine " + AssetDictionary.PORTRAIT_NERVOUS,
            serp + "Well actually, my Diamonds, I was hoping to tell you about some designs I had for the Communication Hub.",
            serp + "As I've been overseeing the construction, I couldn't help but notice how insecure our network is.",
            serp + "It'd be a shame if the rebels figured out how to hijack our communication systems.",
            "$left Serpentine " + AssetDictionary.PORTRAIT_HAPPY,
            blue + "Thank you, Serpentine. Your prudence is appreciated.",
            "$left Serpentine " + AssetDictionary.PORTRAIT_SAD,
            blue + "But if there were any technical problems with the Communication Hub, a Peridot would tell us.",
            blue + "That will be all.",
            "$left Pink_Diamond " + AssetDictionary.PORTRAIT_NEUTRAL,
            blue + "Now, on to more pressing matters. Pink, have you selected a Morganite to hunt down the rebels?",
            pink + "Yes. She's currently investigating Facet 8.",
            blue + "Facet 8? There have been no rebel sightings in Facet 8.",
            "$left Pink_Diamond " + AssetDictionary.PORTRAIT_NERVOUS,
            pink + "Exactly. They wouldn't hide out in places where they know we've seen them, right?",
            "$right Blue_Diamond " + AssetDictionary.PORTRAIT_HAPPY,
            blue + "Hmm.",
            blue + "Very well then. As long as we don't lose any further progress on construction.",
            "$image moonbase",
            "$sound before-chapter-music2 start",
            "$sound before-chapter-music2 loop",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            "$left Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "We're going to the Communication Hub construction site.",
            "$left Bismuth " + AssetDictionary.PORTRAIT_DARING,
            bismuth + "Are we gonna tear it down?",
            rose + "No, even better. We're going to hijack it so we can send our own messages through Homeworld's system.",
            "$left Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "Could that work?",
            "$left Garnet " + AssetDictionary.PORTRAIT_NEUTRAL,
            garnet + "I think it will.",
            "$left Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            bismuth + "Oh yeah, I forgot we have a Sapphire now to tell us the future!",
            "$left Garnet " + AssetDictionary.PORTRAIT_NEUTRAL,
            garnet + "Well, I'm not just a Sapphire. My future vision is different now. But yes, I can see it.",
            garnet + "Homeworld's network is insecure. It will be easy to overload it.",
            garnet + "As long as we can get Rose to where she needs to be.",
            rose + "Let's do it!",
        };
    }
    public string[] recruitSerpentine()
    {
        return new string[]
        {

        };
    }
    public string[] recruitTigersEye()
    {
        return new string[]
        {

        };
    }
    public string[] getEnding()
    {
        return new string[]
        {

        };
    }

    private PreBattleMenu makePrepMenu()
    {
        if (StaticData.chapterPrep != StaticData.scene)
        {
            /*
            Unit neph_unit = nephrite();
            StaticData.members.Add(neph_unit);
            StaticData.getUnitByName("Rose Quartz").fusionSkillBonus = Unit.FusionSkill.HEALING;

            StaticData.positions = new int[] { 0, -1, -1, -1, -1, -1, -1, -1, -1 };

            StaticData.chapterPrep = StaticData.scene;
            */
        }
        /*
        enemy = new Unit[]{genericTopazFusion(), genericPriestess(), genericQuartz(), genericGuard(),
                    genericPriestess(), genericElite(), genericGuard(), genericGuard(), genericQuartz(),
                    genericQuartz(), genericQuartz(), genericQuartz(), genericGuard(), genericGuard(),
                    genericGuard(), genericGuard(), genericGuard(), genericQuartz(), genericQuartz(),
                    genericQuartz()};
        ally = new Unit[] { citrine(), flint(), aventurine(), chert() };
        */
        other = new Unit[] { };

        string[] teamNames = { "Crystal Gems", "Homeworld", "", "" };

        PreBattleMenu ret = Instantiate(preparationsPrefab);

        Tile[,] map = createMap(StaticData.findDeepChild(ret.transform, "MapTransform"));
        List<Tile> playerTiles = getPlayerDeploymentTiles(map);
        ret.constructor(map, playerTiles,
            enemy, ally, other, Quaternion.identity,
            new RoutObjective(), CHAPTER_TITLE, teamNames, TURNPAR, "prep-music");

        setUnits(map, enemy, Unit.UnitTeam.ENEMY, Quaternion.Euler(0, 180, 0));
        setUnits(map, ally, Unit.UnitTeam.ALLY, Quaternion.identity);

        ret.initializeCursorPosition();

        return ret;
    }

    private GridMap makeChapter()
    {
        playerList = new List<Unit>();
        player = new Unit[StaticData.positions.Length];
        for (int q = 0; q < StaticData.positions.Length; q++)
        {
            if (StaticData.positions[q] == -1)
            {
                player[q] = null;
            }
            else
            {
                player[q] = StaticData.members[StaticData.positions[q]];
                playerList.Add(player[q]);
            }
        }
        /*
        enemy = new Unit[]{ genericTopazFusion(), genericPriestess(), genericQuartz(), genericGuard(),
                    genericPriestess(), genericElite(), genericGuard(), genericGuard(), genericQuartz(),
                    genericQuartz(), genericQuartz(), genericQuartz(), genericGuard(), genericGuard(),
                    genericGuard(), genericGuard(), genericGuard(), genericQuartz(), genericQuartz(),
                    genericQuartz() };
        ally = new Unit[] { citrine(), flint(), aventurine(), chert() };
        */
        other = new Unit[] { };
        string[] teamNames = { "Crystal Gems", "Homeworld", "Refraction Stones", "" };

        gridmap.gameObject.SetActive(true);
        Tile[,] map = createMap(StaticData.findDeepChild(gridmap.transform, "MapTransform"));
        gridmap.constructor(map,
            playerList.ToArray(), enemy, ally, other,
            new RoutObjective(), CHAPTER_TITLE, teamNames, TURNPAR,
            new string[] { "map-music-1", "enemyphase-music-1", "allyphase-music-1", "otherphase-music-1" },
            new string[] { "player-battle-music-1", "enemy-battle-music-1", "ally-battle-music-1", "other-battle-music-1" });
        gridmap.combatBackground = surroundings[1];

        setUnits(map, player, Unit.UnitTeam.PLAYER, Quaternion.identity);
        setUnits(map, enemy, Unit.UnitTeam.ENEMY, Quaternion.Euler(0, 180, 0));
        setUnits(map, ally, Unit.UnitTeam.ALLY, Quaternion.identity);

        gridmap.initializeCursorPosition();

        return gridmap;
    }

}

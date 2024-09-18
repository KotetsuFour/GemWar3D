using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter1Sequence : Chapter
{
    [SerializeField] private GridMap gridmap; //object
    [SerializeField] private Cutscene firstScene; //object
    [SerializeField] private Cutscene introScene; //object
    [SerializeField] private Cutscene finalScene; //object
    [SerializeField] private SaveScreen saveScreen; //prefab
    [SerializeField] private ChapterTitle chapterTitle; //prefab

    [SerializeField] private Material floor;
    [SerializeField] private Material rubble;

    [SerializeField] private GameObject pillar;
    [SerializeField] private GameObject warp_pad;
//    [SerializeField] private GameObject rubble;


    private int sequenceNum;
    private SequenceMember seqMem;

    private List<Unit> playerList;

    // Start is called before the first frame update
    void Start()
    {
        ChapterTitle title = Instantiate(chapterTitle);
        title.constructor("Chapter 1 - Rebellion");
        seqMem = title;

        materialDictionary = new Dictionary<char, Material>();
        materialDictionary.Add('_', floor);
        materialDictionary.Add('|', floor);
        materialDictionary.Add('R', rubble);
        materialDictionary.Add('r', floor);

        decoDictionary = new Dictionary<char, GameObject>();
        decoDictionary.Add('|', pillar);
        decoDictionary.Add('r', warp_pad);
//        decoDictionary.Add('R', rubble);

        tileMap = new string[] {
        "____________",
        "____________",
        "_|________|_",
        "________RR__",
        "__R______R__",
        "_RRR________",
        "_|____RR__|_",
        "______R_____",
        "__R______RR_",
        "_RR_______RR",
        "_|R_______|_",
        "_____rr_____"
        };
        deployMap = new string[] {
        "____________",
        "_____**_____",
        "____________",
        "_x__________",
        "____________",
        "____________",
        "x________x__",
        "__x_________",
        "____________",
        "x__x____x___",
        "____xxx_____",
        "____________"
        };
        lootMap = new string[] {
        "____________",
        "____________",
        "_|________|_",
        "________RR__",
        "__R______R__",
        "_RRR________",
        "_|____RR__|_",
        "______R_____",
        "__R______RR_",
        "_RR_______RR",
        "_|R_______|_",
        "_____rr_____"
        };
        heightMap = new string[] {
        "000000000000",
        "000000000000",
        "000000000000",
        "000000001100",
        "001000000100",
        "011100000000",
        "000000110000",
        "000000100000",
        "001000000110",
        "011000000011",
        "001000000000",
        "000001100000"
        };
        decoMap = new string[] {
        "____________",
        "____________",
        "_|________|_",
        "________RR__",
        "__R______R__",
        "_RRR________",
        "_|____RR__|_",
        "______R_____",
        "__R______RR_",
        "_RR_______RR",
        "_|R_______|_",
        "_____rr_____"
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            seqMem.LEFT_MOUSE();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            seqMem.RIGHT_MOUSE();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            seqMem.UP();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            seqMem.DOWN();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            seqMem.Z();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            seqMem.X();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            seqMem.A();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            seqMem.S();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            seqMem.ENTER();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            seqMem.ESCAPE();
        }

        if (seqMem.completed())
        {
            sequenceNum++;
            if (sequenceNum == 1)
            {
/*
                seqMem.gameObject.SetActive(false);
                firstScene.gameObject.SetActive(true);
                firstScene.constructor(opening());
                seqMem = firstScene;
            }
            else if (sequenceNum == 2)
            {
                seqMem.gameObject.SetActive(false);
                introScene.gameObject.SetActive(true);
                introScene.constructor(intro());

                seqMem = introScene;
            }
            else if (sequenceNum == 3)
            {
*/
                seqMem.gameObject.SetActive(false);
                seqMem = makeChapter();
            }
            else if (sequenceNum == 4)
            {
                playerList = gridmap.player;
                turnsTaken = gridmap.turn;

                seqMem.gameObject.SetActive(false);
                finalScene.gameObject.SetActive(true);
                finalScene.constructor(ending());

                seqMem = finalScene;
            }
            else if (sequenceNum == 5)
            {
                seqMem.gameObject.SetActive(false);

                finalize(playerList);

                StaticData.members.Add(makeBismuthRecruit());

                SaveScreen save = Instantiate(saveScreen);
                seqMem = save;
            }
            else if (sequenceNum == 6)
            {
                SceneManager.LoadScene("Chapter" + StaticData.scene);
            }
        }
    }

    public string[] opening()
    {
        return new string[]
        {

        };
    }
    public string[] intro()
    {
        return new string[]
        {

        };
    }
    public string[] biggsRecruitment()
    {
        return new string[]
        {

        };
    }
    public string[] oceanRecruitment()
    {
        return new string[]
        {

        };
    }
    public string[] bismuthTalk()
    {
        return new string[]
        {

        };
    }
    public string[] ending()
    {
        return new string[]
        {

        };
    }

    public Unit makeBismuthRecruit()
    {
        string bismuth_desc = "An architect given to Pink Diamond for the Earth colony";
        Unit bismuth = new Unit();
        bismuth.constructor("Bismuth", UnitClass.architect, bismuth_desc,
            29, 9, 0, 3, 2, 0, 11, 0, 15, 6,
            60, 50, 5, 40, 30, 25, 30, 5,
                Item.bismuth_hammer.clone(), Weapon.WeaponType.ARMOR, 0, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.ANIMA, new float[] { /*TODO*/ });
        bismuth.isEssential = true;

        return bismuth;
    }

    public Unit makeRoseQuartz()
    {
        //Based on Fergus (FE5)
        //10% STR growth buff
        //1 point DEF buff
        string rose_desc = "The leader of the Crystal Gem rebellion";
        Unit rose_quartz = new Unit();
        rose_quartz.constructor("Rose Quartz", UnitClass.lord, rose_desc,
                26, 6, 5, 7, 7, 6, 6, 5, 8, 6,
                65, 45, 10, 45, 35, 40, 25, 10,
                Item.rose_shield.clone(), Weapon.WeaponType.SWORD, 0, Unit.UnitTeam.PLAYER, 0, 1,
                Unit.Affinity.EARTH, new float[] { });
        rose_quartz.isEssential = true;
        rose_quartz.isLeader = true;

        return rose_quartz;
    }
    public Unit makePearl()
    {
        //Based on Machua (FE5)
        //2 point SPD nerf
        //1 point DEF buff
        string pearl_desc = "Rose Quartz's loyal companion";
        Unit pearl = new Unit();
        pearl.constructor("Pearl", UnitClass.servant, pearl_desc,
                22, 4, 1, 10, 9, 6, 5, 1, 6, 6,
                60, 30, 10, 55, 60, 35, 25, 10,
                Item.pearl_spear.clone(), Weapon.WeaponType.SWORD, 0, Unit.UnitTeam.PLAYER, 0, 2,
                Unit.Affinity.WATER, new float[] { });
        pearl.isEssential = true;

        return pearl;
    }

    public Unit makeBiggs()
    {
        //Based on Halvan (FE5)
        string biggs_desc = "A soldier fighting in the war";
        Unit biggs = new Unit();
        biggs.constructor("Biggs", UnitClass.soldier, biggs_desc,
                28, 7, 0, 7, 7, 2, 5, 0, 12, 6,
                80, 40, 5, 20, 30, 30, 30, 5,
                Item.biggs_whip.clone(), Weapon.WeaponType.CLUB, 20, Unit.UnitTeam.ENEMY, 9, -1,
                Unit.Affinity.FIRE, new float[] { });
        biggs.setTalkConvo(biggsRecruitment(), true, null);
        biggs.ai1 = Unit.AIType.ATTACK; biggs.ai2 = Unit.AIType.GUARD;

        return biggs;
    }

    public Unit makeOcean()
    {
        //Based on Othin (FE5)
        string ocean_desc = "A soldier fighting in the war";
        Unit ocean = new Unit();
        ocean.constructor("Ocean", UnitClass.soldier, ocean_desc,
                27, 6, 0, 7, 9, 3, 4, 0, 11, 6,
                85, 30, 5, 25, 35, 55, 25, 5,
                Item.ocean_club.clone(), Weapon.WeaponType.WHIP, 20, Unit.UnitTeam.ENEMY, 9, -1,
                Unit.Affinity.WATER, new float[] { });
        ocean.setTalkConvo(oceanRecruitment(), false, null);
        ocean.ai1 = Unit.AIType.ATTACK; ocean.ai2 = Unit.AIType.GUARD;

        return ocean;
    }

    public Unit makeBismuthGeneric()
    {
        //Based on Dashin (FE5)
        //5% SPD growth buff and 10% DEF growth buff
        string bismuth_desc = "An architect given to Pink Diamond for the Earth colony";
        Unit bismuth = new Unit();
        bismuth.constructor("Bismuth", UnitClass.architect, bismuth_desc,
                29, 9, 0, 3, 2, 0, 11, 0, 15, 6,
                60, 50, 5, 40, 30, 25, 30, 5,
                Item.bismuth_hammer.clone(), Weapon.WeaponType.ARMOR, 0, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.ANIMA, new float[] { });
        bismuth.isEssential = true;
        bismuth.setTalkConvo(bismuthTalk(), true, Item.iron_sword);
        bismuth.ai1 = Unit.AIType.IDLE; bismuth.ai2 = Unit.AIType.IDLE;

        return bismuth;
    }
    private Unit genericQuartz()
    {
        //2 point STR nerf
        string quartz_desc = "Soldier serving the diamonds against the rebellion";
        Unit quartz = new Unit();
        quartz.constructor("Quartz", UnitClass.soldier, quartz_desc,
                24, 6, 0, 3, 6, 2, 4, 0, 9, 6,
                80, 40, 5, 20, 30, 30, 30, 5,
                Item.quartz_axe, Weapon.WeaponType.FIST, 20, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.FIRE, new float[] { });
        quartz.ai1 = Unit.AIType.ATTACK; quartz.ai2 = Unit.AIType.GUARD;

        return quartz;
    }

    public GridMap makeChapter()
    {
        Unit rose_quartz = makeRoseQuartz();
        Unit pearl = makePearl();
        StaticData.members.Add(rose_quartz);
        StaticData.members.Add(pearl);
        Unit[] player = { rose_quartz, pearl };
        Unit[] enemy = {makeBismuthGeneric(), genericQuartz(), genericQuartz(),
            genericQuartz(), genericQuartz(), genericQuartz(),
            genericQuartz(), makeBiggs(), makeOcean(), genericQuartz()};
        Unit[] ally = { };
        Unit[] other = { };
        string[] teamNames = { "Rose's Rebels", "Homeworld", "", "" };

        gridmap.gameObject.SetActive(true);
        Tile[,] mapArray = createMap(StaticData.findDeepChild(gridmap.transform, "MapTransform"));
        gridmap.constructor(mapArray, player, enemy, ally, other,
            new EscapeObjective(), "Chapter 1 - Rebellion", teamNames, 10);
        playerList = gridmap.player;

        setUnits(mapArray, enemy, Unit.UnitTeam.ENEMY);
        setUnits(mapArray, player, Unit.UnitTeam.PLAYER);

        return gridmap;

    }
}

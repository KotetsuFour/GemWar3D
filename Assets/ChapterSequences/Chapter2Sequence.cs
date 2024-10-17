using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2Sequence : Chapter
{
    public Tile tile;

    [SerializeField] private Sprite lunar_sea_spire_floor;
    [SerializeField] private Sprite lunar_sea_spire_pillar;
    [SerializeField] private Sprite deep_water;
    [SerializeField] private Sprite lunar_sea_spire_wall;
    [SerializeField] private Sprite lunar_sea_spire_chest;
    [SerializeField] private Sprite lunar_sea_spire_throne;
    [SerializeField] private Sprite heal_tile;


    [SerializeField] private PreBattleMenu preparationsPrefab; //prefab
    [SerializeField] private GridMap gridmap; //object
    [SerializeField] private Cutscene firstScene; //object
    [SerializeField] private Cutscene introScene; //object
    [SerializeField] private Cutscene finalScene; //object
    [SerializeField] private SaveScreen saveScreenPrefab; //prefab
    [SerializeField] private ChapterTitle chapterTitlePrefab; //prefab
    [SerializeField] private Sprite battleBackPicture;

    private int sequenceNum;
    private SequenceMember seqMem;

    private List<Unit> playerList;

    private int turnsTaken;

    public Unit[] player;
    public Unit[] enemy;
    public Unit[] ally;
    public Unit[] other;

    public static string CHAPTER_TITLE = "Chapter 2 - Sea of Troubles";
    public static int TURNPAR = 20;


    // Start is called before the first frame update
    void Start()
    {
        firstScene.constructor(getOpening());
        seqMem = firstScene;
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
                ChapterTitle title = Instantiate(chapterTitlePrefab);
                title.constructor(CHAPTER_TITLE);
                seqMem = title;
            }
            else if (sequenceNum == 2)
            {
                seqMem.gameObject.SetActive(false);
                introScene.constructor(getMapIntro());
                seqMem = introScene;
            }
            else if (sequenceNum == 3)
            {
                seqMem.gameObject.SetActive(false);
                SaveMechanism.loadGame(StaticData.savefile);

                PreBattleMenu pbm = makePrepMenu();

                seqMem = pbm;
            }
            else if (sequenceNum == 4)
            {
                seqMem.gameObject.SetActive(false);
                seqMem = makeChapter();
            }
            else if (sequenceNum == 5)
            {
                seqMem.gameObject.SetActive(false);
                turnsTaken = ((GridMap)seqMem).turn;
                finalScene.constructor(getEnding());
                seqMem = finalScene;
            }
            else if (sequenceNum == 6)
            {
                StaticData.dealWithGemstones();
                StaticData.refreshUnits();
                StaticData.registerRemainingSupports(playerList, turnsTaken);
                StaticData.scene++;
                SaveScreen save = Instantiate(saveScreenPrefab);
                seqMem = save;
            }
            else if (sequenceNum == 7)
            {
                goToChapter(3);
            }
        }
    }

    private string[] getOpening()
    {
        return new string[]
        {

        };
    }
    private string[] getMapIntro()
    {
        return new string[]
        {

        };
    }
    private string[] turquoiseRecruitment()
    {
        return new string[]
        {

        };
    }
    private string[] moonstoneRecruitment()
    {
        return new string[]
        {

        };
    }
    private string[] getEnding()
    {
        return new string[]
        {

        };
    }

    private Unit ruby()
    {
        //Based on FE5 Lifis
        string ruby_desc = "A stalwart guardian for Turquoise";
        Unit ruby_unit = new Unit();
        ruby_unit.constructor("Ruby", UnitClass.guard, ruby_desc,
                20, 3, 0, 4, 10, 1, 2, 0, 6, 5,
                65, 35, 10, 25, 45, 5, 15, 10,
                Item.ruby_pike.clone(), Weapon.WeaponType.WHIP, 0, Unit.UnitTeam.PLAYER, 6, -1,
                Unit.Affinity.FIRE, new float[]
                {
                    //TODO
                });
        return ruby_unit;
    }
    private Unit genericNoble()
    {
        //Based on FE3 Merric
        string noble_desc = "A noble in the court of a Diamond";
        Unit noble = new Unit();
        noble.constructor("Noble", UnitClass.diplomat, noble_desc,
                20, 0, 6, 3, 6, 3, 4, 3, 5, 6,
                80, 5, 20, 30, 50, 50, 20, 3,
                Item.palm_laser.clone(), Weapon.WeaponType.SWORD, 0, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.ICE, new float[]
                {

                });
        noble.ai1 = Unit.AIType.GUARD; noble.ai2 = Unit.AIType.GUARD;
        noble.movement = 0;
        return noble;
    }

    private Unit genericPriestess()
    {
        //Based on FE3 Gordon
        string priest_desc = "A priestess in the service of the Moon Goddess";
        Unit priest = new Unit();
        priest.constructor("Priestess", UnitClass.priestess, priest_desc,
                18, 5, 0, 5, 4, 4, 6, 0, 5, 6,
                40, 30, 3, 30, 30, 40, 10, 3,
                Item.priest_bow.clone(), Weapon.WeaponType.SWORD, 0, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.LIGHT, new float[]
                {

                });
        priest.ai1 = Unit.AIType.ATTACK; priest.ai2 = Unit.AIType.GUARD;
        return priest;
    }

    private Unit moonstone()
    {
        //Based on FE5 Tanya
        string moon_desc = "A priestess in the service of the Moon Goddess";
        Unit moon = new Unit();
        moon.constructor("Moonstone", UnitClass.priestess, moon_desc,
                20, 3, 1, 6, 10, 6, 2, 1, 4, 6,
                60, 35, 15, 55, 70, 60, 15, 15,
                Item.moon_bow.clone(), Weapon.WeaponType.LANCE, 0, Unit.UnitTeam.ENEMY, 10, -1,
                Unit.Affinity.LIGHT, new float[]
                {

                });
        moon.setTalkConvo(moonstoneRecruitment(), true, null);
        moon.ai1 = Unit.AIType.ATTACK; moon.ai2 = Unit.AIType.GUARD;
        return moon;
    }

    private Unit turquoise()
    {
        //Based on FE5 Asvel
        string turq_desc = "An emissary from Blue Diamond's court";
        Unit turq = new Unit();
        turq.constructor("Turquoise", UnitClass.diplomat, turq_desc,
                22, 0, 4, 3, 7, 5, 0, 4, 4, 6,
                55, 15, 35, 55, 75, 35, 10, 35,
                Item.palm_laser, Weapon.WeaponType.SPECIAL, 0, Unit.UnitTeam.ENEMY, 6, 10,
                Unit.Affinity.ICE, new float[]
                {

                });
        turq.setTalkConvo(turquoiseRecruitment(), false, null);
        turq.ai1 = Unit.AIType.GUARD; turq.ai2 = Unit.AIType.GUARD;
        return turq;
    }
    private PreBattleMenu makePrepMenu()
    {
        if (StaticData.chapterPrep != StaticData.scene)
        {
            Unit ruby_unit = ruby();
            StaticData.members.Add(ruby_unit);

            StaticData.addToConvoy(Item.iron_blade.clone());
        }

        //TODO everything to do
        int[] playerPos = { 0, -1, -1, -1, -1, -1 };
        if (StaticData.chapterPrep == StaticData.scene)
        {
            playerPos = StaticData.positions;
        }
        Sprite[] tileSprites = {lunar_sea_spire_floor, null, lunar_sea_spire_pillar, null, deep_water,
                    lunar_sea_spire_wall, lunar_sea_spire_chest, lunar_sea_spire_throne, heal_tile};
        enemy = new Unit[]{genericNoble(), genericPriestess(), genericPriestess(),
                    genericPriestess(), moonstone(), turquoise(), genericNoble(), genericNoble(),
                    genericNoble(), genericNoble(), genericNoble(),
                    genericNoble(), genericPriestess(), genericPriestess(),
                    genericPriestess(), genericPriestess()};
        ally = new Unit[] { };
        other = new Unit[] { };

        string[] teamNames = { "Crystal Gems", "Homeworld", "", "" };

        PreBattleMenu ret = Instantiate(preparationsPrefab);

        Tile[,] map = createMap(StaticData.findDeepChild(ret.transform, "MapTransform"));
        List<Tile> playerTiles = getPlayerDeploymentTiles(map);
        ret.constructor(map, playerTiles,
            enemy, ally, other, Quaternion.Euler(0, 180, 0),
            new SeizeObjective(), CHAPTER_TITLE, teamNames, TURNPAR, "prep-music");

        return ret;
    }
    private GridMap makeChapter()
    {
        Sprite[] tileSprites = {lunar_sea_spire_floor, null, lunar_sea_spire_pillar, null, deep_water,
        lunar_sea_spire_wall, lunar_sea_spire_chest, lunar_sea_spire_throne, heal_tile};

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

        int[] loot = {0, 2, 0, -1,/*2 Steel*/0, 0, 0, 11,/*Iron Lance*/
            0, 0, 0, 24,/*Moon Goddess Icon*/0, 0, 0, 15,/*Iron Whip*/4, 0, 0, -1/*4 Iron*/};

        string[] teamNames = { "Crystal Gems", "Homeworld", "", "" };
        
        Tile[,] map = createMap(StaticData.findDeepChild(gridmap.transform, "MapTransform"));

        gridmap.constructor(map,
            playerList.ToArray(), enemy, ally, other,
            new SeizeObjective(), CHAPTER_TITLE, teamNames, TURNPAR,
            new string[] {  },
            new string[] {  });

        /*
Tile[,] map,
        Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits, Unit[] otherUnits,
        Objective objective, string chapterName, string[] teamNames, int turnPar,
        string[] teamMusic, string[] battleMusic         */

        setUnits(map, player, Unit.UnitTeam.PLAYER, Quaternion.Euler(0, 180, 0));

        return gridmap;
    }

}

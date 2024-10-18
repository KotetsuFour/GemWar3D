using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2Sequence : Chapter
{
    [SerializeField] private PreBattleMenu preparationsPrefab; //prefab
    [SerializeField] private GridMap gridmap; //object
    [SerializeField] private Cutscene firstScene; //object
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

    public static string CHAPTER_TITLE = "Chapter 2 - Sea of Troubles";
    public static int TURNPAR = 20;

    [SerializeField] private Material floor;
    [SerializeField] private Material water;
    [SerializeField] private Material wall;
    [SerializeField] private Material heal;

    [SerializeField] private GameObject pillar;
    [SerializeField] private GameObject throne;
    [SerializeField] private GameObject chest;

    [SerializeField] private Color rubyHair;
    [SerializeField] private Color rubySkin;
    [SerializeField] private Color rubyEyes;
    [SerializeField] private Color rubyPalette4;
    [SerializeField] private Color rubyPalette5;
    [SerializeField] private Color rubyPalette6;
    [SerializeField] private Color rubyPalette7;

    [SerializeField] private Color nobleHair;
    [SerializeField] private Color nobleSkin;
    [SerializeField] private Color nobleEyes;
    [SerializeField] private Color noblePalette4;
    [SerializeField] private Color noblePalette5;
    [SerializeField] private Color noblePalette6;
    [SerializeField] private Color noblePalette7;

    [SerializeField] private Color priestHair;
    [SerializeField] private Color priestSkin;
    [SerializeField] private Color priestEyes;
    [SerializeField] private Color priestPalette4;
    [SerializeField] private Color priestPalette5;
    [SerializeField] private Color priestPalette6;
    [SerializeField] private Color priestPalette7;

    [SerializeField] private Color moonHair;
    [SerializeField] private Color moonSkin;
    [SerializeField] private Color moonEyes;
    [SerializeField] private Color moonPalette4;
    [SerializeField] private Color moonPalette5;
    [SerializeField] private Color moonPalette6;
    [SerializeField] private Color moonPalette7;

    [SerializeField] private Color turqHair;
    [SerializeField] private Color turqSkin;
    [SerializeField] private Color turqEyes;
    [SerializeField] private Color turqPalette4;
    [SerializeField] private Color turqPalette5;
    [SerializeField] private Color turqPalette6;
    [SerializeField] private Color turqPalette7;

    // Start is called before the first frame update
    void Start()
    {
        firstScene.gameObject.SetActive(true);
        firstScene.constructor(getOpening());
        seqMem = firstScene;

        materialDictionary = new Dictionary<char, Material>();
        materialDictionary.Add('_', floor);
        materialDictionary.Add('|', floor);
        materialDictionary.Add('~', water);
        materialDictionary.Add('T', floor);
        materialDictionary.Add('W', wall);
        materialDictionary.Add('e', floor);
        materialDictionary.Add('+', heal);

        decoDictionary = new Dictionary<char, GameObject>();
        decoDictionary.Add('|', pillar);
        decoDictionary.Add('T', throne);
        decoDictionary.Add('e', chest);

        tileMap = new string[]
        {
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~________T________~~~~~~~",
            "~~~~~____|____________|___~~~~~",
            "~~~~W_____________________W~~~~",
            "~~~_WW_________+_________WW_~~~",
            "~~~__WW_________________WW__~~~",
            "~~____WW_______________WW____~~",
            "~~__e__WW_____________WW__e__~~",
            "~~______W__|__________W______~~",
            "~_______W__________|__W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~___+_______W__+__W_______+___~",
            "~_______W___W_____W___W_______~",
            "~_______W___W__|__W___W_______~",
            "~_______W___W_|e|_W_|_W_______~",
            "~_______W___WWWWWWW___W_______~",
            "~_______W_____________W_______~",
            "~~______W_____________W______~~",
            "~~__e__WW_____________WW__e__~~",
            "~~____WW__|_________|__WW____~~",
            "~~~__WW_________________WW__~~~",
            "~~~_WW______|__+__|______WW_~~~",
            "~~~~W_____________________W~~~~",
            "~~~~~_____________________~~~~~",
            "~~~~~~~_________________~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
        };

        deployMap = new string[]
        {
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~________x________~~~~~~~",
            "~~~~~____x____________x___~~~~~",
            "~~~~W_____________________W~~~~",
            "~~~_WW____x______________WW_~~~",
            "~~~__WW____________x____WW__~~~",
            "~~____WW_______________WW____~~",
            "~~__e__WW_____________WW__e__~~",
            "~~______W__|__________W______~~",
            "~_______W_x________|__W_______~",
            "~___x___W___W_____W___W___x___~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W_x_W_______~",
            "~___________W_____W___________~",
            "~_______W___W_____W___W_______~",
            "~_______W___W__x__W___W_______~",
            "~_______W___W_xex_W_|_W_______~",
            "~___x___W___WWWWWWW___W___x___~",
            "~_______W_____________W_______~",
            "~~______W__x_______x__W______~~",
            "~~__e__WW_____________WW__e__~~",
            "~~____WW__|_________|__WW____~~",
            "~~~__WW_________________WW__~~~",
            "~~~_WW___________________WW_~~~",
            "~~~~W__________*__________W~~~~",
            "~~~~~_________*_*_________~~~~~",
            "~~~~~~~______*_*_*______~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
        };

        lootMap = new string[]
        {
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~________T________~~~~~~~",
            "~~~~~____|____________|___~~~~~",
            "~~~~W_____________________W~~~~",
            "~~~_WW___________________WW_~~~",
            "~~~__WW_________________WW__~~~",
            "~~____WW_______________WW____~~",
            "~~__e__WW_____________WW__e__~~",
            "~~______W__|__________W______~~",
            "~_______W__________|__W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~___________W_____W___________~",
            "~_______W___W_____W___W_______~",
            "~_______W___W__|__W___W_______~",
            "~_______W___W_|e|_W_|_W_______~",
            "~_______W___WWWWWWW___W_______~",
            "~_______W_____________W_______~",
            "~~______W_____________W______~~",
            "~~__e__WW_____________WW__e__~~",
            "~~____WW__|_________|__WW____~~",
            "~~~__WW_________________WW__~~~",
            "~~~_WW___________________WW_~~~",
            "~~~~W_____________________W~~~~",
            "~~~~~_____________________~~~~~",
            "~~~~~~~_________________~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
        };

        heightMap = new string[]
        {
            "0000000000000000000000000000000",
            "0000000000AAAAAAAAAAA0000000000",
            "0000000AAAAAAAABAAAAAAAA0000000",
            "00000AAAAAAAAAAAAAAAAAAAAA00000",
            "0000IAAAAAAAAAAAAAAAAAAAAAI0000",
            "000AIIAAAAAAAAABAAAAAAAAAIIA000",
            "000AAIIAAAAAAAAAAAAAAAAAIIAA000",
            "00AAAAIIAAAAAAAAAAAAAAAIIAAAA00",
            "00AAAAAIIAAAAAAAAAAAAAIIAAAAA00",
            "00AAAAAAIAAAAAAAAAAAAAIAAAAAA00",
            "0AAAAAAAIAAAAAAAAAAAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAABAAAAAAAIAABAAIAAAAAAABAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIAAAAAIAAAIAAAAAAA0",
            "0AAAAAAAIAAAIIIIIIIAAAIAAAAAAA0",
            "0AAAAAAAIAAAAAAAAAAAAAIAAAAAAA0",
            "00AAAAAAIAAAAAAAAAAAAAIAAAAAA00",
            "00AAAAAIIAAAAAAAAAAAAAIIAAAAA00",
            "00AAAAIIAAAAAAAAAAAAAAAIIAAAA00",
            "000AAIIAAAAAAAAAAAAAAAAAIIAA000",
            "000AIIAAAAAAAAABAAAAAAAAAIIA000",
            "0000IAAAAAAAAAAAAAAAAAAAAAI0000",
            "00000AAAAAAAAAAAAAAAAAAAAA00000",
            "0000000AAAAAAAAAAAAAAAAA0000000",
            "0000000000AAAAAAAAAAA0000000000",
            "0000000000000000000000000000000"
        };

        decoMap = new string[]
        {
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~________T________~~~~~~~",
            "~~~~~____|____________|___~~~~~",
            "~~~~W_____________________W~~~~",
            "~~~_WW_________+_________WW_~~~",
            "~~~__WW_________________WW__~~~",
            "~~____WW_______________WW____~~",
            "~~__e__WW_____________WW__e__~~",
            "~~______W__|__________W______~~",
            "~_______W__________|__W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~_______W___W_____W___W_______~",
            "~___+_______W__+__W_______+___~",
            "~_______W___W_____W___W_______~",
            "~_______W___W__|__W___W_______~",
            "~_______W___W_|e|_W_|_W_______~",
            "~_______W___WWWWWWW___W_______~",
            "~_______W_____________W_______~",
            "~~______W_____________W______~~",
            "~~__e__WW_____________WW__e__~~",
            "~~____WW__|_________|__WW____~~",
            "~~~__WW_________________WW__~~~",
            "~~~_WW______|__+__|______WW_~~~",
            "~~~~W_____________________W~~~~",
            "~~~~~_____________________~~~~~",
            "~~~~~~~_________________~~~~~~~",
            "~~~~~~~~~~___________~~~~~~~~~~",
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
        };
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
                playerList = gridmap.player;
                turnsTaken = gridmap.turn;
                finalScene.constructor(getEnding());
                seqMem = finalScene;
            }
            else if (sequenceNum == 6)
            {
                finalize(playerList);
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
                    rubyHair.r, rubyHair.g, rubyHair.b,
                    rubySkin.r, rubySkin.g, rubySkin.b,
                    rubyEyes.r, rubyEyes.g, rubyEyes.b,
                    rubyPalette4.r, rubyPalette4.g, rubyPalette4.b,
                    rubyPalette5.r, rubyPalette5.g, rubyPalette5.b,
                    rubyPalette6.r, rubyPalette6.g, rubyPalette6.b,
                    rubyPalette7.r, rubyPalette7.g, rubyPalette7.b,
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
                    nobleHair.r, nobleHair.g, nobleHair.b,
                    nobleSkin.r, nobleSkin.g, nobleSkin.b,
                    nobleEyes.r, nobleEyes.g, nobleEyes.b,
                    noblePalette4.r, noblePalette4.g, noblePalette4.b,
                    noblePalette5.r, noblePalette5.g, noblePalette5.b,
                    noblePalette6.r, noblePalette6.g, noblePalette6.b,
                    noblePalette7.r, noblePalette7.g, noblePalette7.b,
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
                    priestHair.r, priestHair.g, priestHair.b,
                    priestSkin.r, priestSkin.g, priestSkin.b,
                    priestEyes.r, priestEyes.g, priestEyes.b,
                    priestPalette4.r, priestPalette4.g, priestPalette4.b,
                    priestPalette5.r, priestPalette5.g, priestPalette5.b,
                    priestPalette6.r, priestPalette6.g, priestPalette6.b,
                    priestPalette7.r, priestPalette7.g, priestPalette7.b,
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
                    moonHair.r, moonHair.g, moonHair.b,
                    moonSkin.r, moonSkin.g, moonSkin.b,
                    moonEyes.r, moonEyes.g, moonEyes.b,
                    moonPalette4.r, moonPalette4.g, moonPalette4.b,
                    moonPalette5.r, moonPalette5.g, moonPalette5.b,
                    moonPalette6.r, moonPalette6.g, moonPalette6.b,
                    moonPalette7.r, moonPalette7.g, moonPalette7.b,
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
                    turqHair.r, turqHair.g, turqHair.b,
                    turqSkin.r, turqSkin.g, turqSkin.b,
                    turqEyes.r, turqEyes.g, turqEyes.b,
                    turqPalette4.r, turqPalette4.g, turqPalette4.b,
                    turqPalette5.r, turqPalette5.g, turqPalette5.b,
                    turqPalette6.r, turqPalette6.g, turqPalette6.b,
                    turqPalette7.r, turqPalette7.g, turqPalette7.b,
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

        setUnits(map, enemy, Unit.UnitTeam.ENEMY, Quaternion.identity);

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
        enemy = new Unit[]{genericNoble(), genericPriestess(), genericPriestess(),
                    genericPriestess(), moonstone(), turquoise(), genericNoble(), genericNoble(),
                    genericNoble(), genericNoble(), genericNoble(),
                    genericNoble(), genericPriestess(), genericPriestess(),
                    genericPriestess(), genericPriestess()};
        ally = new Unit[] { };
        other = new Unit[] { };
        string[] teamNames = { "Crystal Gems", "Homeworld", "", "" };

        gridmap.gameObject.SetActive(true);
        Tile[,] map = createMap(StaticData.findDeepChild(gridmap.transform, "MapTransform"));
        gridmap.constructor(map,
            playerList.ToArray(), enemy, ally, other,
            new SeizeObjective(), CHAPTER_TITLE, teamNames, TURNPAR,
            new string[] {  },
            new string[] {  });

        setUnits(map, player, Unit.UnitTeam.PLAYER, Quaternion.Euler(0, 180, 0));
        setUnits(map, enemy, Unit.UnitTeam.ENEMY, Quaternion.identity);

        gridmap.initializeCursorPosition();

        return gridmap;
    }

}

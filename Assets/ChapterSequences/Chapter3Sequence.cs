using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter3Sequence : Chapter
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

    [SerializeField] private Color priestHair;
    [SerializeField] private Color priestSkin;
    [SerializeField] private Color priestEyes;
    [SerializeField] private Color priestPalette4;
    [SerializeField] private Color priestPalette5;
    [SerializeField] private Color priestPalette6;
    [SerializeField] private Color priestPalette7;

    [SerializeField] private Color nephHair;
    [SerializeField] private Color nephSkin;
    [SerializeField] private Color nephEyes;
    [SerializeField] private Color nephPalette4;
    [SerializeField] private Color nephPalette5;
    [SerializeField] private Color nephPalette6;
    [SerializeField] private Color nephPalette7;

    [SerializeField] private Color citrineHair;
    [SerializeField] private Color citrineSkin;
    [SerializeField] private Color citrineEyes;
    [SerializeField] private Color citrinePalette4;
    [SerializeField] private Color citrinePalette5;
    [SerializeField] private Color citrinePalette6;
    [SerializeField] private Color citrinePalette7;

    [SerializeField] private Color avenHair;
    [SerializeField] private Color avenSkin;
    [SerializeField] private Color avenEyes;
    [SerializeField] private Color avenPalette4;
    [SerializeField] private Color avenPalette5;
    [SerializeField] private Color avenPalette6;
    [SerializeField] private Color avenPalette7;

    [SerializeField] private Color chertHair;
    [SerializeField] private Color chertSkin;
    [SerializeField] private Color chertEyes;
    [SerializeField] private Color chertPalette4;
    [SerializeField] private Color chertPalette5;
    [SerializeField] private Color chertPalette6;
    [SerializeField] private Color chertPalette7;

    [SerializeField] private Color flintHair;
    [SerializeField] private Color flintSkin;
    [SerializeField] private Color flintEyes;
    [SerializeField] private Color flintPalette4;
    [SerializeField] private Color flintPalette5;
    [SerializeField] private Color flintPalette6;
    [SerializeField] private Color flintPalette7;

    [SerializeField] private Color eliteHair;
    [SerializeField] private Color eliteSkin;
    [SerializeField] private Color eliteEyes;
    [SerializeField] private Color elitePalette4;
    [SerializeField] private Color elitePalette5;
    [SerializeField] private Color elitePalette6;
    [SerializeField] private Color elitePalette7;

    [SerializeField] private Color topazHair;
    [SerializeField] private Color topazSkin;
    [SerializeField] private Color topazEyes;
    [SerializeField] private Color topazPalette4;
    [SerializeField] private Color topazPalette5;
    [SerializeField] private Color topazPalette6;
    [SerializeField] private Color topazPalette7;

    [SerializeField] private Color guardHair;
    [SerializeField] private Color guardSkin;
    [SerializeField] private Color guardEyes;
    [SerializeField] private Color guardPalette4;
    [SerializeField] private Color guardPalette5;
    [SerializeField] private Color guardPalette6;
    [SerializeField] private Color guardPalette7;




    // Start is called before the first frame update
    void Start()
    {
        ChapterTitle title = Instantiate(chapterTitlePrefab);
        title.constructor(CHAPTER_TITLE);
        seqMem = title;

        materialDictionary = new Dictionary<char, Material>();
        materialDictionary.Add('_', floor);
        materialDictionary.Add('-', grass);
        materialDictionary.Add('A', grass);
        materialDictionary.Add('P', peak);
        materialDictionary.Add('^', mountain);
        materialDictionary.Add('C', cave);
        materialDictionary.Add('H', grass);
        materialDictionary.Add('c', cliff);
        materialDictionary.Add('+', heal);
        materialDictionary.Add('V', grass);

        decoDictionary = new Dictionary<char, GameObject>();
        decoDictionary.Add('A', tree);
        decoDictionary.Add('H', house);
        decoDictionary.Add('V', village);

        dialogueEvents = new string[][] { house1(), village1(), house2() };

        tileMap = new string[]
        {
            "--A--AAAPPPPPPPA----",
            "A------A^PPPPP^-___-",
            "--PPP----PPPPPP-___-",
            "--PPP--^^PPPPPPA___-",
            "H-PCP----PPPPPP-----",
            "c-----PPPPPPPPPcc---",
            "A---ccPPPPPPPPPA-c--",
            "-----VHAPPPPPPP^A---",
            "-------A^PPPPPP^A---",
            "--------APPPPPPA----",
            "---------PPPPPP-----",
            "-------PPPPPPPPP----",
            "-------PPPPPPPPP----",
            "--------PPPAPPP----A",
            "-----AcccPPAA-P----A",
            "----cc-AA------+----",
            "---+------------+PPP",
            "AAc-------------PPPP",
            "Acc-------------PPPP"
        };

        deployMap = new string[]
        {
            "!!A!!AAAPPPPPPPA!!!!",
            "A!!!!!!A^PPPPP^!___!",
            "!!PPP!!!!PPPPPP!_x_!",
            "!!PPP!!^^PPPPPPA___!",
            "H!PCP!!!!PPPPPP!!!!x",
            "c!!!!!PPPPPPPPPcc!!!",
            "A!!!ccPPPPPPPPPAxc!x",
            "!!!!!VHAPPPPPPP^Axx!",
            "!!*!!!!A^PPPPPP^A!!x",
            "!***!!!!APPPPPPA!!xx",
            "*!*!*!!!!PPPPPP!!xxx",
            "!*!*!!!PPPPPPPPP!!xx",
            "!!!!!!!PPPPPPPPP!!!!",
            "!!!!!!!!PPPAPPP!!!!A",
            "!!!x!AcccPPAA!P!!!!A",
            "!!!!cc!AA!!!!!!+!!!!",
            "!x!+!x!!!!!!o!!!+PPP",
            "AAc!!!!!x!!!!oo!PPPP",
            "Acc!x!x!!!!!o!!!PPPP"
        };

        lootMap = new string[]
        {
            "--A--AAAPPPPPPPA----",
            "A------A^PPPPP^-___-",
            "--PPP----PPPPPP-___-",
            "--PPP--^^PPPPPPA___-",
            "H-PCP----PPPPPP-----",
            "c-----PPPPPPPPPcc---",
            "A---ccPPPPPPPPPA-c--",
            "-----VHAPPPPPPP^A---",
            "-------A^PPPPPP^A---",
            "--------APPPPPPA----",
            "---------PPPPPP-----",
            "-------PPPPPPPPP----",
            "-------PPPPPPPPP----",
            "--------PPPAPPP----A",
            "-----AcccPPAA-P----A",
            "----cc-AA------+----",
            "---+------------+PPP",
            "AAc-------------PPPP",
            "Acc-------------PPPP"
        };

        heightMap = new string[]
        {
            "00000000244433200000",
            "00000000234433200000",
            "00444000024443200000",
            "00444001124563200000",
            "00404000023443200000",
            "10000024556644332000",
            "00002234577743200100",
            "00000000247765320000",
            "00000000224654320000",
            "00000000023344200000",
            "00000000023443200000",
            "00000002334443320000",
            "00000002334333320000",
            "00000000444033200000",
            "00000033344000200000",
            "00002200000000010000",
            "00010000000000001233",
            "00100000000000002234",
            "01100000000000003445"
        };

        decoMap = new string[]
        {
            "--A--AAAPPPPPPPA----",
            "A------A^PPPPP^-___-",
            "--PPP----PPPPPP-___-",
            "--PPP--^^PPPPPPA___-",
            "H-PCP----PPPPPP-----",
            "c-----PPPPPPPPPcc---",
            "A---ccPPPPPPPPPA-c--",
            "-----VHAPPPPPPP^A---",
            "-------A^PPPPPP^A---",
            "--------APPPPPPA----",
            "---------PPPPPP-----",
            "-------PPPPPPPPP----",
            "-------PPPPPPPPP----",
            "--------PPPAPPP----A",
            "-----AcccPPAA-P----A",
            "----cc-AA------+----",
            "---+------------+PPP",
            "AAc-------------PPPP",
            "Acc-------------PPPP"
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
                goToChapter(-4);
            }
        }
    }


    public string[] getIntro()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string bismuth = "Bismuth Bismuth ";
        string citrine = "Citrine Citrine ";
        string aven = "Aventurine Aventurine ";
        string neph = "Nephrite Nephrite ";
        string neph_c = "Nephrite Nephrite_C ";
        return new string[]
        {
            "$image the-answer-back",
            "$left Pearl " + AssetDictionary.PORTRAIT_HAPPY,
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_SAD,
            pearl + "Blue Diamond's cloud arena is just ahead. This is our chance!",
            rose + "*sigh* What are we doing, Pearl?",
            "$left Pearl " + AssetDictionary.PORTRAIT_CONFUSED,
            pearl + "Huh? We're attacking Blue Diamond at her palanquin to show her that Earth is not a viable colony. Would you like me to go over the plan again?",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "No, I mean what's the point? She's not going to listen. This isn't going to convince her.",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_SAD,
            "$left Pearl " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "Why are we still fighting when we both know it's not going to save the Earth?",
            "$left Pearl " + AssetDictionary.PORTRAIT_SAD,
            rose + "...Is Earth even really worth saving?",
            pearl + "...",
            "$sound before-chapter-music3 start",
            "$sound before-chapter-music3 loop",
            "$right Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            bismuth + "Hey, Rose! Look who I just found!",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "Whoa. You alright?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NERVOUS,
            rose + "I'm fine. Who did you find?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$right Citrine " + AssetDictionary.PORTRAIT_HAPPY,
            citrine + "No way. It's her! It's actually her! Aventurine, look!",
            "$right Aventurine " + AssetDictionary.PORTRAIT_HAPPY,
            aven + "Oh my stars! I can't believe it!",
            rose + "Umm hi. Are you new recruits?",
            "$right Citrine " + AssetDictionary.PORTRAIT_HAPPY,
            citrine + "Well not exactly. But we're here to help!",
            "$right Aventurine " + AssetDictionary.PORTRAIT_HAPPY,
            aven + "We're part of another rebellion effort, see?",
            aven + "The Refraction Stones!",
            "$right Citrine " + AssetDictionary.PORTRAIT_DARING,
            citrine + "We have orders to help you get up to Blue Diamond's cloud arena.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_CONFUSED,
            rose + "Orders from whom?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$right Nephrite_c " + AssetDictionary.PORTRAIT_DARING,
            neph_c + "That'd *cough* *cough* be me!",
            neph_c + "Nephrite, Facet XJ - Cut 763. I'm originally from Yellow Diamond's navy *cough*",
            neph_c + "When we heard about a Rose *cough* Quartz disrupting Pink Diamond's colony, we *cough* just had to show our support.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_SAD,
            rose + "Your Gem! It's cracked!",
            "$right Nephrite_c " + AssetDictionary.PORTRAIT_HAPPY,
            neph_c + "Oh this? Yeah, we weren't exactly welcomed on Earth.",
            "$left Bismuth " + AssetDictionary.PORTRAIT_ANGRY,
            bismuth + "How could Homeworld do this to you?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_SAD,
            neph_c + "It's just a scratch. No big--",
            "$right Nephrite_c _glitch",
            neph_c + "RN UILN QFMTOV NLLM",
            "$right Nephrite_c " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "Oh no. I'm so sorry. This is all my fault.",
            neph_c + "*cough* What do you mean?",
            rose + "If I had never started this stupid rebellion, you wouldn't have gotten hurt *sniff* *sob*",
            rose + "*crying * I'm so sorry!",
            "$right Nephrite " + AssetDictionary.PORTRAIT_HAPPY,
            neph + "Wha-- My Gem! It's healed!",
            "$left Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "You have healing tears?!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "I have healing tears?",
            "$right Nephrite " + AssetDictionary.PORTRAIT_DARING,
            neph + "Citrine, Aventurine, go ahead and start clearing the way. Chert and Flint are waiting.",
            neph + "Rose, I'll be joining your party! It's the least I can do after you healed me.",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$left Pearl " + AssetDictionary.PORTRAIT_NERVOUS,
            pearl + "Are you going to be okay, Rose?",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "I... Sure. Let's get this over with.",
            "$sound itemget play",
            "_ null Rose gained the skill \"Healing Tears\"!"
        };
    }

    public string[] getEnding()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string bismuth = "Bismuth Bismuth ";
        string neph = "Nephrite Nephrite ";
        string guard = "Guard Guard ";
        string garnet = "Fusion Garnet_E ";
        string garnet_named = "Garnet Garnet_E ";

        int garnetModel = 0;
        int roseModel = 1;
        int pearlModel = 2;

        int rosePos = 0;
        int garnetPos = 1;

        return new string[]
        {
            "$image the-answer-back",
            "$left Bismuth " + AssetDictionary.PORTRAIT_DARING,
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "The way is clear. It's all you and Pearl from here. We'll stay down here and keep them from gathering reinforcements.",
            "$if alive Nephrite",
            "$left Nephrite " + AssetDictionary.PORTRAIT_DARING,
            neph + "Don't worry about us. We'll follow you whole or in pieces!",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_NERVOUS,
            rose + "Er-- right....",
            "$endif",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "Thank you all for your help. Let's go, Pearl.",

            "$silence",
            "$solidColor 0 0 0 1",
            "$pause 1",
            "$image cloud-arena",
            "$sound map-music-1 start",
            "$sound map-music-1 loop",

            rose + "Blue Diamond, leave this planet! This colony will not be completed!",
            guard + "It's the rebels!",
            guard + "Who are you?! Show yourselves!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            "$right Pearl " + AssetDictionary.PORTRAIT_DARING,
            "Rose_+_Pearl null " + "We are the Crystal Gems!",

            "$silence",
            "$solidColor 0 0 0 1",
            "$sound damage start",
            "$pause 1",
            "$sound damage start",
            "$pause 1",
            "$sound poof start",
            "$pause 2",

            "$sound map-music-1 stop",
            "$image garnet-first-fusion",
            "$pause 3",
            rose + "This is...",
            "$silence",
            "$pause 3",

            "$sound peaceful-music start",
            "$equip " + pearlModel + " Iron_Sword",
            "$animate " + pearlModel + " Sword_Idle",
            "$animate " + garnetModel + " Old_Man_Idle",
            "$image null",

            garnet + "Ah! Don't hurt her!",
            garnet + "Don't hurt me?",
            pearl + "It's you! The fusion.",
            garnet + "We didn't mean to fuse!",
            garnet + "Well, we did this time...",
            garnet + "We'll unfuse! We'll--",
            "$animate " + roseModel + " Standard_Walk",
            "$moveCharacter " + roseModel + " " + rosePos + " 5",
            "$pause 2.5",
            "$animate " + roseModel + " Idle",
            "$animate " + garnetModel + " Idle",
            "$rotateCharacter " + roseModel + " " + garnetPos,
            "$rotateCharacter " + garnetModel + " " + rosePos,
            rose + "No, no, please. I'm glad to see you again.",
            "$animate " + pearlModel + " Idle",
            "$animate " + garnetModel + " Talking",
            garnet + "I don't upset you?",
            "$animate " + garnetModel + " Idle",
            "$animate " + roseModel + " Talking",
            rose + "Who cares about how I feel? How you feel is bound to be much more interesting.",
            "$animate " + roseModel + " Idle",
            "$animate " + garnetModel + " Talking",
            garnet + "How I feel? I feel... lost... and scared... a-and happy.",
            garnet + "Why am I so sure that I'd rather be this than everything I was supposed to be?",
            garnet + "And that I'd rather do this than everything I was supposed to do?",
            "$animate " + garnetModel + " Idle",
            "$animate " + roseModel + " Laughing",
            rose + "*chuckles* Welcome to Earth.",
            "$animate " + roseModel + " Idle",
            "$animate " + garnetModel + " Talking_2",
            garnet + "Can you tell me? How was Ruby able to alter fate?",
            garnet + "Why was Sapphire willing to give up everything?",
            garnet + "W-What am I?!",
            "$animate " + garnetModel + " Idle",
            "$animate " + roseModel + " Talking",
            rose + "No more questions. Don't ever question this. You already are the answer.",
            rose + "Do you have a name?",
            "$animate " + roseModel + " Idle",
            "$animate " + garnetModel + " Talking",
            garnet + "No. I mean... I feel like maybe... Garnet.",
            "$animate " + garnetModel + " Idle",
            "$animate " + roseModel + " Talking",
            rose + "A Ruby and a Sapphire make a Garnet? How fascinating.",
            rose + "Would you like to come with us, Garnet?",
            rose + "I think it's because of Gems like you that we're fighting.",
            rose + "Here on Earth, Gems can be extraordinary, like you. It won't be easy, but no matter what it takes...",
            rose + "I think the love that forms you is worth fighting for. What do you say?",
            "$animate " + roseModel + " Idle",
            "$animate " + garnetModel + " Talking",
            garnet_named + "...",
            garnet_named + "I will.",
            "$animate " + garnetModel + " Idle",
            "$animate " + roseModel + " Talking",
            rose + "Then welcome, Garnet to the Crystal Gems!",
        };
    }

    private string[] house1()
    {
        string human = "Human null ";
        return new string[]
        {
            "$sound convo-music playMusic",
            human + "Hey, whenever you're done littering stones everywhere, could you try and pick up after yourselves?",
            human + "Those rocks are no good just scattered over the ground. Maybe you can use them for something."
        };
    }
    private string[] village1()
    {
        string human = "Human null ";
        return new string[]
        {
            "$sound convo-music playMusic",
            human + "This ancient robe has the power to prolong life. May it bless you for visiting our humble village.",
            "$reward 19",
            "$sound itemget play",
            "_ null Received Snerson Robe"
        };
    }
    private string[] house2()
    {
        string human = "Human null ";
        return new string[]
        {
            "$sound convo-music playMusic",
            human + "Know the Weapon Triangle!",
            human + "Swords and Fists beat Axes and Whips",
            human + "Axes and Whips beat Lances and Armor",
            human + "Lances and Armor beat Swords and Fists",
            human + "\"Hard to remember,\" you say? Well don't look at me. I didn't make this game!",
        };
    }

    private Unit genericQuartz()
    {
        string quartz_desc = "Soldier serving the diamonds against the rebellion";
        Weapon wep = Item.quartz_axe;
        Unit quartz = new Unit();
        UnitClass soldier = UnitClass.soldier;
        quartz.constructor("Quartz", soldier, quartz_desc,
                24, 8, 0, 3, 6, 2, 4, 0, 9, 6,
                80, 40, 5, 20, 30, 30, 30, 5,
                wep, Weapon.WeaponType.FIST, 20, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.FIRE,
                new float[] {
                    quartzHair.r, quartzHair.g, quartzHair.b,
                    quartzSkin.r, quartzSkin.g, quartzSkin.b,
                    quartzEyes.r, quartzEyes.g, quartzEyes.b,
                    quartzPalette4.r, quartzPalette4.g, quartzPalette4.b,
                    quartzPalette5.r, quartzPalette5.g, quartzPalette5.b,
                    quartzPalette6.r, quartzPalette6.g, quartzPalette6.b,
                    quartzPalette7.r, quartzPalette7.g, quartzPalette7.b,
                });
        quartz.ai1 = Unit.AIType.ATTACK; quartz.ai2 = Unit.AIType.GUARD;
        quartz.recruitability = 10;
        quartz.recruitQuote = new string[]
        {
            "Alright, I'm convinced. Need any more help?"
        };
        return quartz;
    }
    private Unit genericPriestess()
    {
        //Based on FE3 Gordon
        string priest_desc = "A priestess in the service of the Moon Goddess";
        Weapon wep = Item.priest_bow;
        Unit priest = new Unit();
        UnitClass priestClass = UnitClass.priestess;
        priest.constructor("Priestess", priestClass, priest_desc,
                18, 5, 0, 5, 4, 4, 6, 0, 5, 6,
                40, 30, 3, 30, 30, 40, 10, 3,
                wep, Weapon.WeaponType.SWORD, 0, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.LIGHT,
                new float[] {
                    priestHair.r, priestHair.g, priestHair.b,
                    priestSkin.r, priestSkin.g, priestSkin.b,
                    priestEyes.r, priestEyes.g, priestEyes.b,
                    priestPalette4.r, priestPalette4.g, priestPalette4.b,
                    priestPalette5.r, priestPalette5.g, priestPalette5.b,
                    priestPalette6.r, priestPalette6.g, priestPalette6.b,
                    priestPalette7.r, priestPalette7.g, priestPalette7.b,
                });
        priest.ai1 = Unit.AIType.ATTACK; priest.ai2 = Unit.AIType.PURSUE;
        priest.recruitability = 10;
        priest.recruitQuote = new string[]
        {
            "May the Moon Goddess bring you victory."
        };
        return priest;
    }
    private Unit nephrite()
    {
        //Based on FE5 Karin
        //1 point SPD buff so she doubles in this chapter
        string neph_desc = "A rebellious pilot from Yellow Diamond's navy";
        Weapon wep = Item.ship_laser;
        Unit neph = new Unit();
        UnitClass pilot = UnitClass.pilot;
        neph.constructor("Nephrite", pilot, neph_desc,
                18, 4, 7, 4, 15, 12, 4, 7, 4, 8,
                55, 30, 15, 35, 70, 70, 15, 15,
                wep, Weapon.WeaponType.LANCE, 10, Unit.UnitTeam.PLAYER, -1, -1,
                Unit.Affinity.HEAVEN,
                new float[] {
                    nephHair.r, nephHair.g, nephHair.b,
                    nephSkin.r, nephSkin.g, nephSkin.b,
                    nephEyes.r, nephEyes.g, nephEyes.b,
                    nephPalette4.r, nephPalette4.g, nephPalette4.b,
                    nephPalette5.r, nephPalette5.g, nephPalette5.b,
                    nephPalette6.r, nephPalette6.g, nephPalette6.b,
                    nephPalette7.r, nephPalette7.g, nephPalette7.b,
                });
        neph.level = 5;
        neph.heldItem = Item.currentHP.clone();
        return neph;
    }
    private Unit citrine()
    {
        //Based on FE3 Oguma
        //1 point MOV buff
        string citr_desc = "A rebel soldier and Aventurine's soulmate";
        Weapon wep = Item.citrine_sword;
        Unit citr = new Unit();
        UnitClass eliteClass = UnitClass.elite_quartz;
        citr.constructor("Citrine", eliteClass, citr_desc,
                22, 6, 0, 11, 12, 3, 6, 0, 6, 7,
                80, 40, 3, 20, 30, 40, 30, 3,
                wep, Weapon.WeaponType.LANCE, 30, Unit.UnitTeam.ALLY, -1, -1,
                Unit.Affinity.EARTH,
                new float[] {
                    citrineHair.r, citrineHair.g, citrineHair.b,
                    citrineSkin.r, citrineSkin.g, citrineSkin.b,
                    citrineEyes.r, citrineEyes.g, citrineEyes.b,
                    citrinePalette4.r, citrinePalette4.g, citrinePalette4.b,
                    citrinePalette5.r, citrinePalette5.g, citrinePalette5.b,
                    citrinePalette6.r, citrinePalette6.g, citrinePalette6.b,
                    citrinePalette7.r, citrinePalette7.g, citrinePalette7.b,
                });
        citr.ai1 = Unit.AIType.ATTACK; citr.ai2 = Unit.AIType.PURSUE;
        return citr;
    }
    private Unit aventurine()
    {
        //Based on FE3 Navarre
        string aven_desc = "A rebel soldier and Citrine's soulmate";
        Weapon wep = Item.aventurine_axe;
        Unit aven = new Unit();
        UnitClass soldier = UnitClass.soldier;
        aven.constructor("Aventurine", soldier, aven_desc,
                19, 5, 0, 9, 11, 8, 6, 0, 8, 6,
                90, 50, 3, 40, 50, 60, 20, 3,
                wep, Weapon.WeaponType.LANCE, 20, Unit.UnitTeam.ALLY, -1, -1,
                Unit.Affinity.WIND,
                new float[] {
                    avenHair.r, avenHair.g, avenHair.b,
                    avenSkin.r, avenSkin.g, avenSkin.b,
                    avenEyes.r, avenEyes.g, avenEyes.b,
                    avenPalette4.r, avenPalette4.g, avenPalette4.b,
                    avenPalette5.r, avenPalette5.g, avenPalette5.b,
                    avenPalette6.r, avenPalette6.g, avenPalette6.b,
                    avenPalette7.r, avenPalette7.g, avenPalette7.b,
                });
        aven.ai1 = Unit.AIType.ATTACK; aven.ai2 = Unit.AIType.PURSUE;
        return aven;
    }
    private Unit chert()
    {
        //Based on FE3 Oguma (Book 2)
        string chert_desc = "A rebel soldier and Flint's best friend";
        Weapon wep = Item.pacifist_gauntlet;
        Unit chert = new Unit();
        UnitClass soldier = UnitClass.soldier;
        chert.constructor("Chert", soldier, chert_desc,
                25, 8, 0, 14, 14, 5, 8, 0, 8, 6,
                80, 40, 3, 30, 30, 40, 30, 3,
                wep, Weapon.WeaponType.SPECIAL, 0, Unit.UnitTeam.ALLY, -1, -1,
                Unit.Affinity.ANIMA,
                new float[] {
                    chertHair.r, chertHair.g, chertHair.b,
                    chertSkin.r, chertSkin.g, chertSkin.b,
                    chertEyes.r, chertEyes.g, chertEyes.b,
                    chertPalette4.r, chertPalette4.g, chertPalette4.b,
                    chertPalette5.r, chertPalette5.g, chertPalette5.b,
                    chertPalette6.r, chertPalette6.g, chertPalette6.b,
                    chertPalette7.r, chertPalette7.g, chertPalette7.b,
                });
        chert.ai1 = Unit.AIType.IDLE; chert.ai2 = Unit.AIType.IDLE;
        return chert;
    }
    private Unit flint()
    {
        //Based on FE3 Navarre (Book 2)
        string flint_desc = "A rebel soldier and Chert's best friend";
        Weapon wep = Item.pacifist_gauntlet;
        Unit flint = new Unit();
        UnitClass soldier = UnitClass.soldier;
        flint.constructor("Flint", soldier, flint_desc,
                23, 9, 0, 16, 16, 7, 10, 0, 8, 6,
                90, 50, 3, 40, 50, 60, 20, 3,
                wep, Weapon.WeaponType.CLUB, 0, Unit.UnitTeam.ALLY, -1, -1,
                Unit.Affinity.ANIMA,
                new float[] {
                    flintHair.r, flintHair.g, flintHair.b,
                    flintSkin.r, flintSkin.g, flintSkin.b,
                    flintEyes.r, flintEyes.g, flintEyes.b,
                    flintPalette4.r, flintPalette4.g, flintPalette4.b,
                    flintPalette5.r, flintPalette5.g, flintPalette5.b,
                    flintPalette6.r, flintPalette6.g, flintPalette6.b,
                    flintPalette7.r, flintPalette7.g, flintPalette7.b,
                });
        flint.ai1 = Unit.AIType.IDLE; flint.ai2 = Unit.AIType.IDLE;
        return flint;
    }
    private Unit genericElite()
    {
        //Based on FE3 Oguma and Hardin (best parts of both, but worst of Luck, and only 6 Speed)
        //1 point MOV buff
        string elite_desc = "A specialized officer in the Diamonds' armies";
        Weapon wep = Item.elite_sword;
        Unit elite = new Unit();
        UnitClass eliteClass = UnitClass.elite_quartz;
        elite.constructor("Elite", eliteClass, elite_desc,
                22, 6, 0, 11, 6, 3, 6, 0, 9, 7,
                90, 50, 3, 40, 50, 60, 30, 3,
                wep, Weapon.WeaponType.AXE, 30, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.EARTH,
                new float[] {
                    eliteHair.r, eliteHair.g, eliteHair.b,
                    eliteSkin.r, eliteSkin.g, eliteSkin.b,
                    eliteEyes.r, eliteEyes.g, eliteEyes.b,
                    elitePalette4.r, elitePalette4.g, elitePalette4.b,
                    elitePalette5.r, elitePalette5.g, elitePalette5.b,
                    elitePalette6.r, elitePalette6.g, elitePalette6.b,
                    elitePalette7.r, elitePalette7.g, elitePalette7.b,
                });
        elite.ai1 = Unit.AIType.ATTACK; elite.ai2 = Unit.AIType.GUARD;
        elite.recruitability = 20;
        elite.recruitQuote = new string[]
        {
            "I think I actually agree with you. Heh, didn't see that coming."
        };
        return elite;
    }
    private Unit genericTopazFusion()
    {
        //Based on FE5 Xavier
        string topaz_desc = "A powerful soldier, fused for extra strength";
        Weapon wep = Item.topaz_lance;
        Unit topaz = new Unit();
        UnitClass topazFusion = UnitClass.topaz_fusion;
        topaz.constructor("Officer", topazFusion, topaz_desc,
                38, 13, 3, 9, 6, 3, 13, 3, 15, 6,
                50, 40, 45, 55, 50, 70, 15, 45,
                wep, Weapon.WeaponType.CLUB, 40, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.LIGHTNING,
                new float[] {
                    topazHair.r, topazHair.g, topazHair.b,
                    topazSkin.r, topazSkin.g, topazSkin.b,
                    topazEyes.r, topazEyes.g, topazEyes.b,
                    topazPalette4.r, topazPalette4.g, topazPalette4.b,
                    topazPalette5.r, topazPalette5.g, topazPalette5.b,
                    topazPalette6.r, topazPalette6.g, topazPalette6.b,
                    topazPalette7.r, topazPalette7.g, topazPalette7.b,
                });
        topaz.ai1 = Unit.AIType.GUARD; topaz.ai2 = Unit.AIType.GUARD;
        topaz.movement = 0;
        topaz.fusionSkillBonus = Unit.FusionSkill.ABSORPTION;
        topaz.recruitability = 50;
        topaz.recruitQuote = new string[]
        {
            "I want to help you guys. What do you need?"
        };
        return topaz;
    }
    private Unit genericGuard()
    {
        //Based on FE3 Julian
        //4 point SPD nerf
        string ruby_desc = "A common guard in the service of the Diamonds";
        Weapon wep = Item.guard_shield;
        Unit ruby = new Unit();
        UnitClass guard = UnitClass.guard;
        ruby.constructor("Guard", guard, ruby_desc,
                17, 4, 0, 6, 3, 7, 4, 0, 5, 5,
                50, 40, 45, 55, 50, 70, 15, 45,
                wep, Weapon.WeaponType.CLUB, 10, Unit.UnitTeam.ENEMY, -1, -1,
                Unit.Affinity.FIRE,
                new float[] {
                    guardHair.r, guardHair.g, guardHair.b,
                    guardSkin.r, guardSkin.g, guardSkin.b,
                    guardEyes.r, guardEyes.g, guardEyes.b,
                    guardPalette4.r, guardPalette4.g, guardPalette4.b,
                    guardPalette5.r, guardPalette5.g, guardPalette5.b,
                    guardPalette6.r, guardPalette6.g, guardPalette6.b,
                    guardPalette7.r, guardPalette7.g, guardPalette7.b,
                });
        ruby.ai1 = Unit.AIType.BURN; ruby.ai2 = Unit.AIType.ATTACK;
        ruby.recruitability = 10;
        ruby.recruitQuote = new string[]
        {
            "I like your luster!"
        };
        return ruby;
    }


    private PreBattleMenu makePrepMenu()
    {
        if (StaticData.chapterPrep != StaticData.scene)
        {
            Unit neph_unit = nephrite();
            StaticData.members.Add(neph_unit);
            StaticData.getUnitByName("Rose Quartz").fusionSkillBonus = Unit.FusionSkill.HEALING;

            StaticData.positions = new int[] { 0, -1, -1, -1, -1, -1, -1, -1, -1 };

            StaticData.chapterPrep = StaticData.scene;
        }

        enemy = new Unit[]{genericTopazFusion(), genericPriestess(), genericQuartz(), genericGuard(),
                    genericPriestess(), genericElite(), genericGuard(), genericGuard(), genericQuartz(),
                    genericQuartz(), genericQuartz(), genericQuartz(), genericGuard(), genericGuard(),
                    genericGuard(), genericGuard(), genericGuard(), genericQuartz(), genericQuartz(),
                    genericQuartz()};
        ally = new Unit[] { citrine(), flint(), aventurine(), chert() };
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
        enemy = new Unit[]{ genericTopazFusion(), genericPriestess(), genericQuartz(), genericGuard(),
                    genericPriestess(), genericElite(), genericGuard(), genericGuard(), genericQuartz(),
                    genericQuartz(), genericQuartz(), genericQuartz(), genericGuard(), genericGuard(),
                    genericGuard(), genericGuard(), genericGuard(), genericQuartz(), genericQuartz(),
                    genericQuartz() };
        ally = new Unit[] { citrine(), flint(), aventurine(), chert() };
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

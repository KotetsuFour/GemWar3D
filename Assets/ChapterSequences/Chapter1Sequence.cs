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

    [SerializeField] private Color roseQuartzHair;
    [SerializeField] private Color roseQuartzSkin;
    [SerializeField] private Color roseQuartzEyes;
    [SerializeField] private Color roseQuartzPalette4;
    [SerializeField] private Color roseQuartzPalette5;
    [SerializeField] private Color roseQuartzPalette6;
    [SerializeField] private Color roseQuartzPalette7;

    [SerializeField] private Color pearlHair;
    [SerializeField] private Color pearlSkin;
    [SerializeField] private Color pearlEyes;
    [SerializeField] private Color pearlPalette4;
    [SerializeField] private Color pearlPalette5;
    [SerializeField] private Color pearlPalette6;
    [SerializeField] private Color pearlPalette7;

    [SerializeField] private Color bismuthHair;
    [SerializeField] private Color bismuthSkin;
    [SerializeField] private Color bismuthEyes;
    [SerializeField] private Color bismuthPalette4;
    [SerializeField] private Color bismuthPalette5;
    [SerializeField] private Color bismuthPalette6;
    [SerializeField] private Color bismuthPalette7;

    [SerializeField] private Color biggsHair;
    [SerializeField] private Color biggsSkin;
    [SerializeField] private Color biggsEyes;
    [SerializeField] private Color biggsPalette4;
    [SerializeField] private Color biggsPalette5;
    [SerializeField] private Color biggsPalette6;
    [SerializeField] private Color biggsPalette7;

    [SerializeField] private Color oceanHair;
    [SerializeField] private Color oceanSkin;
    [SerializeField] private Color oceanEyes;
    [SerializeField] private Color oceanPalette4;
    [SerializeField] private Color oceanPalette5;
    [SerializeField] private Color oceanPalette6;
    [SerializeField] private Color oceanPalette7;

    [SerializeField] private Color quartzHair;
    [SerializeField] private Color quartzSkin;
    [SerializeField] private Color quartzEyes;
    [SerializeField] private Color quartzPalette4;
    [SerializeField] private Color quartzPalette5;
    [SerializeField] private Color quartzPalette6;
    [SerializeField] private Color quartzPalette7;

    public static string CHAPTER_TITLE = "Chapter 1 - The Crystal Gems";

    // Start is called before the first frame update
    void Start()
    {
        ChapterTitle title = Instantiate(chapterTitle);
        title.constructor(CHAPTER_TITLE);
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
        handleInput(seqMem);

        if (seqMem.completed())
        {
            sequenceNum++;
            if (sequenceNum == 1)
            {
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
                goToChapter(2);
            }
        }
    }

    public string[] opening()
    {
        string noOne = "_ null ";
        return new string[]
        {
            "$solidColor 0 0 0",
            "$sound storytelling-music start",
            "$sound storytelling-music loop",
            noOne + "For hundreds of millions of years, the Great Diamond Authority spread their perfection throughout the cosmos.",
            noOne + "They bent planets to their will and expanded their empire, creating life from nothing.",
            noOne + "White, Yellow, and Blue Diamond all possessed many worlds.",
            noOne + "But the littlest Diamond, Pink Diamond, had only the Earth.",
            noOne + "Pink Diamond fell in love with the life that existed on the planet",
            noOne + "And she realized that she could not go through with creating her colony.",
            noOne + "But the other Diamonds wouldn't listen.",
            noOne + "They told her she was being ridiculous.",
            noOne + "So Pink Diamond donned her false persona of Rose Quartz.",
            noOne + "Together with her Pearl, she rebelled against the Diamonds as someone they couldn't ignore.",
            noOne + "She spoke out at the Prime Kindergarten and made her case known",
            noOne + "And now, at the Sky Arena, Rose Quartz aims to recruit allies to her cause."
        };
    }
    public string[] intro()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string quartz = "Quartz Quartz ";
        return new string[]
        {
            "$solidColor 0 0 0",
            "$sound before-chapter-music1 start",
            "$sound before-chapter-music1 loop",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            "$right Pearl " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "Here we are. The Sky Arena",
            rose + "If the Quartzes at the Prime Kindergarten wouldn't join us, maybe some here will.",
            pearl + "I hope so, my Quartz. Soldiers are very important for fighting a war.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_TEASE,
            rose + "Pearl, you don't have to call me \"my Quartz\". Just call me Rose.",
            "$right Pearl " + AssetDictionary.PORTRAIT_NEUTRAL,
            pearl + "Understood, my Rose.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            rose + "*sigh* I saw that coming.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "Just remember the plan.Let me give my speech, and if things go south...",
            "$right Pearl " + AssetDictionary.PORTRAIT_HAPPY,
            pearl + "Yes, then I get to use my new spear that you graciously gave me at the Reef!",
            rose + "*chuckle* You mean the spear you begged me to give you.",
            rose + "Alright, it's time. Wish me luck!",
            pearl + "Good luck, my Rose!",
            "$sound before-chapter-music1 stop",
            "$sound rally-music start",
            "$sound rally-music loop",
            "$right null",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "Gems of Earth! Listen to me!",
            rose + "Look around at what you are building. Is this really what you want?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            rose + "I have made an amazing discovery. This planet is the home of a diverse ecosystem of organic life!",
            rose + "I've seen them for myself! The lives they live and the things they do are extraordinary!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "But our invasion is destroying their home.If we continue, nothing will be left.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            rose + "Will you destroy this life, or will you join it?",
            "$sound rally-music stop",
            "$sound trouble-music start",
            "$sound trouble-music loop",
            "$right Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            quartz + "That's her! It's the Rose Quartz who attacked the kindergarten!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            quartz + "Get her!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "Well, here we go again.",
        };
    }
    public string[] biggsRecruitment()
    {
        string biggs = "Biggs Biggs ";
        string rose = "Rose_Quartz Rose_Quartz ";
        return new string[]
        {
            "$removeConvo",
            "$sound recruit-music playMusic",
            "$left Rose_Quartz",
            "$right Biggs",
            biggs + "Hey you!",
            rose + "Hm?",
            biggs + "I heard your speech. Are you really going to stop the colony?",
            biggs + "All for the earthling organics?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            rose + "I'll do what I have to. I can't let them die.",
            "$right Biggs " + AssetDictionary.PORTRAIT_HAPPY,
            biggs + "You must have a lot of conviction.I want to join you!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "Really?",
            biggs + "Yeah! I'm only building the colony because I was ordered to.",
            biggs + "But you actually believe in what you're doing. It's inspiring!",
            biggs + "So from now on, you have my whip!",
            biggs + "Oh, and if you wouldn't mind, talk to my pal Ocean too. I'm sure she'll join you.",
            "$sound recruit play",
            "$join",
            "_ null Biggs joined your party!"
        };
    }
    public string[] oceanRecruitment()
    {
        string ocean = "Ocean Ocean ";
        return new string[]
        {
            "$removeConvo",
            "$sound recruit-music playMusic",
            "$right Ocean " + AssetDictionary.PORTRAIT_ANGRY,
            ocean + "What's that? You want me to join you?",
            ocean + "To turn my back on everything we've built here and betray the Diamonds?",
            ocean + "All for the sake of one planet? You're insane.",
            "$right Ocean " + AssetDictionary.PORTRAIT_HAPPY,
            ocean + "I kinda admire that.",
            ocean + "Alright, I'll join you! Let's do this!",
            "$sound recruit play",
            "$join",
            "_ null Ocean joined your party!"
        };
    }
    public string[] bismuthTalk()
    {
        string bismuth = "Bismuth Bismuth ";
        string rose = "Rose_Quartz Rose_Quartz ";
        return new string[]
        {
            "$removeConvo",
            "$sound convo-music playMusic",
            "$left Rose_Quartz",
            "$right Bismuth " + AssetDictionary.PORTRAIT_SAD,
            bismuth + "Ah! I'm just a builder! Don't hurt me!",
            rose + "I won't hurt you!",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NERVOUS,
            bismuth + "Oh, thanks.",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "Uh... I hope you don't mind, but I'm supposed to finish building this part of the arena.",
            rose + "Is that what you want to do? Build this arena?",
            "$right Bismuth " + AssetDictionary.PORTRAIT_CONFUSED,
            bismuth + "...Huh ?",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "I mean if you had a choice, what would you want to build?",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NERVOUS,
            bismuth + "I...Well... there is this one thing that-- But I can't.",
            bismuth + "Look, I can't talk to you. You're gonna get me in a lot of trouble.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_DARING,
            rose + "Sometimes, trouble is the only way to freedom. Think about it.",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NEUTRAL,
            bismuth + "Wait! You're at a disadvantage with just that shield.",
            "$right Bismuth " + AssetDictionary.PORTRAIT_DARING,
            bismuth + "Here. You and your Pearl take these.",
            "$sound itemget play",
            "$give Rose_Quartz",
            "_ null Rose got an Iron Sword",
            "$sound itemget play",
            "$give Pearl",
            "_ null Pearl got an Iron Sword",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "Thank you! These will be really helpful.",
            rose + "You'd better get out of here. I hope we'll see each other again.",
            "$leave",
            "$right Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            bismuth + "Heh, yeah, maybe we will."
        };
    }
    public string[] ending()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string bismuth = "Bismuth Bismuth ";
        return new string[]
        {
            "$solidColor 0 0 0",
            "$sound peaceful-music start",
            "$sound peaceful-music loop",
            "$left Rose_Quartz",
            "$right Pearl",
            rose + "It's a good thing we got away.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "And it looks like we even inspired some Gems to join us!",
            "$right Pearl " + AssetDictionary.PORTRAIT_ANGRY,
            pearl + "Wait. I heard something... I think we have company...",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "Be careful.",
            "$right Bismuth " + AssetDictionary.PORTRAIT_NERVOUS,
            bismuth + "Hey, hey, wait! I come in peace!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_NEUTRAL,
            rose + "You're the Bismuth from the arena.",
            "$right Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            bismuth + "Yeah. I figured out what I want to do. I don't want to build arenas and spires.",
            bismuth + "I want to make weapons!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_TEASE,
            rose + "Weapons?",
            "$right Bismuth " + AssetDictionary.PORTRAIT_SAD,
            bismuth + "Is that too--",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            "$right Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "No, that's wonderful! In fact, it's just what we need!",
            bismuth + "Well then I guess I'm joining you! The Diamonds don't have use for a Bismuth who makes weapons.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_ANGRY,
            rose + "No, the Diamonds wouldn't understand.",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "But we're glad to have you here in the... um...",
            "$left Pearl " + AssetDictionary.PORTRAIT_HAPPY,
            pearl + "The Crystal Gems!",
            "$right Bismuth " + AssetDictionary.PORTRAIT_CONFUSED,
            bismuth + "The \"Crystal Gems\" ?",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_CONFUSED,
            rose + "The \"Crystal Gems\" ?",
            pearl + "We're a group of Gems that have \"dissolved\" off of Homeworld's structure.",
            pearl + "Then we deposited onto each other to form an organized lattice.",
            pearl + "Like a crystal.",
            "$right Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "I love it!",
            "$left Rose_Quartz " + AssetDictionary.PORTRAIT_HAPPY,
            "$right Bismuth " + AssetDictionary.PORTRAIT_HAPPY,
            rose + "Bismuth, welcome to the Crystal Gems!"
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
                Unit.Affinity.ANIMA,
                new float[] {
                    bismuthHair.r, bismuthHair.g, bismuthHair.b,
                    bismuthSkin.r, bismuthSkin.g, bismuthSkin.b,
                    bismuthEyes.r, bismuthEyes.g, bismuthEyes.b,
                    bismuthPalette4.r, bismuthPalette4.g, bismuthPalette4.b,
                    bismuthPalette5.r, bismuthPalette5.g, bismuthPalette5.b,
                    bismuthPalette6.r, bismuthPalette6.g, bismuthPalette6.b,
                    bismuthPalette7.r, bismuthPalette7.g, bismuthPalette7.b,
                });
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
                Unit.Affinity.EARTH,
                new float[] {
                    roseQuartzHair.r, roseQuartzHair.g, roseQuartzHair.b,
                    roseQuartzSkin.r, roseQuartzSkin.g, roseQuartzSkin.b,
                    roseQuartzEyes.r, roseQuartzEyes.g, roseQuartzEyes.b,
                    roseQuartzPalette4.r, roseQuartzPalette4.g, roseQuartzPalette4.b,
                    roseQuartzPalette5.r, roseQuartzPalette5.g, roseQuartzPalette5.b,
                    roseQuartzPalette6.r, roseQuartzPalette6.g, roseQuartzPalette6.b,
                    roseQuartzPalette7.r, roseQuartzPalette7.g, roseQuartzPalette7.b,

                });
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
                Unit.Affinity.WATER,
                new float[] {
                    pearlHair.r, pearlHair.g, pearlHair.b,
                    pearlSkin.r, pearlSkin.g, pearlSkin.b,
                    pearlEyes.r, pearlEyes.g, pearlEyes.b,
                    pearlPalette4.r, pearlPalette4.g, pearlPalette4.b,
                    pearlPalette5.r, pearlPalette5.g, pearlPalette5.b,
                    pearlPalette6.r, pearlPalette6.g, pearlPalette6.b,
                    pearlPalette7.r, pearlPalette7.g, pearlPalette7.b,
                });
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
                Unit.Affinity.FIRE,
                new float[] {
                    biggsHair.r, biggsHair.g, biggsHair.b,
                    biggsSkin.r, biggsSkin.g, biggsSkin.b,
                    biggsEyes.r, biggsEyes.g, biggsEyes.b,
                    biggsPalette4.r, biggsPalette4.g, biggsPalette4.b,
                    biggsPalette5.r, biggsPalette5.g, biggsPalette5.b,
                    biggsPalette6.r, biggsPalette6.g, biggsPalette6.b,
                    biggsPalette7.r, biggsPalette7.g, biggsPalette7.b,
                });
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
                Unit.Affinity.WATER,
                new float[] {
                    oceanHair.r, oceanHair.g, oceanHair.b,
                    oceanSkin.r, oceanSkin.g, oceanSkin.b,
                    oceanEyes.r, oceanEyes.g, oceanEyes.b,
                    oceanPalette4.r, oceanPalette4.g, oceanPalette4.b,
                    oceanPalette5.r, oceanPalette5.g, oceanPalette5.b,
                    oceanPalette6.r, oceanPalette6.g, oceanPalette6.b,
                    oceanPalette7.r, oceanPalette7.g, oceanPalette7.b,
                });
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
                Unit.Affinity.ANIMA,
                new float[] {
                    bismuthHair.r, bismuthHair.g, bismuthHair.b,
                    bismuthSkin.r, bismuthSkin.g, bismuthSkin.b,
                    bismuthEyes.r, bismuthEyes.g, bismuthEyes.b,
                    bismuthPalette4.r, bismuthPalette4.g, bismuthPalette4.b,
                    bismuthPalette5.r, bismuthPalette5.g, bismuthPalette5.b,
                    bismuthPalette6.r, bismuthPalette6.g, bismuthPalette6.b,
                    bismuthPalette7.r, bismuthPalette7.g, bismuthPalette7.b,
                });
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
            new EscapeObjective(), CHAPTER_TITLE, teamNames, 10,
            new string[] { "map-music-1", "enemyphase-music-1" },
            new string[] { "player-battle-music-1", "enemy-battle-music-1" });
        playerList = gridmap.player;

        setUnits(mapArray, enemy, Unit.UnitTeam.ENEMY, Quaternion.Euler(0, 180, 0));
        setUnits(mapArray, player, Unit.UnitTeam.PLAYER, Quaternion.identity);
        gridmap.initializeCursorPosition();

        return gridmap;

    }
}

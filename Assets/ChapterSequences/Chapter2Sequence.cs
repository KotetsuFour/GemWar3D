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

        loot = new int[] {0, 2, 0, -1,/*2 Steel*/0, 0, 0, 11,/*Iron Lance*/
            0, 0, 0, 24,/*Moon Goddess Icon*/0, 0, 0, 15,/*Iron Whip*/4, 0, 0, -1/*4 Iron*/};

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
                introScene.gameObject.SetActive(true);
                introScene.constructor(getMapIntro());
                seqMem = introScene;
            }
            else if (sequenceNum == 3)
            {
                seqMem.gameObject.SetActive(false);
                //                SaveMechanism.loadGame(StaticData.savefile);
                Debug.Log("before making prep");
                PreBattleMenu pbm = makePrepMenu();
                Debug.Log("after making prep");
                seqMem = pbm;
            }
            else if (sequenceNum == 4)
            {
                Debug.Log("before disabling prep");
                seqMem.gameObject.SetActive(false);
                Debug.Log("after disabling prep");
                seqMem = makeChapter();
                gridmap.gameObject.SetActive(true);
                seqMem = gridmap;
            }
            else if (sequenceNum == 5)
            {
                seqMem.gameObject.SetActive(false);
                playerList = gridmap.player;
                turnsTaken = gridmap.turn;
                finalScene.gameObject.SetActive(true);
                finalScene.constructor(getEnding());
                seqMem = finalScene;
            }
            else if (sequenceNum == 6)
            {
                seqMem.gameObject.SetActive(false);
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
        string blue = "Blue_Diamond null ";
        string yellow = "Yellow_Diamond null ";
        string pink = "Pink_Diamond Pink_Diamond ";
        string narrator = "_ null ";
        string pearl = "Pearl Pearl ";
        return new string[]
        {
            narrator + "The Crystal Gems, led by Rose Quartz, began sabotaging the colonial effort.",
            narrator + "With this, Pink Diamond had the perfect excuse to end the colony. Or so she thought.",

            pink + "Rose Quartz has become a significant threat. She has turned several of my Gems against me.",
            yellow + "Well I think the solution is simple.",
            pink + "We should stop the colony!",
            yellow + "I was going to say you ought to bubble all of your Rose Quartzes.",
            yellow + "They are a new model of quartz. For all we know, they could all be just as defective as this one.",
            pink + "But that isn't enough!",
            yellow + "Capturing the other Rose Quartzes will work as a scare tactic.",
            yellow + "And if not, it should be easy enough still to hunt down these few traitors.",
            pink + "You don't understand! They've organized into a rebellion! They're calling themselves the Crystal Gems!",
            yellow + "Ugh. Blue, you handle this.",
            pink + "I don't understand why you won't just let me leave the Earth alone!",
            blue + "\"Leave the Earth alone\" ? Is that what you're asking of us?",
            pink + "It's just--",
            blue + "But this is what you wanted.",
            blue + "You begged us for a colony of your own.And now all you want to do is be rid of it!",
            blue + "First, there were too many organics, then their cities were too difficult to dismantle.",
            blue + "And now these \"Crystal Gems\" ? We're tired of your excuses, Pink!",
            pink + "...",
            blue + "This Rose Quartz can't hurt you. You can't be swayed by a few unruly Gems.",
            pink + "---",
            blue + "Enough!",
            blue + "You must understand. You are a Diamond. Everyone on this planet is looking to you.",
            blue + "You don't even have to do anything. Just smile and wave.",
            blue + "Show everyone you are unfazed by this little uprising.",
            blue + "Your Gems will fall into line, and these Crystal Gems will be no more.",
            blue + "As long as you are there to rule, this colony will be completed.",

            pink + "Pearl? What are you doing with that sword?",
            pearl + "Bringing it to you, my Diamond! It's a new sword that Bismuth made especially for you.",
            pearl + "She says it's not balanced entirely correctly and that she'll make another one to replace it.",
            pink + "That's not how you give someone a sword. I thought you were about to shatter me.",
            pearl + "Oh! I'm terribly sorry!",
            pink + "They didn't listen, Pearl. As usual. Looks like the war will go on.",
            pink + "Thanks for bringing me the sword. Let's go find somewhere to use it."
        };
    }
    private string[] getMapIntro()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string bismuth = "Bismuth Bismuth ";
        string ruby = "Ruby Ruby ";
        return new string[]
        {
            rose + "This is perfect! Once we capture the Lunar Sea Spire, Blue and Yellow will have to leave the Earth alone.",
            bismuth + "And Pink Diamond, right? This is her colony.",
            rose + "Right! Obviously.",
            rose + "But how are we going to approach? If we just come in the main entrance, we'll be seen from a mile away.",
            pearl + "Wait a minute...",
            pearl + "Halt! Who goes there?!",
            ruby + "---",
            ruby + "Whoa!",
            rose + "A Ruby? What are you doing here?",
            ruby + "I was just guarding the private entrance-- Wait, that's classified! If you ask me, I should be the one asking you what you're doing here.",
            ruby + "And it seems like what your doing here is being up to no good!",
            ruby + "I just caught you red-handed! Ha!",
            bismuth + "You know you're outnumbered, right? And we're the ones who just caught you.",
            ruby + "...",
            ruby + "Dang it! How am I supposed to explain to my Turquiose that I got captured?! This is humiliating!",
            rose + "You know, if you were to help us get in the private entrance and capture the spire, I suppose we could let you go.",
            rose + "Your Turquoise wouldn't even have to know.",
            ruby + "Hm... I guess I have no choice. Okay, I'll do it.",
            ruby + "But if you see my Turquoise in there, tell her I had nothing to do with this."
        };
    }
    private string[] turquoiseRecruitment()
    {
        string turq = "Turquoise Turquoise ";
        string note = "_ null ";
        return new string[]
        {
            turq + "Did I see a Ruby fighting with you? With her Gem on her foot and using a pike?",
            turq + "Ugh, what does she think she's doing?!",
            turq + "Well I can't let word get out that I can't keep control over my servants....",
            turq + "Very well. I'm joining you! At least that way, everyone will know that I am in complete control over my life and what is mine!",
            turq + "You may thank me for the privilege of having me on your team when we get out of this fiasco.",
            note + "Turquoise joined your party!"
        };
    }
    private string[] moonstoneRecruitment()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string moon = "Moonstone Moonstone ";
        string note = "_ null ";
        return new string[]
        {
            rose + "Hi there.",
            moon + "Huh?",
            rose + "Sorry to catch you offguard, but I just had the sudden impulse to talk to you.",
            moon + "Why me? I'm just a nameless priestess for the Moon Goddess.",
            moon + "I'm invisible.",
            rose + "Is that how Homeworld has made you feel? Invisible?",
            moon + "I--",
            rose + "This might sound crazy, but maybe it's time you fought for someone who valued you as more than just a nameless priestess.",
            moon + "You know... I think you may be right. I'll do it! I'll join you-- er-- if you'll have me.",
            rose + "I would love to have you onboard, Moonstone!",
            note + "Moonstone joined your party!"
        };
    }
    private string[] getEnding()
    {
        string rose = "Rose_Quartz Rose_Quartz ";
        string pearl = "Pearl Pearl ";
        string turq = "Turquoise Turquoise ";
        string ruby = "Ruby Ruby ";
        string pink = "Pink_Diamond Pink_Diamond ";
        string blue = "Blue_Diamond null ";
        string yellow = "Yellow_Diamond null ";
        return new string[]
        {
            rose + "Now that the spire is in Crystal Gem hands, all we have to do is wait.",
            "$if alive Ruby",
            "$putString ruby",
            ruby + "That was really fun! I never get to fight real opponents with Turquoise!",
            ruby + "Do you mind if I come along with you guys?",
            rose + "You can absolutely come with us! We'll need Gems like you.",
            "$if alive Turquoise",
            turq + "Ahem...",
            ruby + "Turquoise! I can explain! There was a tsunami! And if I hadn't--",
            turq + "Save it. I can see you're quite taken with this rabble, so I have no choice.",
            turq + "I'm seeing this war through to the end! You clearly need the help anyway.",
            "$endif",
            "$endif",
            pearl + "Rose, are you sure this will be enough?",
            rose + "It has to be. Or we have a long road ahead of us.",

            blue + "*sigh*",
            blue + "I have to admit, I am disappointed in you, Pink.",
            blue + "All of these resources spent, all of this time scouting the landscape and examining the planet's conditions...",
            blue + "We did it because we truly believed in you, Pink.We believed that you could run your own colony.",
            pink + "I'm sorry I let you down. I really did want this, but...",
            blue + "No \"but\"s. You may have lost faith in yourself, but I haven't lost faith in you.",
            blue + "We're Diamonds. When we start something, we finish it, and we don't give up.",
            yellow + "Especially when a lower Gem challenges our authority.",
            yellow + "For the record, when this mess is cleaned up, I won't be trusting you with a new colony for at least another million years.",
            blue + "You can do this, Pink.You just need some assistance.",
            pink + "What are you saying?",
            blue + "I will bring my court to Earth and we will help you deal with these rebels.",
            yellow + "It will take some time for me to catch a free moment, but I intend to do the same.",
            yellow + "And when I get there, you'd better have all of the Rose Quartzes you can locate bubbled.",
            pink + "Blue, Yellow, I really don't think that's necessary.",
            yellow + "Oh, what's this? You don't want us to embarrass you by showing you how simple a matter this is?",
            yellow + "It's too late. You've had your chance to handle this. Now it's ours."
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
        noble.recruitability = 10;
        noble.recruitQuote = new string[]
        {
            "There's something... noble about your cause."
        };
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
        priest.recruitability = 10;
        priest.recruitQuote = new string[]
        {
            "May the Moon Goddess bring you victory."
        };
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
        moon.recruitability = 100;
        moon.recruitQuote = new string[]
        {
            "You want me to join you? Me?",
            "No one's ever chosen me for something before..."
        };
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
        turq.recruitability = 100;
        turq.recruitQuote = new string[]
        {
            "Ugh, fine!",
            "If you're not going to return my Ruby into my service, I have no choice but to join you."
        };

        return turq;
    }
    private PreBattleMenu makePrepMenu()
    {
        if (StaticData.chapterPrep != StaticData.scene)
        {
            Unit ruby_unit = ruby();
            StaticData.members.Add(ruby_unit);

            StaticData.addToConvoy(Item.iron_blade.clone());
            StaticData.positions = new int[] { 0, -1, -1, -1, -1, -1 };

            StaticData.chapterPrep = StaticData.scene;
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
            new string[] { "map-music-1", "enemyphase-music-1" },
            new string[] { "player-battle-music-1", "enemy-battle-music-1" });

        setUnits(map, player, Unit.UnitTeam.PLAYER, Quaternion.Euler(0, 180, 0));
        setUnits(map, enemy, Unit.UnitTeam.ENEMY, Quaternion.identity);

        gridmap.initializeCursorPosition();

        return gridmap;
    }

}

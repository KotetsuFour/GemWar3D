using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : SequenceMember
{
    private int width;
    private int height;

    public bool testingMode;

    public List<Unit> player;
    public List<Unit> enemy;
    public List<Unit> ally;
    public List<Unit> other;
    private string[] teamNames;

    private Objective objective;
    public bool seized;
    private string chapterName;
    private bool objectiveComplete;

    public int turn;
    private int turnPar;

    private Tile tile;

    private Tile[,] map;
    private List<Tile> healTiles;

    private float timer;

    public GameObject cursor;
    public GameObject cam;
    public GameObject menuBackground;
    public GameObject instantiatedMenuBackground;
    public float menuSizeX;
    public List<MenuChoice> menuElements;
    public int menuIdx;
    public GameObject forecastBackground;
    public GameObject instantiatedForecastBackground;
    public float forecastSize;

    public int cursorX;
    public int cursorY;

    public SelectionMode selectionMode = SelectionMode.ROAM;

    public Dictionary<Tile, object> traversableTiles;
    public Dictionary<Tile, object> attackableTiles;
    public Tile selectedTile;
    public Unit selectedUnit;
    public Tile moveDest;
    public List<Tile> interactableUnits;
    public int interactIdx;
    public Tile targetTile;
    public Unit targetEnemy;

    public GameObject battleBackground;
    public GameObject instantiatedBattleBackground;
    public GameObject battleUI;
    public GameObject instantiatedBattleUI;
    public GameObject battleTile;
    public GameObject battleEnemyTile;
    public GameObject battlePlayerTile;
    public GameObject battleCombatant;
    public GameObject battleCombatantEnemy;
    public GameObject battleCombatantPlayer;
    public float battleMoveSpeed; //Set in UI

//    private List<AttackComponent> attackParts;
    private int attackMode; //0 = attack, 1 = return
    private int attackPartIdx;
    private bool keepBattling;

    private int enemyIdx;
    private Unit currentEnemy;
    private object[] enemyAction;
    private Tile enemyStartTile;
    private Tile enemyDestTile;
    private float moveTime;

    private int allyIdx;
    private Unit currentAlly;
    private object[] allyAction;
    private Tile allyStartTile;
    private Tile allyDestTile;

    public GameObject dialogueBox;
    private GameObject instantiatedDialogueBox;
    public GameObject speakerPortrait;
    private List<GameObject> instantiatedSpeakerPortraits;
//    private DialogueEvent currentDialogue;
    private Tile talkerTile;

    public GameObject statsPageBackground;
    private GameObject instantiatedStatsPageBackground;

    private Weapon brokenWeapon;
    private bool showingBrokenWeapon;
    private bool showingEXP;
    private bool showingLevelUp;
    private bool findBattleEndStart;
    private int expToAdd;
    private Unit expUnit;
    private bool[] levelGrowths;
    public GameObject breakAndExpPane;
    private GameObject instantiatedBreakAndExpPane;
    public GameObject levelUpBackground;
    private GameObject instantiatedLevelUpBackground;

    public void constructor(Tile[,] map,
        Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits, Unit[] otherUnits,
        Objective objective, string chapterName, string[] teamNames, int turnPar)
    {
        this.map = map;

        player = new List<Unit>(playerUnits);
        enemy = new List<Unit>(enemyUnits);
        ally = new List<Unit>(allyUnits);
        other = new List<Unit>(otherUnits);

        this.objective = objective;
        this.chapterName = chapterName;
        this.teamNames = teamNames;
        this.turnPar = turnPar;
    }

    public override bool completed()
    {
        return objectiveComplete;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public enum SelectionMode
    {
        ROAM, MOVE, MENU, SELECT_ENEMY, SELECT_TALKER, SELECT_WEAPON, FORECAST, BATTLE, MAP_MENU, IN_CONVO,
        SELECT_GEM, STATUS, STATS_PAGE, CONTROLS, NOTIFICATION, ITEM_MENU, SELECT_TRADER, SELECT_WEAPON_TRADER,
        ENEMYPHASE_SELECT_UNIT, ENEMYPHASE_MOVE, ENEMYPHASE_ATTACK, ENEMYPHASE_BURN, ENEMYPHASE_COMBAT_PAUSE,
        ALLYPHASE_SELECT_UNIT, ALLYPHASE_MOVE, ALLYPHASE_ATTACK, ALLYPHASE_BURN, ALLYPHASE_COMBAT_PAUSE,
        STANDBY, GAMEOVER, ESCAPE_MENU
    }

    public enum MenuChoice
    {
        TALK, ATTACK, ESCAPE, SEIZE, ITEM, WEAPON, GEM, CHEST, WAIT, STATUS, CONTROLS, END,
        USE_PERSONAL, USE_HELD, TRADE, DROP, EQUIP_PERSONAL, EQUIP_HELD, EQUIP_NONE, TRADE_WEAPON,
        DROP_WEAPON
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;

[RequireComponent (typeof(NavMeshSurface))]
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

    private Tile[,] map;
    private Transform mapTransform;
    private List<Tile> healTiles;

    private float timer;

    [SerializeField] private GameObject cursor;

    [SerializeField] private Button menuOption;
    private List<MenuChoice> menuElements;

    public int cursorX;
    public int cursorY;

    private SelectionMode selectionMode;

    public Dictionary<Tile, object> traversableTiles;
    public Dictionary<Tile, object> attackableTiles;
    public Tile selectedTile;
    public Unit selectedUnit;
    public Tile moveDest;
    public List<Tile> interactableUnits;
    public int interactIdx;
    public Tile targetTile;
    public Unit targetEnemy;

    [SerializeField] private float battleMoveSpeed;

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

    private Tile talkerTile;

    public void constructor(Tile[,] map,
        Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits, Unit[] otherUnits,
        Objective objective, string chapterName, string[] teamNames, int turnPar)
    {
        this.map = map;
        width = map.GetLength(0);
        height = map.GetLength(1);

        GetComponent<NavMeshSurface>().BuildNavMesh();

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

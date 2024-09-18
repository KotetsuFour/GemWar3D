using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;

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
    [SerializeField] private LayerMask unitOrTile;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask tileLayer;

    [SerializeField] private Button menuOption;
    private List<MenuChoice> menuElements;

    public int cursorX;
    public int cursorY;

    private SelectionMode selectionMode;

    private Dictionary<Tile, object> traversableTiles;
    private Dictionary<Tile, object> attackableTiles;
    private Tile selectedTile;
    private Unit selectedUnit;
    private Tile moveDest;
    private List<Tile> interactableUnits;
    private int interactIdx;
    private Tile targetTile;
    private Unit targetEnemy;

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

    private void setCursor(int x, int y)
    {
        cursorX = Mathf.Clamp(x, 0, width - 1);
        cursorY = Mathf.Clamp(y, 0, height - 1);
        cursor.transform.position = new Vector3(cursorX, map[x, y].height * Tile.TILE_HEIGHT_MULTIPLIER, cursorY);
    }
    private void setCursor(Tile tile)
    {
        setCursor(tile.x, tile.y);
    }
    private void moveCursor(int xDirection, int yDirection)
    {
        setCursor(cursorX + xDirection, cursorY + yDirection);
    }

    public override void LEFT_MOUSE()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            RaycastHit hit;
            if (Physics.Raycast(getCamera().ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, unitOrTile))
            {
                if (hit.collider.GetComponent<UnitModel>() != null)
                {
                    selectedTile = hit.collider.GetComponent<UnitModel>().getTile();
                    selectedUnit = hit.collider.GetComponent<UnitModel>().getUnit();
                }
                else if (hit.collider.GetComponent<Tile>() != null)
                {
                    selectedTile = hit.collider.GetComponent<Tile>();
                    if (selectedTile.getOccupant() != null)
                    {
                        selectedUnit = selectedTile.getOccupant().getUnit();
                    }
                    else
                    {
                        selectedUnit = null;
                    }
                }
                setCursor(selectedTile);

                if (selectedUnit != null)
                {
                    initiateMove();
                }
            }
            else
            {
                selectedTile = null;
                selectedUnit = null;
            }
        }
        else if (selectionMode == SelectionMode.MOVE)
        {
            RaycastHit hit;
            if (selectedUnit.team == Unit.UnitTeam.PLAYER
                && Physics.Raycast(getCamera().ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, tileLayer))
            {
                if (traversableTiles.ContainsKey(hit.collider.GetComponent<Tile>())
                    && hit.collider.GetComponent<Tile>().getOccupant() == null)
                {
                    moveDest = hit.collider.GetComponent<Tile>();
                    initiateTravel();
                }
            }
            else
            {
                selectionMode = SelectionMode.ROAM;
                selectedTile = null;
                selectedUnit = null;
                unfillTraversableTiles();
            }
        }

    }

    private void initiateMove()
    {
        selectedUnit.model.cancelMovement();
        fillTraversableTiles(selectedUnit, cursorX, cursorY);

        selectionMode = SelectionMode.MOVE;
    }
    private void initiateTravel()
    {
        Vector3[] path = getPath();
        selectedUnit.model.setPath(path);
        unfillTraversableTiles();

        selectionMode = SelectionMode.TRAVEL;
    }
    private void initiateMenu()
    {
        //TODO

        selectionMode = SelectionMode.MENU;
    }

    private void fillTraversableTiles(Unit u, int x, int y)
    {
        traversableTiles = getTraversableTiles(u, x, y);
        attackableTiles = getAttackableTiles(traversableTiles, u);
        foreach (Tile t in traversableTiles.Keys)
        {
            t.highlightMove();
        }
        foreach (Tile t in attackableTiles.Keys)
        {
            if (!traversableTiles.ContainsKey(t))
            {
                t.highlightAttack();
            }
        }
    }

    private void unfillTraversableTiles()
    {
        foreach (Tile t in traversableTiles.Keys)
        {
            t.unhighlight();
        }
        foreach (Tile t in attackableTiles.Keys)
        {
            t.unhighlight();
        }
    }

    public Dictionary<Tile, object> getTraversableTiles(Unit u, int x, int y)
    {
        //TODO
        Dictionary<Tile, object> traversable = new Dictionary<Tile, object>();
        LinkedList<int[]> searchList = new LinkedList<int[]>();
        searchList.AddFirst(new int[] { x, y, u.movement });
        int[] dimensions = new int[] { width, height };
        while (searchList.Count != 0)
        {
            int[] from = searchList.First.Value;
            searchList.RemoveFirst();
            Tile fromTile = map[from[0], from[1]];
            addKeyAndValue(traversable, fromTile, from[2]);
            if (from[2] == 0)
            {
                continue;
            }
            if (from[0] > 0)
            {
                int checkX = from[0] - 1;
                int checkY = from[1];
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)outInt < from[2])
                        && from[2] - check.getMoveCost(u) >= 0
                        && (check.isVacant()
                                || enemy.Contains(check.getOccupant().getUnit()) == enemy.Contains(u)))
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - check.getMoveCost(u) });
                }
            }
            if (from[0] < dimensions[0] - 1)
            {
                int checkX = from[0] + 1;
                int checkY = from[1];
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)outInt < from[2])
                        && from[2] - check.getMoveCost(u) >= 0
                        && (check.isVacant()
                                || enemy.Contains(check.getOccupant().getUnit()) == enemy.Contains(u)))
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - check.getMoveCost(u) });
                }
            }
            if (from[1] > 0)
            {
                int checkX = from[0];
                int checkY = from[1] - 1;
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)outInt < from[2])
                        && from[2] - check.getMoveCost(u) >= 0
                        && (check.isVacant()
                                || enemy.Contains(check.getOccupant().getUnit()) == enemy.Contains(u)))
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - check.getMoveCost(u) });
                }
            }
            if (from[1] < dimensions[1] - 1)
            {
                int checkX = from[0];
                int checkY = from[1] + 1;
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)outInt < from[2])
                        && from[2] - check.getMoveCost(u) >= 0
                        && (check.isVacant()
                                || enemy.Contains(check.getOccupant().getUnit()) == enemy.Contains(u)))
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - check.getMoveCost(u) });
                }
            }
        }
        return traversable;
    }
    public Dictionary<Tile, object> getAttackableTiles(Dictionary<Tile, object> traversable, Unit selected)
    {
        Dictionary<Tile, object> ret = new Dictionary<Tile, object>();
        Dictionary<Tile, object>.KeyCollection keys = traversable.Keys;
        foreach (Tile t in keys)
        {
            Dictionary<Tile, object> att = getAttackableBattlegroundTilesFromDestination(selected, t);
            Dictionary<Tile, object>.KeyCollection check = att.Keys;
            foreach (Tile c in check)
            {
                object outInt;
                if (!ret.TryGetValue(c, out outInt) || (int)ret[c] > (int)att[c])
                {
                    addKeyAndValue(ret, c, att[c]);
                }
            }
        }
        return ret;
    }

    public Dictionary<Tile, object> getAttackableBattlegroundTilesFromDestination(Unit u, Tile dest)
    {
        int x = dest.x;
        int y = dest.y;
        int minRange = 1;
        int maxRange = 1;
        Weapon w = u.getEquippedWeapon();
        if (w != null)
        {
            minRange = w.minRange;
            maxRange = w.maxRange;
        }
        //TODO find actual range
        Dictionary<Tile, object> traversable = new Dictionary<Tile, object>();
        Dictionary<Tile, object> attackable = new Dictionary<Tile, object>(); //Gives distance from attacker for each target
        LinkedList<int[]> searchList = new LinkedList<int[]>(); //[0] = x, [1] = y, [2] = remainingMovement
        searchList.AddFirst(new int[] { x, y, maxRange });
        int[] dimensions = new int[] { width, height };
        while (searchList.Count != 0)
        {
            int[] from = searchList.First.Value; searchList.RemoveFirst();
            Tile fromTile = map[from[0], from[1]];
            addKeyAndValue(traversable, fromTile, from[2]);
            int distance = maxRange - from[2];
            if (distance >= minRange
                    && (fromTile.getOccupant() == null ||
                    enemy.Contains(fromTile.getOccupant().getUnit()) != enemy.Contains(u)))
            {
                addKeyAndValue(attackable, fromTile, distance);
            }
            if (from[2] == 0)
            {
                continue;
            }
            if (from[0] > 0)
            {
                int checkX = from[0] - 1;
                int checkY = from[1];
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)traversable[check] < from[2])
                        && from[2] - 1 >= 0)
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - 1 });
                }
            }
            if (from[0] < dimensions[0] - 1)
            {
                int checkX = from[0] + 1;
                int checkY = from[1];
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)traversable[check] < from[2])
                        && from[2] - 1 >= 0)
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - 1 });
                }
            }
            if (from[1] > 0)
            {
                int checkX = from[0];
                int checkY = from[1] - 1;
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)traversable[check] < from[2])
                        && from[2] - 1 >= 0)
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - 1 });
                }
            }
            if (from[1] < dimensions[1] - 1)
            {
                int checkX = from[0];
                int checkY = from[1] + 1;
                Tile check = map[checkX, checkY];
                object outInt;
                if ((!traversable.TryGetValue(check, out outInt) || (int)traversable[check] < from[2])
                        && from[2] - 1 >= 0)
                {
                    searchList.AddLast(new int[] { checkX, checkY, from[2] - 1 });
                }
            }
        }
        return attackable;
    }

    public List<Tile> getTalkableTiles(Tile dest, Unit selected)
    {
        List<Tile> ret = new List<Tile>();
        int x = dest.x;
        int y = dest.y;
        if (x > 0)
        {
            Tile t = map[x - 1, y];
            Unit u = t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (x < width - 1)
        {
            Tile t = map[x + 1, y];
            Unit u = t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (y > 0)
        {
            Tile t = map[x, y - 1];
            Unit u = t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (y < height - 1)
        {
            Tile t = map[x, y + 1];
            Unit u = t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        return ret;
    }

    public static List<Tile> getAttackableTilesWithEnemies(Dictionary<Tile, object> attackable,
        Unit u)
    {
        // TODO Auto-generated method stub
        List<Tile> ret = new List<Tile>();
        Dictionary<Tile, object>.KeyCollection keys = attackable.Keys;
        foreach (Tile t in keys)
        {
            if (t.getOccupant() != null)
            {
                ret.Add(t);
            }
        }
        return ret;
    }

    public List<Tile> getAdjacentTilesWithAllies(Tile here, Unit unit)
    {
        List<Tile> ret = new List<Tile>();
        if (here.x > 0)
        {
            Tile attempt = map[here.x - 1, here.y];
            if (attempt.getOccupant() != null && attempt.getOccupant().getUnit() != unit && attempt.getOccupant().getUnit().team == unit.team)
            {
                ret.Add(attempt);
            }
        }
        if (here.x < width - 1)
        {
            Tile attempt = map[here.x + 1, here.y];
            if (attempt.getOccupant() != null && attempt.getOccupant().getUnit() != unit && attempt.getOccupant().getUnit().team == unit.team)
            {
                ret.Add(attempt);
            }
        }
        if (here.y > 0)
        {
            Tile attempt = map[here.x, here.y - 1];
            if (attempt.getOccupant() != null && attempt.getOccupant().getUnit() != unit && attempt.getOccupant().getUnit().team == unit.team)
            {
                ret.Add(attempt);
            }
        }
        if (here.y < height - 1)
        {
            Tile attempt = map[here.x, here.y + 1];
            if (attempt.getOccupant() != null && attempt.getOccupant().getUnit() != unit && attempt.getOccupant().getUnit().team == unit.team)
            {
                ret.Add(attempt);
            }
        }
        return ret;
    }

    private void fillAttackableTiles()
    {
        foreach (Tile t in interactableUnits)
        {
            t.unhighlight();
        }
        Dictionary<Tile, object> attackable = getAttackableBattlegroundTilesFromDestination(selectedUnit, moveDest);
        interactableUnits.Clear();
        foreach (Tile t in attackable.Keys)
        {
            t.highlightAttack();
            interactableUnits.Add(t);
        }
    }
    private void unfillAttackableTiles()
    {
        foreach (Tile t in interactableUnits)
        {
            t.unhighlight();
        }
        interactableUnits.Clear();
    }



    private void addKeyAndValue(Dictionary<Tile, object> dict, Tile key, object val)
    {
        if (dict.ContainsKey(key))
        {
            dict.Remove(key);
        }
        dict.Add(key, val);
    }

    private Vector3[] getPath()
    {
        //TODO use A Star

        return new Vector3[] { moveDest.getStage().position };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionMode == SelectionMode.TRAVEL && selectedUnit.model.reachedDestination())
        {
            initiateMenu();
        }
    }


    public enum SelectionMode
    {
        ROAM, MOVE, TRAVEL, MENU, SELECT_ENEMY, SELECT_TALKER, SELECT_WEAPON, FORECAST, BATTLE, MAP_MENU, IN_CONVO,
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

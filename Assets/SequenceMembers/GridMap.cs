using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridMap : SequenceMember
{
    private int width;
    private int height;

    [SerializeField] private bool testingMode;
    [SerializeField] private bool testFutureVision;

    public List<Unit> player;
    public List<Unit> enemy;
    public List<Unit> ally;
    public List<Unit> other;
    private string[] teamNames;
    private int teamPhase;

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

    [SerializeField] private GameObject cursorPrefab;
    private GameObject cursor;
    [SerializeField] private LayerMask unitOrTile;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask tileLayer;

    [SerializeField] private Button menuOption;
    private List<Button> menuOptions;
    private List<MenuChoice> menuElements;
    private int menuIdx;
    private int specialMenuIdx;

    public int cursorX;
    public int cursorY;

    [SerializeField] private float cameraDistance;
    private int camOrientation;

    private SelectionMode selectionMode;

    [SerializeField] private BattleAnimation battleAnimation;
    private BattleAnimation instantiatedBattleAnimation;
    [SerializeField] private MapEventExecutor mapEvent;
    private MapEventExecutor instantiatedMapEvent;
    [SerializeField] private ParticleAnimation warpAnimation;

    private Dictionary<Tile, object> traversableTiles;
    private Dictionary<Tile, object> attackableTiles;
    private Tile selectedTile;
    private Unit selectedUnit;
    private Tile moveDest;
    private List<Tile> interactableUnits;
    private int interactIdx;
    private Tile targetTile;
    private Unit targetEnemy;
    private Unit statsUnit;

    private int npcIdx;
    private object[] npcAction;

    private Tile talkerTile;

    private AudioSource music;
    private string[] teamMusic;
    private string[] battleMusic;

    public static int NUM_CAMERA_POSITIONS = 5;

    public void constructor(Tile[,] map,
        Unit[] playerUnits, Unit[] enemyUnits, Unit[] allyUnits, Unit[] otherUnits,
        Objective objective, string chapterName, string[] teamNames, int turnPar,
        string[] teamMusic, string[] battleMusic)
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
        turn = 1;

        StaticData.findDeepChild(transform, "HUDObjective").GetComponent<TextMeshProUGUI>()
            .text = "Objective: " + objective.getName(this);

        menuOptions = new List<Button>();

        interactableUnits = new List<Tile>();
        cursor = Instantiate(cursorPrefab);

        this.teamMusic = teamMusic;
        this.battleMusic = battleMusic;

        teamPhase = -1;
        nextTeamPhase();
    }
    public void initializeCursorPosition()
    {
        Tile tile = player[0].model.getTile();
        setCursor(tile);
        setCameraPosition();
    }

    public override bool completed()
    {
        return objectiveComplete;
    }
    private void enableChild(string hudName, bool enable)
    {
        StaticData.findDeepChild(transform, hudName).gameObject.SetActive(enable);
    }

    public GameObject getCursor()
    {
        return cursor;
    }
    private void setCursor(int x, int y)
    {
        cursorX = Mathf.Clamp(x, 0, width - 1);
        cursorY = Mathf.Clamp(y, 0, height - 1);
        cursor.transform.position = new Vector3(cursorX, map[cursorX, cursorY].getCursorPosition().y, cursorY);

        Tile tile = map[cursorX, cursorY];
        StaticData.findDeepChild(transform, "TileName").GetComponent<TextMeshProUGUI>()
            .text = tile.getName();
        StaticData.findDeepChild(transform, "TileAVO").GetComponent<TextMeshProUGUI>()
            .text = "" + tile.getAvoidBonus();
        if (tile.getOccupant() == null)
        {
            enableChild("UnitHere", false);
        }
        else
        {
            Unit unitHere = tile.getOccupant().getUnit();
            enableChild("UnitHere", true);
            StaticData.findDeepChild(transform, "UnitHerePortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(unitHere.unitName);
            StaticData.findDeepChild(transform, "UnitHereName").GetComponent<TextMeshProUGUI>()
                .text = unitHere.unitName;
            StaticData.findDeepChild(transform, "UnitHereHP").GetComponent<TextMeshProUGUI>()
                .text = $"{unitHere.currentHP}/{unitHere.maxHP}";
            if (unitHere.getEquippedWeapon() == null)
            {
                enableChild("UnitHereWeaponImage", false);
            }
            else
            {
                Weapon wep = unitHere.getEquippedWeapon();
                enableChild("UnitHereWeaponImage", true);
                StaticData.findDeepChild(transform, "UnitHereWeaponImage").GetComponent<Image>()
                    .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(wep.weaponType));
                StaticData.findDeepChild(transform, "UnitHereWeaponName").GetComponent<TextMeshProUGUI>()
                    .text = wep.itemName;
            }
        }
    }
    private void setCursor(Tile tile)
    {
        setCursor(tile.x, tile.y);
    }
    private void moveCursor(int xDirection, int yDirection)
    {
        int[] actualDirection = transformCoordsBasedOnCamera(xDirection, yDirection);
        playOneTimeSound(AssetDictionary.getAudio("tile"));

        setCursor(cursorX + actualDirection[0], cursorY + actualDirection[1]);

        setCameraPosition();
    }
    private void setCameraPosition()
    {
        Vector3 pos = cursor.transform.position;
        if (camOrientation == 4)
        {
            pos += new Vector3(0, 1.5f, 0);
        }
        else
        {
            int[] displacement = transformCoordsBasedOnCamera(0, -1);
            pos += new Vector3(displacement[0], 1, displacement[1]);
        }
        Vector3 newPos = cursor.transform.position - ((cursor.transform.position - pos).normalized * cameraDistance);
        getCamera().transform.position = newPos;
        getCamera().transform.rotation = Quaternion.LookRotation(cursor.transform.position - getCamera().transform.position);
    }
    private int[] transformCoordsBasedOnCamera(int x, int y)
    {
        if (camOrientation == 0 || camOrientation == 4)
        {
            return new int[] { x, y };
        }
        else if (camOrientation == 1)
        {
            return new int[] { y, -x };
        }
        else if (camOrientation == 2)
        {
            return new int[] { -x, -y };
        }
        else if (camOrientation == 3)
        {
            return new int[] { -y, x };
        }
        return null;
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
                    playOneTimeSound(AssetDictionary.getAudio("select"));

                    initiateMove(hit.collider.GetComponent<UnitModel>().getTile());
                }
                else if (hit.collider.GetComponent<Tile>() != null)
                {
                    playOneTimeSound(AssetDictionary.getAudio("select"));

                    Tile currentTile = hit.collider.GetComponent<Tile>();
                    initiateMove(currentTile);
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
                initiateTravel(hit.collider.GetComponent<Tile>());
            }
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            Tile tile = null;
            RaycastHit hit;
            if (Physics.Raycast(getCamera().ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, unitLayer))
            {
                tile = hit.collider.GetComponent<UnitModel>().getTile();
            }
            else if (Physics.Raycast(getCamera().ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, tileLayer))
            {
                tile = hit.collider.GetComponent<Tile>();
            }
            if (tile != null && interactableUnits.Contains(tile))
            {
                interactIdx = interactableUnits.IndexOf(tile);
                setCursor(tile);
                Z();
            }
        }
        else if (selectionMode == SelectionMode.FORECAST)
        {
            Z();
        }
        else if (selectionMode == SelectionMode.IN_CONVO)
        {
            Z();
        }
    }
    public override void RIGHT_MOUSE()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            initiateMapMenu();
        }
        else
        {
            X();
        }
    }

    public override void Z()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            Tile currentTile = map[cursorX, cursorY];
            if (currentTile.getOccupant() == null)
            {
                playOneTimeSound(AssetDictionary.getAudio("select"));

                initiateMapMenu();
            }
            else
            {
                playOneTimeSound(AssetDictionary.getAudio("select"));

                initiateMove(currentTile);
            }
        }
        else if (selectionMode == SelectionMode.MOVE)
        {
            Tile currentTile = map[cursorX, cursorY];
            initiateTravel(currentTile);
        }
        else if (selectionMode == SelectionMode.MENU || selectionMode == SelectionMode.MAP_MENU
            || selectionMode == SelectionMode.ITEM_MENU || selectionMode == SelectionMode.SELECT_WEAPON
            || selectionMode == SelectionMode.SELECT_GEM)
        {
            selectMenuOption(menuIdx);
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY)
        {
            createForecast();
        }
        else if (selectionMode == SelectionMode.FORECAST)
        {
            enableChild("HUD", false);
            finalizeMove();
            startBattle();
        }
        else if (selectionMode == SelectionMode.BATTLE || selectionMode == SelectionMode.ENEMYPHASE_ATTACK
            || selectionMode == SelectionMode.ALLYPHASE_ATTACK || selectionMode == SelectionMode.OTHERPHASE_ATTACK)
        {
            instantiatedBattleAnimation.levelUpOK();
        }
        else if (selectionMode == SelectionMode.SELECT_TALKER)
        {
            talkerTile = interactableUnits[interactIdx];

            enableChild("Menu", false);

            unfillAttackableTiles();
            finalizeMove();

            performTalk();
        }
        else if (selectionMode == SelectionMode.IN_CONVO)
        {
            instantiatedMapEvent.Z();
        }
        else if (selectionMode == SelectionMode.SELECT_TRADER)
        {
            Unit trader = interactableUnits[interactIdx].getOccupant().getUnit();

            enableChild("Menu", false);
            enableChild("HUD", false);

            unfillAttackableTiles();
            finalizeMove();

            tradeItem(trader);
        }
        else if (selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            Unit trader = interactableUnits[interactIdx].getOccupant().getUnit();

            enableChild("Menu", false);
            enableChild("HUD", false);

            unfillAttackableTiles();
            finalizeMove();

            tradeWeapon(trader);
        }
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            if (specialMenuIdx == 0)
            {
                restartChapter();
            }
            else if (specialMenuIdx == 1)
            {
                mainMenu();
            }
        }
    }

    public override void X()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            initiateMapMenu();
        }
        else if (selectionMode == SelectionMode.MOVE)
        {
            cancelMove();
        }
        else if (selectionMode == SelectionMode.TRAVEL)
        {
            cancelTravel(false);
        }
        else if (selectionMode == SelectionMode.MENU)
        {
            cancelMenu(false);
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            cancelOtherUnitSelection();
        }
        else if (selectionMode == SelectionMode.SELECT_WEAPON || selectionMode == SelectionMode.SELECT_GEM
            || selectionMode == SelectionMode.ITEM_MENU)
        {
            initiateMenu();
        }
        else if (selectionMode == SelectionMode.FORECAST)
        {
            enableChild("Forecast", false);
            enableChild("FutureVision", false);
            enableChild("HUD", true);

            getAttackOptions();
        }
        else if (selectionMode == SelectionMode.MAP_MENU)
        {
            enableChild("Menu", false);
            enableChild("HUD", true);

            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            enableChild("StatsPage", false);
            enableChild("HUD", true);

            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.CONTROLS)
        {
            /*
            controlsPage.SetActive(false);
            Camera camCam = cam.GetComponent<Camera>();
            camCam.orthographicSize = cameraOrthographicSize;
            setCameraPosition(cursorX, cursorY);
            instantiatedMapHUD.SetActive(true);
            */
            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.STATUS)
        {
            enableChild("Status", false);
            enableChild("Menu", false);
            enableChild("HUD", true);
            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.ESCAPE_MENU)
        {
            enableChild("EscapeMenu", false);
            selectionMode = SelectionMode.ROAM;
        }
    }
    public override void A()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            if (map[cursorX, cursorY].getOccupant() != null)
            {
                statsPage(map[cursorX, cursorY].getOccupant().getUnit());
                enableChild("HUD", false);
                playOneTimeSound(AssetDictionary.getAudio("select"));

                selectionMode = SelectionMode.STATS_PAGE;
            }
        }
    }
    public override void S()
    {
        if (map[cursorX, cursorY].getOccupant() != null)
        {
            UnitModel unitModel = map[cursorX, cursorY].getOccupant();
            List<Unit> allies = null;
            if (unitModel.getUnit().team == Unit.UnitTeam.PLAYER)
            {
                allies = player;
            }
            else if (unitModel.getUnit().team == Unit.UnitTeam.ENEMY)
            {
                allies = enemy;
            }
            else if (unitModel.getUnit().team == Unit.UnitTeam.ALLY)
            {
                allies = ally;
            }
            else if (unitModel.getUnit().team == Unit.UnitTeam.OTHER)
            {
                allies = other;
            }
            int idx = (allies.IndexOf(unitModel.getUnit()) + 1) % allies.Count;
            while (allies[idx].isExhausted && allies[idx] != unitModel.getUnit())
            {
                idx = (idx + 1) % allies.Count;
            }
            Tile tile = allies[idx].model.getTile();
            setCursor(tile.x, tile.y);
            setCameraPosition();
            if (allies[idx] != unitModel.getUnit())
            {
                playOneTimeSound("tile");
            }
        }
        else
        {
            for (int q = 0; q < player.Count; q++)
            {
                if (!player[q].isExhausted)
                {
                    Tile tile = player[q].model.getTile();
                    setCursor(tile.x, tile.y);
                    setCameraPosition();
                    playOneTimeSound("tile");
                    break;
                }
            }
        }
    }
    public override void UP()
    {
        if (selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.MOVE)
        {
            moveCursor(0, 1);
        }
        else if (selectionMode == SelectionMode.MENU || selectionMode == SelectionMode.SELECT_WEAPON
          || selectionMode == SelectionMode.MAP_MENU || selectionMode == SelectionMode.SELECT_GEM
          || selectionMode == SelectionMode.ITEM_MENU)
        {
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text")
                .GetComponent<TextMeshProUGUI>().color = Color.black;
            menuIdx--;
            if (menuIdx < 0)
            {
                menuIdx = menuOptions.Count - 1;
            }
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text")
                .GetComponent<TextMeshProUGUI>().color = Color.cyan;
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            interactIdx = (interactIdx + 1) % interactableUnits.Count;
            setCursor(interactableUnits[interactIdx].x, cursorY = interactableUnits[interactIdx].y);
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            if (menuIdx == 1)
            {
                specialMenuIdx = specialMenuIdx <= 0 ? 2 : specialMenuIdx - 1;
                getItemDescription(specialMenuIdx);
            }
        }
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            Transform menu = selectionMode == SelectionMode.GAMEOVER ? StaticData.findDeepChild(transform, "GameOverMenu")
                : StaticData.findDeepChild(transform, "EscapeMenu");
            specialMenuIdx = 1 - specialMenuIdx;
            StaticData.findDeepChild(menu, "SpecialMenu" + specialMenuIdx).GetComponent<TextMeshProUGUI>()
                .color = Color.cyan;
            StaticData.findDeepChild(menu, "SpecialMenu" + (1 - specialMenuIdx)).GetComponent<TextMeshProUGUI>()
                .color = Color.white;
        }
    }
    public override void DOWN()
    {
        if (selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.MOVE)
        {
            moveCursor(0, -1);
        }
        else if (selectionMode == SelectionMode.MENU || selectionMode == SelectionMode.SELECT_WEAPON
            || selectionMode == SelectionMode.MAP_MENU || selectionMode == SelectionMode.SELECT_GEM
            || selectionMode == SelectionMode.ITEM_MENU)
        {
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text")
                 .GetComponent<TextMeshProUGUI>().color = Color.black;
            menuIdx = (menuIdx + 1) % menuOptions.Count;
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text")
                .GetComponent<TextMeshProUGUI>().color = Color.cyan;
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            interactIdx--;
            if (interactIdx < 0)
            {
                interactIdx = interactableUnits.Count - 1;
            }
            setCursor(interactableUnits[interactIdx].x, interactableUnits[interactIdx].y);
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            if (menuIdx == 1)
            {
                specialMenuIdx = (specialMenuIdx + 1) % 3;
                getItemDescription(specialMenuIdx);
            }
        }
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            UP();
        }
    }
    public override void LEFT()
    {
        if (selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.MOVE)
        {
            moveCursor(-1, 0);
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            interactIdx--;
            if (interactIdx < 0)
            {
                interactIdx = interactableUnits.Count - 1;
            }
            setCursor(interactableUnits[interactIdx].x, cursorY = interactableUnits[interactIdx].y);
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            menuIdx = menuIdx == 0 ? 2 : menuIdx - 1;
            switchStatsPage();
            playOneTimeSound(AssetDictionary.getAudio("tile"));
        }

    }
    public override void RIGHT()
    {
        if (selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.MOVE)
        {
            moveCursor(1, 0);
        }
        else if (selectionMode == SelectionMode.SELECT_ENEMY || selectionMode == SelectionMode.SELECT_TALKER
            || selectionMode == SelectionMode.SELECT_TRADER || selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            interactIdx = (interactIdx + 1) % interactableUnits.Count;
            setCursor(interactableUnits[interactIdx].x, interactableUnits[interactIdx].y);
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            menuIdx = (menuIdx + 1) % 3;
            switchStatsPage();
            playOneTimeSound(AssetDictionary.getAudio("tile"));
        }
    }

    public override void ENTER()
    {
        if (selectionMode == SelectionMode.BATTLE || selectionMode == SelectionMode.ENEMYPHASE_ATTACK
            || selectionMode == SelectionMode.ALLYPHASE_ATTACK || selectionMode == SelectionMode.OTHERPHASE_ATTACK)
        {
            instantiatedBattleAnimation.skip();
        }
        else if (selectionMode == SelectionMode.IN_CONVO)
        {
            instantiatedMapEvent.skip();
        }
    }
    public override void ESCAPE()
    {
        enableChild("EscapeMenu", true);
        Transform escape = StaticData.findDeepChild(transform, "EscapeMenu");
        StaticData.findDeepChild(escape, "SpecialMenu0").GetComponent<TextMeshProUGUI>()
            .color = Color.cyan;
        StaticData.findDeepChild(escape, "SpecialMenu1").GetComponent<TextMeshProUGUI>()
            .color = Color.cyan;
        specialMenuIdx = 0;
        selectionMode = SelectionMode.ESCAPE_MENU;
    }
    public void restartChapter()
    {
        SpecialMenuLogic.restartChapter();
    }
    public void mainMenu()
    {
        SpecialMenuLogic.mainMenu();
    }
    private void initiateMapMenu()
    {
        enableChild("HUD", false);
        enableChild("Menu", true);
        getMapMenuOptions();

        selectionMode = SelectionMode.MAP_MENU;
    }
    private void cancelMove()
    {
        playOneTimeSound(AssetDictionary.getAudio("back"));
        selectionMode = SelectionMode.ROAM;
        selectedTile = null;
        selectedUnit = null;
        unfillTraversableTiles();
    }
    private void initiateMove(Tile tile)
    {
        setCursor(tile);
        if (tile.getOccupant() != null)
        {
            selectedTile = tile;
            selectedUnit = tile.getOccupant().getUnit();

            selectedUnit.model.cancelMovement();
            fillTraversableTiles(selectedUnit, cursorX, cursorY);

            selectionMode = SelectionMode.MOVE;
        }
    }
    private void cancelTravel(bool mouse)
    {
        playOneTimeSound(AssetDictionary.getAudio("back"));
        setCursor(selectedTile);
        if (!mouse)
        {
            setCameraPosition();
        }
        initiateMove(selectedTile);
    }
    private void initiateTravel(Tile tile)
    {
        playOneTimeSound(AssetDictionary.getAudio("select"));

        setCursor(tile);

        if (selectedUnit.team == Unit.UnitTeam.PLAYER && traversableTiles.ContainsKey(tile)
            && (tile.getOccupant() == null || tile.getOccupant().getUnit() == selectedUnit)
            && (!selectedUnit.isExhausted || testingMode))
        {
            moveDest = tile;

            Vector3[] path = getPath();
            selectedUnit.model.setPath(path);
            unfillTraversableTiles();

            selectionMode = SelectionMode.TRAVEL;
        }
    }
    private void cancelMenu(bool mouse)
    {
        playOneTimeSound(AssetDictionary.getAudio("back"));
        StaticData.findDeepChild(transform, "Menu").gameObject.SetActive(false);
        enableChild("HUD", true);
        cancelTravel(mouse);
    }
    private void initiateMenu()
    {
        setCameraPosition();
        StaticData.findDeepChild(transform, "Menu").gameObject.SetActive(true);
        enableChild("HUD", false);
        getMenuOptions();

        selectionMode = SelectionMode.MENU;
    }
    private void cancelOtherUnitSelection()
    {
        playOneTimeSound(AssetDictionary.getAudio("back"));
        unfillAttackableTiles();
        initiateMenu();

        selectionMode = SelectionMode.MENU;
    }
    private void tradeItem(Unit trader)
    {
        Item temp = trader.heldItem;
        trader.heldItem = selectedUnit.heldItem;
        selectedUnit.heldItem = temp;

        selectionMode = SelectionMode.ROAM;
    }
    private void tradeWeapon(Unit trader)
    {
        Weapon temp = trader.heldWeapon;
        trader.heldWeapon = selectedUnit.heldWeapon;
        selectedUnit.heldWeapon = temp;

        selectionMode = SelectionMode.ROAM;
    }
    public void getAttackOptions()
    {
        Dictionary<Tile, object> attackable = getAttackableBattlegroundTilesFromDestination(selectedUnit, moveDest);
        interactableUnits = getAttackableTilesWithEnemies(attackable, selectedUnit);
        foreach (Tile t in interactableUnits)
        {
            t.highlightAttack();
        }
        interactIdx = 0;
        setCursor(interactableUnits[interactIdx].x, interactableUnits[interactIdx].y);

        selectionMode = SelectionMode.SELECT_ENEMY;
    }
    public void getMenuOptions()
    {
        Transform mi = StaticData.findDeepChild(transform, "MenuItems");
        clearMenu(mi);

        fillAttackableTiles();
        if (selectedUnit.isLeader && moveDest.getType() == Tile.SEIZE_POINT)
        {
            Button seize = Instantiate(menuOption, mi);
            StaticData.findDeepChild(seize.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Seize";
            menuOptions.Add(seize);
            menuElements.Add(MenuChoice.SEIZE);
        }
        List<Tile> talkable = getTalkableTiles(moveDest, selectedUnit);
        if (talkable.Count != 0)
        {
            Button talk = Instantiate(menuOption, mi);
            StaticData.findDeepChild(talk.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Talk";
            menuOptions.Add(talk);
            menuElements.Add(MenuChoice.TALK);
        }
        Dictionary<Tile, object> attackable = getAttackableBattlegroundTilesFromDestination(selectedUnit, moveDest);
        List<Tile> reallyAttackable = getAttackableTilesWithEnemies(attackable, selectedUnit);
        if (reallyAttackable.Count != 0)
        {
            Button attack = Instantiate(menuOption, mi);
            StaticData.findDeepChild(attack.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Attack";
            menuOptions.Add(attack);
            menuElements.Add(MenuChoice.ATTACK);
        }
        if (moveDest.getType() == Tile.WARP_PAD)
        {
            Button escape = Instantiate(menuOption, mi);
            StaticData.findDeepChild(escape.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Escape";
            menuOptions.Add(escape);
            menuElements.Add(MenuChoice.ESCAPE);
        }
        if (moveDest.getType() == Tile.CHEST && moveDest.hasLoot())
        {
            Button chest = Instantiate(menuOption, mi);
            StaticData.findDeepChild(chest.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Chest";
            menuOptions.Add(chest);
            menuElements.Add(MenuChoice.CHEST);
        }
        if (selectedUnit.heldItem != null || selectedUnit.personalItem is UsableItem)
        {
            Button item = Instantiate(menuOption, mi);
            StaticData.findDeepChild(item.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Item";
            menuOptions.Add(item);
            menuElements.Add(MenuChoice.ITEM);
        }
        if (selectedUnit.personalItem is Weapon || selectedUnit.heldWeapon != null)
        {
            Button weapon = Instantiate(menuOption, mi);
            StaticData.findDeepChild(weapon.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Weapon";
            menuOptions.Add(weapon);
            menuElements.Add(MenuChoice.WEAPON);
        }
        if (moveDest.getGemstones().Count > 0 && selectedUnit.heldItem == null)
        {
            Button gem = Instantiate(menuOption, mi);
            StaticData.findDeepChild(gem.transform, "Text").GetComponent<TextMeshProUGUI>().text
                = "Pick Up Gem";
//            gem.fontSize = 18;
            menuOptions.Add(gem);
            menuElements.Add(MenuChoice.GEM);
        }

        Button wait = Instantiate(menuOption, mi);
        StaticData.findDeepChild(wait.transform, "Text").GetComponent<TextMeshProUGUI>().text
            = "Wait";
        menuOptions.Add(wait);
        menuElements.Add(MenuChoice.WAIT);

        menuIdx = 0;
        prepareMenu();
    }

    private void getMapMenuOptions()
    {
        Transform mi = StaticData.findDeepChild(transform, "MenuItems");
        clearMenu(mi);

        /*
        Button unitData = Instantiate(menuOption, mi);
        StaticData.findDeepChild(unitData.transform, "Text").GetComponent<TextMeshProUGUI>().text
            = "Units";
        menuOptions.Add(unitData);
        //        menuElements.Add(MenuChoice.SEIZE);
        */

        Button status = Instantiate(menuOption, mi);
        StaticData.findDeepChild(status.transform, "Text").GetComponent<TextMeshProUGUI>().text
            = "Status";
        menuOptions.Add(status);
        menuElements.Add(MenuChoice.STATUS);

        Button options = Instantiate(menuOption, mi);
        StaticData.findDeepChild(options.transform, "Text").GetComponent<TextMeshProUGUI>().text
            = "Options";
        menuOptions.Add(options);
        menuElements.Add(MenuChoice.OPTIONS);

        Button end = Instantiate(menuOption, mi);
        StaticData.findDeepChild(end.transform, "Text").GetComponent<TextMeshProUGUI>().text
            = "End Turn";
        menuOptions.Add(end);
        menuElements.Add(MenuChoice.END);

        menuIdx = 0;
        prepareMenu();
    }

    private void updateMenu()
    {
        foreach (Button opt in menuOptions)
        {
            StaticData.findDeepChild(opt.transform, "Text").GetComponent<TextMeshProUGUI>()
                .color = Color.black;
        }
        StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
            .color = Color.cyan;
    }
    private void prepareMenu()
    {
        for (int q = 0; q < menuOptions.Count; q++)
        {
            Button opt = menuOptions[q];
            opt.gameObject.SetActive(true);
            menuOptions[q].GetComponent<MenuOption>().setIdx(q);
        }
        updateMenu();
    }
    private void clearMenu(Transform mi)
    {
        foreach (Button b in menuOptions)
        {
            Destroy(b);
        }
        mi.DetachChildren();
        menuOptions = new List<Button>();
        menuElements = new List<MenuChoice>();
    }
    private void finalizeMove()
    {
        selectedTile.setOccupant(null);
        moveDest.setOccupant(selectedUnit.model);
        selectedUnit.isExhausted = true;
        selectedUnit.model.setStandingRotation(selectedUnit.model.transform.rotation);
        selectedUnit.model.setCircleColorExhaust();
    }
    private void tryTakeLoot(Unit taker, Tile takeFrom)
    {
        if (moveDest.hasLoot() && Random.Range(0, 100) < selectedUnit.luck) //OR selectedUnit is a thief
        {
            string note = takeFrom.takeLoot(taker);
            makeNotification(note, "itemget");
            timer = 2;

            selectionMode = SelectionMode.ITEM_NOTE;
        }
        else
        {
            enableChild("HUD", true);
            selectionMode = SelectionMode.ROAM;
        }
    }
    private void makeNotification(string note, string soundEffect)
    {
        enableChild("Notification", true);
        StaticData.findDeepChild(transform, "NotificationMessage").GetComponent<TextMeshProUGUI>()
            .text = note;
        if (soundEffect != null)
        {
            playOneTimeSound(soundEffect);
        }
    }
    public void selectMenuOption(int idx)
    {
        Debug.Log($"selected option {idx}, {menuElements[idx]}");

        playOneTimeSound(AssetDictionary.getAudio("select"));

        MenuChoice choice = menuElements[idx];

        if (choice == MenuChoice.TALK)
        {
            unfillAttackableTiles();
            interactableUnits = getTalkableTiles(moveDest, selectedUnit);
            foreach (Tile t in interactableUnits)
            {
                t.highlightInteract();
            }
            interactIdx = 0;
            setCursor(interactableUnits[interactIdx].x, cursorY = interactableUnits[interactIdx].y);

            enableChild("Menu", false);

            selectionMode = SelectionMode.SELECT_TALKER;

        }
        else if (choice == MenuChoice.ATTACK)
        {
            enableChild("Menu", false);
            enableChild("HUD", true);
            unfillAttackableTiles();

            getAttackOptions();
        }
        else if (choice == MenuChoice.WEAPON)
        {
            Transform mi = StaticData.findDeepChild(transform, "MenuItems");
            clearMenu(mi);

            if (selectedUnit.personalItem is Weapon)
            {
                Button wep = Instantiate(menuOption, mi);
                StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .text = "Equip " + selectedUnit.personalItem.itemName;
                StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .fontSize = 13;
                menuOptions.Add(wep);
                menuElements.Add(MenuChoice.EQUIP_PERSONAL);
            }
            if (selectedUnit.heldWeapon != null)
            {
                if (selectedUnit.weaponType == selectedUnit.heldWeapon.weaponType
                    && selectedUnit.proficiency >= selectedUnit.heldWeapon.proficiency)
                {
                    Button wep = Instantiate(menuOption, mi);
                    StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                        .text = "Equip " + selectedUnit.heldWeapon.itemName;
                    StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                        .fontSize = 13;
                    menuOptions.Add(wep);
                    menuElements.Add(MenuChoice.EQUIP_HELD);
                }
                if (getAdjacentTilesWithAllies(moveDest, selectedUnit).Count > 0)
                {
                    Button wep = Instantiate(menuOption, mi);
                    StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                        .text = "Trade " + selectedUnit.heldWeapon.itemName;
                    StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                        .fontSize = 13;
                    menuOptions.Add(wep);
                    menuElements.Add(MenuChoice.TRADE_WEAPON);
                }
                Button drop = Instantiate(menuOption, mi);
                StaticData.findDeepChild(drop.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .text = "Drop " + selectedUnit.heldWeapon.itemName;
                StaticData.findDeepChild(drop.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .fontSize = 13;
                menuOptions.Add(drop);
                menuElements.Add(MenuChoice.DROP_WEAPON);
            }
            Button none = Instantiate(menuOption, mi);
            StaticData.findDeepChild(none.transform, "Text").GetComponent<TextMeshProUGUI>()
                .text = "Equip None";
            StaticData.findDeepChild(none.transform, "Text").GetComponent<TextMeshProUGUI>()
                .fontSize = 13;
            menuOptions.Add(none);
            menuElements.Add(MenuChoice.EQUIP_NONE);

            menuIdx = 0;
            prepareMenu();

            selectionMode = SelectionMode.SELECT_WEAPON;
        }
        else if (choice == MenuChoice.EQUIP_PERSONAL)
        {
            selectedUnit.equip(0);
            selectedUnit.model.equip();
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.EQUIP_HELD)
        {
            selectedUnit.equip(1);
            selectedUnit.model.equip();
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.EQUIP_NONE)
        {
            selectedUnit.equip(2);
            selectedUnit.model.equip();
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.ESCAPE)
        {
            unfillAttackableTiles();

            player.Remove(selectedUnit);
            StaticData.registerSupportUponEscape(selectedUnit, player, turn);
            selectedTile.setOccupant(null);
            Instantiate(warpAnimation, selectedUnit.model.transform.position, warpAnimation.transform.rotation);
            playOneTimeSound(AssetDictionary.getAudio("warp"));
            Destroy(selectedUnit.model.gameObject);

            enableChild("Menu", false);

            selectionMode = SelectionMode.ROAM;
        }
        else if (choice == MenuChoice.ITEM)
        {
            Transform mi = StaticData.findDeepChild(transform, "MenuItems");
            clearMenu(mi);

            if (selectedUnit.personalItem is UsableItem)
            {
                Button pers = Instantiate(menuOption, mi);
                TextMeshProUGUI text =
                    StaticData.findDeepChild(pers.transform, "Text").GetComponent<TextMeshProUGUI>();
                    text.text = "Use " + selectedUnit.personalItem.itemName;
                if (text.text.Length > 7)
                {
                    text.fontSize = 13;
                }
                menuOptions.Add(pers);
                menuElements.Add(MenuChoice.USE_PERSONAL);
            }
            if (selectedUnit.heldItem is UsableItem)
            {
                Button held = Instantiate(menuOption, mi);
                TextMeshProUGUI text =
                    StaticData.findDeepChild(held.transform, "Text").GetComponent<TextMeshProUGUI>();
                text.text = "Use " + selectedUnit.heldItem.itemName;
                if (text.text.Length > 7)
                {
                    text.fontSize = 13;
                }
                menuOptions.Add(held);
                menuElements.Add(MenuChoice.USE_HELD);
            }
            if (selectedUnit.heldItem != null)
            {
                if (getAdjacentTilesWithAllies(moveDest, selectedUnit).Count > 0)
                {
                    Button trade = Instantiate(menuOption, mi);
                    TextMeshProUGUI text =
                        StaticData.findDeepChild(trade.transform, "Text").GetComponent<TextMeshProUGUI>();
                    text.text = "Trade Items";
                    if (text.text.Length > 7)
                    {
                        text.fontSize = 13;
                    }
                    menuOptions.Add(trade);
                    menuElements.Add(MenuChoice.TRADE);
                }
                Button drop = Instantiate(menuOption, mi);
                TextMeshProUGUI dropText =
                    StaticData.findDeepChild(drop.transform, "Text").GetComponent<TextMeshProUGUI>();
                dropText.text = "Drop " + selectedUnit.heldItem.itemName;
                if (dropText.text.Length > 7)
                {
                    dropText.fontSize = 13;
                }
                menuOptions.Add(drop);
                menuElements.Add(MenuChoice.DROP);
            }

            menuIdx = 0;
            prepareMenu();

            selectionMode = SelectionMode.ITEM_MENU;

        }
        else if (choice == MenuChoice.USE_PERSONAL)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            ((UsableItem)selectedUnit.personalItem).use(selectedUnit);

            selectionMode = SelectionMode.USE_ITEM;
        }
        else if (choice == MenuChoice.USE_HELD)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            ((UsableItem)selectedUnit.heldItem).use(selectedUnit);
            if (selectedUnit.heldItem.usesLeft <= 0)
            {
                selectedUnit.heldItem = null;
            }

            tryTakeLoot(selectedUnit, moveDest);
        }
        else if (choice == MenuChoice.TRADE)
        {
            unfillAttackableTiles();
            interactableUnits = getAdjacentTilesWithAllies(moveDest, selectedUnit);
            foreach (Tile t in interactableUnits)
            {
                t.highlightInteract();
            }
            interactIdx = 0;
            setCursor(interactableUnits[interactIdx].x, interactableUnits[interactIdx].y);

            enableChild("Menu", false);

            selectionMode = SelectionMode.SELECT_TRADER;
        }
        else if (choice == MenuChoice.TRADE_WEAPON)
        {
            unfillAttackableTiles();
            interactableUnits = getAdjacentTilesWithAllies(moveDest, selectedUnit);
            foreach (Tile t in interactableUnits)
            {
                t.highlightInteract();
            }
            interactIdx = 0;
            setCursor(interactableUnits[interactIdx].x, interactableUnits[interactIdx].y);

            enableChild("Menu", false);

            selectionMode = SelectionMode.SELECT_WEAPON_TRADER;
        }
        else if (choice == MenuChoice.DROP)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            if (selectedUnit.heldItem is Gemstone)
            {
                moveDest.getGemstones().Add((Gemstone)selectedUnit.heldItem);
                moveDest.updateGemstones();
            }
            selectedUnit.heldItem = null;

            tryTakeLoot(selectedUnit, moveDest);
        }
        else if (choice == MenuChoice.DROP_WEAPON)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            selectedUnit.heldWeapon = null;

            tryTakeLoot(selectedUnit, moveDest);
        }
        else if (choice == MenuChoice.CHEST)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            moveDest.getDeco().GetComponent<DecoMorph>().morph();
            string note = moveDest.takeLoot(selectedUnit);
            makeNotification(note, null);
            timer = 2;

            selectionMode = SelectionMode.ITEM_NOTE;
        }
        else if (choice == MenuChoice.SEIZE)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);
            menuOptions.Clear();

            finalizeMove();

            seized = true;

            selectionMode = SelectionMode.ROAM;

        }
        else if (choice == MenuChoice.GEM)
        {
            Transform mi = StaticData.findDeepChild(transform, "MenuItems");
            clearMenu(mi);

            for (int q = 0; q < moveDest.getGemstones().Count; q++)
            {
                Gemstone gem = moveDest.getGemstones()[q];
                Button take = Instantiate(menuOption, mi);
                TextMeshProUGUI text =
                    StaticData.findDeepChild(take.transform, "Text").GetComponent<TextMeshProUGUI>();
                text.text = gem.unit.unitName;
                if (text.text.Length > 7)
                {
                    text.fontSize = 13;
                }
                menuOptions.Add(take);
                menuElements.Add(MenuChoice.PICKED_GEM);
            }
            menuIdx = 0;
            prepareMenu();

            selectionMode = SelectionMode.SELECT_GEM;

        }
        else if (choice == MenuChoice.PICKED_GEM)
        {
            selectedUnit.heldItem = moveDest.getGemstones()[menuIdx];
            moveDest.getGemstones().Remove((Gemstone)selectedUnit.heldItem);
            moveDest.updateGemstones();

            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            selectionMode = SelectionMode.ROAM;

        }
        else if (choice == MenuChoice.WAIT)
        {
            unfillAttackableTiles();

            enableChild("Menu", false);

            finalizeMove();

            tryTakeLoot(selectedUnit, moveDest);
        }
        else if (choice == MenuChoice.STATUS)
        {
            enableChild("HUD", false);
            enableChild("Status", true);
            StaticData.findDeepChild(transform, "ChapterName").GetComponent<TextMeshProUGUI>()
                .text = chapterName;
            if (player.Count > 0)
            {
                StaticData.findDeepChild(transform, "PlayerCount").GetComponent<TextMeshProUGUI>()
                    .text = teamNames[0] + ": " + player.Count;
            }
            else
            {
                enableChild("PlayerCountBack", false);
            }
            if (enemy.Count > 0)
            {
                StaticData.findDeepChild(transform, "EnemyCount").GetComponent<TextMeshProUGUI>()
                    .text = teamNames[1] + ": " + enemy.Count;
            }
            else
            {
                enableChild("EnemyCountBack", false);
            }
            if (ally.Count > 0)
            {
                StaticData.findDeepChild(transform, "AllyCount").GetComponent<TextMeshProUGUI>()
                    .text = teamNames[2] + ": " + ally.Count;
            }
            else
            {
                enableChild("AllyCountBack", false);
            }
            if (other.Count > 0)
            {
                StaticData.findDeepChild(transform, "OtherCount").GetComponent<TextMeshProUGUI>()
                    .text = teamNames[3] + ": " + other.Count;
            }
            else
            {
                enableChild("OtherCountBack", false);
            }

            StaticData.findDeepChild(transform, "StatusObjective").GetComponent<TextMeshProUGUI>()
                .text = "Objective: " + objective.getName(this);
            StaticData.findDeepChild(transform, "StatusFailure").GetComponent<TextMeshProUGUI>()
                .text = "Defeat: " + objective.getFailure();
            StaticData.findDeepChild(transform, "Turn").GetComponent<TextMeshProUGUI>()
                .text = "TURN " + turn;
            StaticData.findDeepChild(transform, "Par").GetComponent<TextMeshProUGUI>()
                .text = "Par: " + turnPar;
            StaticData.findDeepChild(transform, "Iron").GetComponent<TextMeshProUGUI>()
                .text = "Iron: " + StaticData.iron;
            StaticData.findDeepChild(transform, "Steel").GetComponent<TextMeshProUGUI>()
                .text = "Steel: " + StaticData.steel;
            StaticData.findDeepChild(transform, "Silver").GetComponent<TextMeshProUGUI>()
                .text = "Silver: " + StaticData.silver;


            selectionMode = SelectionMode.STATUS;
        }
        else if (choice == MenuChoice.OPTIONS)
        {
            //TODO show options page
            selectionMode = SelectionMode.CONTROLS;
        }
        else if (choice == MenuChoice.END)
        {
            enableChild("Menu", false);
            nextTeamPhase();
        }

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
            Unit u = t.getOccupant() == null ? null : t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (x < width - 1)
        {
            Tile t = map[x + 1, y];
            Unit u = t.getOccupant() == null ? null : t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (y > 0)
        {
            Tile t = map[x, y - 1];
            Unit u = t.getOccupant() == null ? null : t.getOccupant().getUnit();
            if (u != null && u.talkConvo != null
                    && (!u.talkRestricted || selected == player[0]))
            {
                ret.Add(t);
            }
        }
        if (y < height - 1)
        {
            Tile t = map[x, y + 1];
            Unit u = t.getOccupant() == null ? null : t.getOccupant().getUnit();
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
        Debug.Log(moveDest.name);
        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();
        Dictionary<Tile, int> f = new Dictionary<Tile, int>();
        Dictionary<Tile, int> g = new Dictionary<Tile, int>();
        Dictionary<Tile, int> pos = new Dictionary<Tile, int>();
        Dictionary<Tile, Tile> parent = new Dictionary<Tile, Tile>();

        open.Add(selectedTile);
        f.Add(selectedTile, 0);
        g.Add(selectedTile, 0);
        pos.Add(selectedTile, 0);
        while (open.Count > 0)
        {
            Tile check = open[0];
            for (int idx = 1; idx < open.Count; idx++)
            {
                if (f[check] > f[open[idx]])
                {
                    check = open[idx];
                }
            }
            open.Remove(check);
            List<Tile> successor = new List<Tile>();
            if (check.x > 0)
            {
                Tile child = map[check.x - 1, check.y];
                successor.Add(child);
            }
            if (check.x < width - 1)
            {
                Tile child = map[check.x + 1, check.y];
                successor.Add(child);
            }
            if (check.y > 0)
            {
                Tile child = map[check.x, check.y - 1];
                successor.Add(child);
            }
            if (check.y < height - 1)
            {
                Tile child = map[check.x, check.y + 1];
                successor.Add(child);
            }
            foreach (Tile child in successor)
            {
                if (child == moveDest)
                {
                    parent.Add(child, check);
                    return interpretPath(parent);
                }
                if (closed.Contains(child)
                    || (child.getOccupant() != null && (child.getOccupant().getUnit().team == Unit.UnitTeam.ENEMY) != (selectedUnit.team == Unit.UnitTeam.ENEMY))
                    || child.getCost(selectedUnit.isFlying()) == int.MaxValue)
                {
                    continue;
                }
                addToDictionary(child, pos[check] + 1, pos);
                addToDictionary(child, g[check] + child.getCost(selectedUnit.isFlying()), g);
                int h = Mathf.Abs(child.x - check.x) + Mathf.Abs(child.y - check.y);
                int calculateF = g[child] + h;
                if (!f.ContainsKey(child) || f[child] > calculateF)
                {
                    open.Add(child);
                    addToDictionary(child, check, parent);
                    addToDictionary(child, calculateF, f);
                }
            }
            closed.Add(check);
        }
        return interpretPath(parent);
    }
    private Vector3[] interpretPath(Dictionary<Tile, Tile> pathData)
    {
        Tile current = moveDest;
        List<Vector3> backwards = new List<Vector3>();
        while (current != selectedTile)
        {
            backwards.Add(current.getStage().position);
            current = pathData[current];
        }
        backwards.Add(selectedTile.getStage().position);
        Vector3[] ret = new Vector3[backwards.Count];
        for (int q = backwards.Count - 1; q >= 0; q--)
        {
            ret[(backwards.Count - 1) - q] = backwards[q];
        }
        return ret;
    }
    private void addToDictionary(Tile key, Tile value, Dictionary<Tile, Tile> dictionary)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
    private void addToDictionary(Tile key, int value, Dictionary<Tile, int> dictionary)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }

    private void statsPage(Unit unit)
    {
        statsUnit = unit;
        enableChild("StatsPage", true);
        menuIdx = 0;
        switchStatsPage();

        if (unit.team == Unit.UnitTeam.PLAYER)
        {
            StaticData.findDeepChild(transform, "PortraitBack").GetComponent<Image>().color = Color.magenta;
        }
        else if (unit.team == Unit.UnitTeam.ENEMY)
        {
            StaticData.findDeepChild(transform, "PortraitBack").GetComponent<Image>().color = Color.yellow;
        }
        else if (unit.team == Unit.UnitTeam.ALLY)
        {
            StaticData.findDeepChild(transform, "PortraitBack").GetComponent<Image>().color = Color.cyan;
        }
        else if (unit.team == Unit.UnitTeam.OTHER)
        {
            StaticData.findDeepChild(transform, "PortraitBack").GetComponent<Image>().color = Color.white;
        }

        StaticData.findDeepChild(transform, "StatsPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(unit.unitName);
        if (unit.talkConvo != null)
        {
            enableChild("TalkIcon", true);
            if (unit.talkRestricted)
            {
                StaticData.findDeepChild(transform, "TalkIcon").GetComponent<Image>()
                    .sprite = AssetDictionary.getImage("Rose Quartz");
            }
            else
            {
                StaticData.findDeepChild(transform, "TalkIcon").GetComponent<Image>()
                    .sprite = AssetDictionary.getImage("Talk");
            }
        }
        else if (unit.isEssential)
        {
            enableChild("TalkIcon", true);
            StaticData.findDeepChild(transform, "TalkIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage("Star");
        }
        else
        {
            enableChild("TalkIcon", false);
        }
        StaticData.findDeepChild(transform, "UnitName").GetComponent<TextMeshProUGUI>()
            .text = unit.unitName;
        StaticData.findDeepChild(transform, "Class").GetComponent<TextMeshProUGUI>()
            .text = unit.unitClass.className;
        StaticData.findDeepChild(transform, "Level").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.level;
        StaticData.findDeepChild(transform, "EXP").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.experience;
        StaticData.findDeepChild(transform, "HP").GetComponent<TextMeshProUGUI>()
            .text = $"{unit.currentHP}/{unit.maxHP}";

        StaticData.findDeepChild(transform, "STR").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.strength;
        StaticData.findDeepChild(transform, "MAG").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.magic;
        StaticData.findDeepChild(transform, "SKL").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.skill;
        StaticData.findDeepChild(transform, "SPD").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.speed;
        StaticData.findDeepChild(transform, "LUK").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.luck;
        StaticData.findDeepChild(transform, "DEF").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.defense;
        StaticData.findDeepChild(transform, "RES").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.resistance;
        StaticData.findDeepChild(transform, "STRGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.strengthGrowth + "%";
        StaticData.findDeepChild(transform, "MAGGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.magicGrowth + "%";
        StaticData.findDeepChild(transform, "SKLGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.skillGrowth + "%";
        StaticData.findDeepChild(transform, "SPDGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.speedGrowth + "%";
        StaticData.findDeepChild(transform, "LUKGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.luckGrowth + "%";
        StaticData.findDeepChild(transform, "DEFGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.defenseGrowth + "%";
        StaticData.findDeepChild(transform, "RESGrowth").GetComponent<TextMeshProUGUI>()
            .text = unit.resistanceGrowth + "%";
        StaticData.findDeepChild(transform, "Move").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.movement;
        StaticData.findDeepChild(transform, "CON").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.constitution;
        StaticData.findDeepChild(transform, "Affinity").GetComponent<Image>()
            .sprite = AssetDictionary.getImage("" + unit.affinity);
        StaticData.findDeepChild(transform, "AffinityName").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.affinity;

        if (unit.personalItem == null)
        {
            enableChild("PersonalItem", false);
        }
        else
        {
            enableChild("PersonalItem", true);
            if (unit.personalItem is Weapon)
            {
                StaticData.findDeepChild(transform, "PersonalItemIcon").GetComponent<Image>()
                    .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(((Weapon)unit.personalItem).weaponType));
            }
            else
            {
                StaticData.findDeepChild(transform, "PersonalItemIcon").GetComponent<Image>()
                    .sprite = AssetDictionary.getImage("item");
            }
            StaticData.findDeepChild(transform, "PersonalItemName").GetComponent<TextMeshProUGUI>()
                .text = unit.personalItem.itemName;
            StaticData.findDeepChild(transform, "PersonalItemUses").GetComponent<TextMeshProUGUI>()
                .text = "--/--" + (unit.equipped == 0 ? "E" : "");
        }
        if (unit.heldWeapon == null)
        {
            enableChild("HeldWeapon", false);
        }
        else
        {
            enableChild("HeldWeapon", true);
            StaticData.findDeepChild(transform, "HeldWeaponIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(unit.heldWeapon.weaponType));
            StaticData.findDeepChild(transform, "HeldWeaponName").GetComponent<TextMeshProUGUI>()
                .text = unit.heldWeapon.itemName;
            StaticData.findDeepChild(transform, "HeldWeaponUses").GetComponent<TextMeshProUGUI>()
                .text = (unit.heldWeapon.uses > 0 ? $"{unit.heldWeapon.usesLeft}/{unit.heldWeapon.uses}"
                : "--/--") + (unit.equipped == 1 ? "E" : "");
        }
        if (unit.heldItem == null)
        {
            enableChild("HeldItem", false);
        }
        else
        {
            enableChild("HeldItem", true);
            StaticData.findDeepChild(transform, "HeldItemIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage("item");
            StaticData.findDeepChild(transform, "HeldItemName").GetComponent<TextMeshProUGUI>()
                .text = unit.heldItem.itemName;
            StaticData.findDeepChild(transform, "HeldItemUses").GetComponent<TextMeshProUGUI>()
                .text = unit.heldItem.uses > 0 ? $"{unit.heldItem.usesLeft}/{unit.heldItem.uses}"
                : "--/--";
        }

        StaticData.findDeepChild(transform, "ATK").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.getAttackPower();
        StaticData.findDeepChild(transform, "HIT").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.getAccuracy();
        StaticData.findDeepChild(transform, "CRIT").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.getCrit();
        StaticData.findDeepChild(transform, "AVO").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.getAvoidance();
        if (unit.getEquippedWeapon() == null)
        {
            StaticData.findDeepChild(transform, "Range").GetComponent<TextMeshProUGUI>()
                .text = "1~1";
        }
        else
        {
            StaticData.findDeepChild(transform, "Range").GetComponent<TextMeshProUGUI>()
                .text = $"{unit.getEquippedWeapon().minRange}~{unit.getEquippedWeapon().maxRange}";
        }
        StaticData.findDeepChild(transform, "HeldWeaponTypeIcon").GetComponent<Image>()
            .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(unit.weaponType));
        StaticData.findDeepChild(transform, "Proficiency").GetComponent<TextMeshProUGUI>()
            .text = "" + unit.proficiency;

        if (unit.fusionSkillBonus == Unit.FusionSkill.LOCKED)
        {
            StaticData.findDeepChild(transform, "BonusSkill").GetComponent<TextMeshProUGUI>()
                .text = "---";
            StaticData.findDeepChild(transform, "BonusSkillDesc").GetComponent<TextMeshProUGUI>()
                .text = "";
        }
        else
        {
            StaticData.findDeepChild(transform, "BonusSkill").GetComponent<TextMeshProUGUI>()
                .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkillBonus].skillName;
            StaticData.findDeepChild(transform, "BonusSkillDesc").GetComponent<TextMeshProUGUI>()
                .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkillBonus].description;
        }

        Unit[] partners = StaticData.findLivingSupportPartners(unit);
        if (partners[0] == null)
        {
            enableChild("Support1", false);
        }
        else
        {
            enableChild("Support1", true);
            StaticData.findDeepChild(transform, "Support1Affinity").GetComponent<Image>()
                .sprite = AssetDictionary.getImage("" + partners[0].affinity);
            StaticData.findDeepChild(transform, "Support1Name").GetComponent<TextMeshProUGUI>()
                .text = partners[0].unitName;
            StaticData.findDeepChild(transform, "Support1Level").GetComponent<TextMeshProUGUI>()
                .text = "" + SupportLog.supportLog[unit.supportId1].level;
            StaticData.findDeepChild(transform, "Support1Skill").GetComponent<TextMeshProUGUI>()
            .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkill1].skillName;
            StaticData.findDeepChild(transform, "Support1SkillDesc").GetComponent<TextMeshProUGUI>()
            .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkill1].description;
        }
        if (partners[1] == null)
        {
            enableChild("Support2", false);
        }
        else
        {
            enableChild("Support2", true);
            StaticData.findDeepChild(transform, "Support2Affinity").GetComponent<Image>()
                .sprite = AssetDictionary.getImage("" + partners[1].affinity);
            StaticData.findDeepChild(transform, "Support2Name").GetComponent<TextMeshProUGUI>()
                .text = partners[1].unitName;
            StaticData.findDeepChild(transform, "Support2Level").GetComponent<TextMeshProUGUI>()
                .text = "" + SupportLog.supportLog[unit.supportId2].level;
            StaticData.findDeepChild(transform, "Support2Skill").GetComponent<TextMeshProUGUI>()
            .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkill2].skillName;
            StaticData.findDeepChild(transform, "Support2SkillDesc").GetComponent<TextMeshProUGUI>()
            .text = FusionSkillExecutioner.SKILL_LIST[(int)unit.fusionSkill2].description;
        }
    }
    private void switchStatsPage()
    {
        enableChild("StatsPage0", false);
        enableChild("StatsPage1", false);
        enableChild("StatsPage2", false);
        enableChild("StatsPage" + menuIdx, true);
        enableChild("DescBackground", false);
        specialMenuIdx = -1;
    }
    public void getItemDescription(int num)
    {
        enableChild("DescBackground", true);
        if (num == 0 && statsUnit.personalItem != null)
        {
            StaticData.findDeepChild(transform, "ItemDescription").GetComponent<TextMeshProUGUI>()
                .text = statsUnit.personalItem.description();
        }
        else if (num == 1 && statsUnit.heldWeapon != null)
        {
            StaticData.findDeepChild(transform, "ItemDescription").GetComponent<TextMeshProUGUI>()
                .text = statsUnit.heldWeapon.description();
        }
        else if (num == 2 && statsUnit.heldItem != null)
        {
            StaticData.findDeepChild(transform, "ItemDescription").GetComponent<TextMeshProUGUI>()
                .text = statsUnit.heldItem.description();
        }
        playOneTimeSound("tile");
    }


    private void createForecast()
    {
        playOneTimeSound(AssetDictionary.getAudio("select"));

        enableChild("HUD", false);

        unfillAttackableTiles();
        targetTile = map[cursorX, cursorY];
        targetEnemy = targetTile.getOccupant().getUnit();

        int[] forecast = Battle.getForecast(selectedUnit.model, targetEnemy.model, player, enemy,
            selectedUnit.getEquippedWeapon(), targetEnemy.getEquippedWeapon(), moveDest, targetTile);

        Transform forecastDisplay = null;

        if (testFutureVision || selectedUnit.fusionSkillBonus == Unit.FusionSkill.FUTURE_VISION
            ||selectedUnit.fusionSkill1 == Unit.FusionSkill.FUTURE_VISION
            || selectedUnit.fusionSkill2 == Unit.FusionSkill.FUTURE_VISION)
        {
            forecastDisplay = StaticData.findDeepChild(transform, "FutureVision");

            Battle result = new Battle(selectedUnit.model, targetEnemy.model, player, enemy,
                selectedUnit.getEquippedWeapon(), targetEnemy.getEquippedWeapon(), moveDest, targetTile);

            StaticData.findDeepChild(forecastDisplay, "PlayerStartingHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[Battle.ATKHP];
            StaticData.findDeepChild(forecastDisplay, "PlayerEndingHP").GetComponent<TextMeshProUGUI>()
                .text = "" + result.getATKFinalHP();

            StaticData.findDeepChild(forecastDisplay, "EnemyStartingHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[Battle.DFDHP];
            StaticData.findDeepChild(forecastDisplay, "EnemyEndingHP").GetComponent<TextMeshProUGUI>()
                .text = "" + result.getDFDFinalHP();
        }
        else
        {
            forecastDisplay = StaticData.findDeepChild(transform, "Forecast");

            StaticData.findDeepChild(forecastDisplay, "PlayerHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[Battle.ATKHP];
            StaticData.findDeepChild(forecastDisplay, "PlayerATK").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.ATKMT] - forecast[Battle.DFDDEF])}" + (forecast[Battle.ATKCOUNT] != 1 ? $" x {forecast[Battle.ATKCOUNT]}" : "");
            StaticData.findDeepChild(forecastDisplay, "PlayerHIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.ATKHIT])}";
            StaticData.findDeepChild(forecastDisplay, "PlayerCRIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.ATKCRIT])}";


            StaticData.findDeepChild(forecastDisplay, "EnemyHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[Battle.DFDHP];
            StaticData.findDeepChild(forecastDisplay, "EnemyATK").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.DFDMT] - forecast[Battle.ATKDEF])}" + (forecast[Battle.DFDCOUNT] != 1 ? $" x {forecast[Battle.DFDCOUNT]}" : "");
            StaticData.findDeepChild(forecastDisplay, "EnemyHIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.DFDHIT])}";
            StaticData.findDeepChild(forecastDisplay, "EnemyCRIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[Battle.DFDCRIT])}";
        }
        forecastDisplay.gameObject.SetActive(true);

        float atkPotentialPercentage = (forecast[Battle.ATKHP] - ((forecast[Battle.DFDMT] - forecast[Battle.ATKDEF]) * forecast[Battle.DFDCOUNT]))
            / (0.0f + selectedUnit.maxHP);
        float dfdPotentialPercentage = (forecast[Battle.DFDHP] - ((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.ATKCOUNT]))
            / (0.0f + targetEnemy.maxHP);
        string atkExpression = null;
        string dfdExpression = null;
        if (atkPotentialPercentage >= 0.667f)
        {
            atkExpression = dfdPotentialPercentage >= 0.667f ? AssetDictionary.PORTRAIT_ANGRY
                : AssetDictionary.PORTRAIT_DARING;
        }
        else if (atkPotentialPercentage >= 0.333f)
        {
            atkExpression = dfdPotentialPercentage >= 0.667f ? AssetDictionary.PORTRAIT_NERVOUS
                : dfdPotentialPercentage >= 0.333f ? AssetDictionary.PORTRAIT_NEUTRAL
                : AssetDictionary.PORTRAIT_DARING;
        }
        else
        {
            atkExpression = dfdPotentialPercentage >= 0.333f ? AssetDictionary.PORTRAIT_SAD
                : AssetDictionary.PORTRAIT_NERVOUS;
        }
        if (dfdPotentialPercentage >= 0.667f)
        {
            dfdExpression = atkPotentialPercentage >= 0.667f ? AssetDictionary.PORTRAIT_ANGRY
                : AssetDictionary.PORTRAIT_DARING;
        }
        else if (dfdPotentialPercentage >= 0.333f)
        {
            dfdExpression = atkPotentialPercentage >= 0.667f ? AssetDictionary.PORTRAIT_NERVOUS
                : atkPotentialPercentage >= 0.333f ? AssetDictionary.PORTRAIT_NEUTRAL
                : AssetDictionary.PORTRAIT_DARING;
        }
        else
        {
            dfdExpression = atkPotentialPercentage >= 0.333f ? AssetDictionary.PORTRAIT_SAD
                : AssetDictionary.PORTRAIT_NERVOUS;
        }

        StaticData.findDeepChild(forecastDisplay, "PlayerName").GetComponent<TextMeshProUGUI>()
            .text = selectedUnit.unitName;
        StaticData.findDeepChild(forecastDisplay, "PlayerPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(selectedUnit.unitName, atkExpression);

        StaticData.findDeepChild(forecastDisplay, "EnemyName").GetComponent<TextMeshProUGUI>()
            .text = targetEnemy.unitName;
        StaticData.findDeepChild(forecastDisplay, "EnemyPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(targetEnemy.unitName, dfdExpression);

        if (selectedUnit.getEquippedWeapon() == null)
        {
            StaticData.findDeepChild(forecastDisplay, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(forecastDisplay, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = null;
        }
        else
        {
            StaticData.findDeepChild(forecastDisplay, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = selectedUnit.getEquippedWeapon().itemName;
            StaticData.findDeepChild(forecastDisplay, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(selectedUnit.getEquippedWeapon().weaponType));
        }

        if (targetEnemy.getEquippedWeapon() == null)
        {
            StaticData.findDeepChild(forecastDisplay, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(forecastDisplay, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = null;
        }
        else
        {
            StaticData.findDeepChild(forecastDisplay, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = targetEnemy.getEquippedWeapon().itemName;
            StaticData.findDeepChild(forecastDisplay, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(targetEnemy.getEquippedWeapon().weaponType));
        }

        selectionMode = SelectionMode.FORECAST;
    }
    private void startBattle()
    {
        enableChild("Forecast", false);
        enableChild("FutureVision", false);
        Vector3 selectedUnitPos = new Vector3(selectedUnit.model.transform.position.x, 0, selectedUnit.model.transform.position.z);
        Vector3 targetEnemyPos = new Vector3(targetEnemy.model.transform.position.x, 0, targetEnemy.model.transform.position.z);
        selectedUnit.model.transform.rotation = Quaternion.LookRotation(targetEnemyPos - selectedUnitPos);
        targetEnemy.model.transform.rotation = Quaternion.LookRotation(selectedUnitPos - targetEnemyPos);

        Battle battle = new Battle(selectedUnit.model, targetEnemy.model,
            getTeam(selectedUnit), getTeam(targetEnemy),
            selectedUnit.getEquippedWeapon(), targetEnemy.getEquippedWeapon(),
            moveDest, targetTile);
        Battle.RNGSTORE = new List<int>();
        music.Pause();

        StaticData.findDeepChild(selectedUnit.model.transform, "TeamCircle").gameObject.SetActive(false);
        StaticData.findDeepChild(targetEnemy.model.transform, "TeamCircle").gameObject.SetActive(false);

        instantiatedBattleAnimation = Instantiate(battleAnimation);
        instantiatedBattleAnimation.constructor(battle,
            battleMusic.Length > ((int)selectedUnit.team) ? battleMusic[(int)selectedUnit.team] : null,
            this);

        if (selectedUnit.team == Unit.UnitTeam.PLAYER)
        {
            selectionMode = SelectionMode.BATTLE;
        }
        else if (selectedUnit.team == Unit.UnitTeam.ENEMY)
        {
            selectionMode = SelectionMode.ENEMYPHASE_ATTACK;
        }
        else if (selectedUnit.team == Unit.UnitTeam.ALLY)
        {
            selectionMode = SelectionMode.ALLYPHASE_ATTACK;
        }
        else if (selectedUnit.team == Unit.UnitTeam.OTHER)
        {
            selectionMode = SelectionMode.OTHERPHASE_ATTACK;
        }
    }
    public void endBattleAnimation()
    {
        music.UnPause();
        Destroy(instantiatedBattleAnimation.gameObject);
        enableChild("HUD", true);

        if (selectedUnit.model != null)
        {
            StaticData.findDeepChild(selectedUnit.model.transform, "TeamCircle").gameObject.SetActive(true);
        }
        if (targetEnemy.model != null)
        {
            StaticData.findDeepChild(targetEnemy.model.transform, "TeamCircle").gameObject.SetActive(true);
        }

        if (selectionMode == SelectionMode.BATTLE)
        {
            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.ENEMYPHASE_ATTACK)
        {
            npcIdx++;
            selectionMode = SelectionMode.ENEMYPHASE_SELECT_UNIT;
        }
        else if (selectionMode == SelectionMode.ALLYPHASE_ATTACK)
        {
            npcIdx++;
            selectionMode = SelectionMode.ALLYPHASE_SELECT_UNIT;
        }
        else if (selectionMode == SelectionMode.OTHERPHASE_ATTACK)
        {
            npcIdx++;
            selectionMode = SelectionMode.OTHERPHASE_SELECT_UNIT;
        }
    }
    private List<Unit> getTeam(Unit u)
    {
        if (u.team == Unit.UnitTeam.PLAYER)
        {
            return player;
        }
        if (u.team == Unit.UnitTeam.ENEMY)
        {
            return enemy;
        }
        if (u.team == Unit.UnitTeam.ALLY)
        {
            return ally;
        }
        if (u.team == Unit.UnitTeam.OTHER)
        {
            return other;
        }
        return null;
    }

    private void performTalk()
    {
        Vector3 selectedUnitPos = new Vector3(selectedUnit.model.transform.position.x, 0, selectedUnit.model.transform.position.z);
        Vector3 interactUnitPos = new Vector3(talkerTile.getOccupant().transform.position.x, 0, talkerTile.getOccupant().transform.position.z);
        selectedUnit.model.transform.rotation = Quaternion.LookRotation(interactUnitPos - selectedUnitPos);
        talkerTile.getOccupant().transform.rotation = Quaternion.LookRotation(selectedUnitPos - interactUnitPos);

        instantiatedMapEvent = Instantiate(mapEvent);
        instantiatedMapEvent.constructor(talkerTile.getOccupant().getUnit().talkConvo, talkerTile.getOccupant().getUnit(),
            selectedUnit, this);
        music.Pause();

        selectionMode = SelectionMode.IN_CONVO;
    }
    public void endMapEvent()
    {
        music.UnPause();
        Destroy(instantiatedMapEvent.gameObject);
        setCameraPosition();
        enableChild("HUD", true);
        selectionMode = SelectionMode.ROAM;
    }

    private void nextTeamPhase()
    {
        if (music != null)
        {
            Destroy(music.gameObject);
        }
        List<Unit>[] teams = { player, enemy, ally, other };
        foreach (Unit u in teams[Mathf.Max(0, teamPhase)])
        {
            u.isExhausted = false;
            if (u.model != null)
            {
                u.model.setCircleColor();
            }
        }
        do
        {
            teamPhase = (teamPhase + 1) % teams.Length;
        } while (teams[teamPhase].Count == 0);
        npcIdx = 0;

        if (teams[teamPhase] == player)
        {
            turn++;
        }

        enableChild("PhaseBack", true);
        StaticData.findDeepChild(transform, "Phase").GetComponent<TextMeshProUGUI>()
            .text = teamNames[teamPhase] + " Turn";
        timer = 2;

        selectionMode = SelectionMode.PHASE;
    }

    //Returns all target tiles (destinations for ATTACK, attackables for GUARD, enemy tiles for PURSUE, houses for BURN)
    public List<Tile> testAISuccess(Unit.AIType ai)
    {
        List<Tile> ret = new List<Tile>();
        Tile startTile = selectedTile;
        if (ai == Unit.AIType.ATTACK)
        {
            Dictionary<Tile, object> traversable = getTraversableTiles(selectedUnit, startTile.x, startTile.y);
            Dictionary<Tile, object>.KeyCollection dests = traversable.Keys;
            foreach (Tile dest in dests)
            {
                if (dest.isVacant() || dest.getOccupant().getUnit() == selectedUnit)
                {
                    Dictionary<Tile, object> att = getAttackableBattlegroundTilesFromDestination(selectedUnit, dest);
                    List<Tile> realAtt = getAttackableTilesWithEnemies(att, selectedUnit);
                    if (realAtt.Count != 0)
                    {
                        ret.Add(dest);
                    }
                }
            }
        }
        else if (ai == Unit.AIType.BURN)
        {
            //TODO if there is a path to a house, add that house's tile
        }
        else if (ai == Unit.AIType.GUARD)
        {
            Dictionary<Tile, object> attackable = getAttackableBattlegroundTilesFromDestination(selectedUnit, startTile);
            List<Tile> enemyTiles = getAttackableTilesWithEnemies(attackable, selectedUnit);
            foreach (Tile t in enemyTiles)
            {
                ret.Add(t);
            }
        }
        else if (ai == Unit.AIType.PURSUE)
        {
            //TODO if there is a path to an enemy, add that enemy's tile
        }

        return ret;
    }

    /**
     * Fills report with
     * [0] = AI type
     * [1] = start tile
     * [2] = dest tile
     * [3] = target tile
     */
    private void actOnUnitAI(Unit.AIType ai,
            List<Tile> target, object[] report, List<Unit> uAllies)
    {
        report[0] = ai;
        Tile startTile = selectedTile;

        if (ai == Unit.AIType.ATTACK)
        {
            //TODO
            int heur = int.MinValue;
            Tile bestDest = null;
            Tile best = null;
            for (int q = 0; q < target.Count; q++)
            {
                Tile dest = target[q];
                Dictionary<Tile, object> att = getAttackableBattlegroundTilesFromDestination(selectedUnit, dest);
                List<Tile> enemTiles = getAttackableTilesWithEnemies(att, selectedUnit);
                for (int r = 0; r < enemTiles.Count; r++)
                {
                    Tile dfdTile = enemTiles[r];
                    int specialHeur = int.MinValue;
                    int heldHeur = int.MinValue;
                    Unit enem = dfdTile.getOccupant().getUnit();
                    int dist = Mathf.Abs(dfdTile.x - dest.x) + Mathf.Abs(dfdTile.y - dest.y);
                    if (selectedUnit.personalItem is Weapon)
                    {
                        Weapon w = (Weapon)selectedUnit.personalItem;
                        if (w.minRange <= dist && w.maxRange >= dist)
                        {
                            specialHeur = 0;
                            List<Unit> enemAllies = player.Contains(enem) ? player : ally.Contains(enem) ? ally
                                : enemy.Contains(enem) ? enemy : other.Contains(enem) ? other : null;
                            int[] forecast = Battle.getForecast(selectedUnit.model, enem.model, uAllies,
                                enemAllies, w, enem.getEquippedWeapon(), dest, dfdTile);
                            if ((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.ATKCOUNT] >= forecast[Battle.DFDHP])
                            {
                                specialHeur += 50;
                            }
                            else
                            {
                                int bonus = Mathf.RoundToInt((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.ATKHIT] / 100.0f);
                                specialHeur += Mathf.Min(40, bonus);
                            }
                            specialHeur += Mathf.Max(0, 20 - forecast[Battle.DFDHP]);
                            if (forecast[Battle.DFDCOUNT] == 0)
                            {
                                specialHeur += 10;
                            }
                            else
                            {
                                int penalty = Mathf.RoundToInt((forecast[Battle.DFDMT] - forecast[Battle.ATKDEF]) * forecast[Battle.DFDHIT] / 100.0f);
                                specialHeur -= Mathf.Min(40, penalty);
                            }
                            specialHeur -= Mathf.Max(0, 20 - (forecast[Battle.ATKHP] - (forecast[Battle.DFDMT] - forecast[Battle.ATKDEF])));
                        }
                    }
                    if (selectedUnit.heldWeapon != null)
                    {
                        Weapon w = selectedUnit.heldWeapon;
                        if (w.minRange <= dist && dist <= w.maxRange)
                        {
                            heldHeur = 0;
                            List<Unit> enemAllies = player.Contains(enem) ? player : ally.Contains(enem) ? ally
                                : enemy.Contains(enem) ? enemy : other.Contains(enem) ? other : null;
                            int[] forecast = Battle.getForecast(selectedUnit.model, enem.model, uAllies, enemAllies,
                                w, enem.getEquippedWeapon(), dest, dfdTile);
                            if ((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.DFDCOUNT] >= forecast[Battle.DFDHP])
                            {
                                heldHeur += 50;
                            }
                            else
                            {
                                int bonus = Mathf.RoundToInt((float)((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.ATKHIT] / 100.0));
                                heldHeur += Mathf.Min(40, bonus);
                            }
                            heldHeur += Mathf.Max(0, 20 - forecast[Battle.DFDHP]);
                            if (forecast[10] == 0)
                            {
                                heldHeur += 10;
                            }
                            else
                            {
                                int penalty = Mathf.RoundToInt((forecast[Battle.DFDMT] - forecast[Battle.ATKDEF]) * forecast[Battle.DFDHIT] / 100.0f);
                                heldHeur -= Mathf.Min(40, penalty);
                            }
                            heldHeur -= Mathf.Max(0, 20 - (forecast[Battle.ATKHP] - (forecast[Battle.DFDMT] - forecast[Battle.ATKDEF])));
                        }
                    }
                    if (specialHeur > heur)
                    {
                        heur = specialHeur;
                        best = dfdTile;
                        selectedUnit.equipSpecial();
                        selectedUnit.model.equip();
                        bestDest = dest;
                    }
                    if (heldHeur > heur)
                    {
                        heur = heldHeur;
                        best = dfdTile;
                        selectedUnit.equipHeld();
                        selectedUnit.model.equip();
                        bestDest = dest;
                    }
                }
            }

            report[1] = startTile;
            report[2] = bestDest;
            report[3] = best;
        }
        else if (ai == Unit.AIType.BURN)
        {
            //TODO move closer to the house or burn it if possible
        }
        else if (ai == Unit.AIType.GUARD)
        {
            int heur = int.MinValue;
            int best = 0;
            for (int q = 0; q < target.Count; q++)
            {
                Unit enem = target[q].getOccupant().getUnit();
                Weapon specialWep = null;
                Weapon heldWep = null;
                int specialHeur = 0;
                int heldHeur = 0;
                if (selectedUnit.personalItem is Weapon)
                {
                    specialWep = (Weapon)selectedUnit.personalItem;
                }
                if (selectedUnit.heldWeapon != null)
                {
                    heldWep = (Weapon)selectedUnit.heldWeapon;
                }
                List<Unit> enemAllies = player.Contains(enem) ? player : ally.Contains(enem) ? ally
                    : enemy.Contains(enem) ? enemy : other.Contains(enem) ? other : null;
                int[] w1Forecast = Battle.getForecast(selectedUnit.model, enem.model, uAllies, enemAllies,
                    specialWep, enem.getEquippedWeapon(), startTile, target[q]);
                int[] w2Forecast = Battle.getForecast(selectedUnit.model, enem.model, uAllies, enemAllies,
                    heldWep, enem.getEquippedWeapon(), startTile, target[q]);
                if ((w1Forecast[Battle.ATKMT] - w1Forecast[Battle.DFDDEF]) * w1Forecast[Battle.ATKCOUNT] >= w1Forecast[Battle.DFDHP])
                {
                    specialHeur += 50;
                }
                else
                {
                    int bonus = Mathf.RoundToInt((w1Forecast[Battle.ATKMT] - w1Forecast[Battle.DFDDEF]) * w1Forecast[Battle.ATKHIT] / 100.0f);
                    specialHeur += Mathf.Min(40, bonus);
                }
                if ((w2Forecast[Battle.ATKMT] - w2Forecast[Battle.DFDDEF]) * w2Forecast[Battle.ATKCOUNT] >= w2Forecast[Battle.DFDHP])
                {
                    heldHeur += 50;
                }
                else
                {
                    int bonus = Mathf.RoundToInt((w2Forecast[Battle.ATKMT] - w2Forecast[Battle.ATKDEF]) * w2Forecast[Battle.ATKHIT] / 100.0f);
                    heldHeur += Mathf.Min(40, bonus);
                }
                specialHeur += Mathf.Max(0, 20 - w1Forecast[Battle.DFDHP]);
                heldHeur += Mathf.Max(0, 20 - w2Forecast[Battle.DFDHP]);
                if (w1Forecast[10] == 0)
                {
                    specialHeur += 10;
                }
                else
                {
                    int penalty = Mathf.RoundToInt((w1Forecast[Battle.DFDMT] - w1Forecast[Battle.ATKDEF]) * w1Forecast[Battle.ATKHIT] / 100.0f);
                    specialHeur -= Mathf.Min(40, penalty);
                }
                if (w2Forecast[Battle.DFDCOUNT] == 0)
                {
                    heldHeur += 10;
                }
                else
                {
                    int penalty = (int)Mathf.RoundToInt((w2Forecast[Battle.DFDMT] - w2Forecast[Battle.ATKDEF]) * w2Forecast[Battle.ATKHIT] / 100.0f);
                    heldHeur -= Mathf.Min(40, penalty);
                }
                specialHeur -= Mathf.Max(0, 20 - (w1Forecast[Battle.ATKHP] - (w1Forecast[Battle.DFDMT] - w1Forecast[Battle.ATKDEF])));
                heldHeur -= Mathf.Max(0, 20 - (w2Forecast[Battle.ATKHP] - (w2Forecast[Battle.DFDMT] - w2Forecast[Battle.ATKDEF])));

                if (specialHeur > heur)
                {
                    heur = specialHeur;
                    best = q;
                    if (specialWep != null)
                    {
                        selectedUnit.equipSpecial();
                        selectedUnit.model.equip();
                    }
                    else
                    {
                        selectedUnit.equipNone();
                        selectedUnit.model.equip();
                    }
                }
                if (heldHeur > heur)
                {
                    heur = heldHeur;
                    best = q;
                    if (heldWep != null)
                    {
                        selectedUnit.equipHeld();
                    }
                    else
                    {
                        selectedUnit.equipNone();
                    }
                }
            }
            Tile enemyTile = target[best];
            report[1] = startTile;
            report[2] = startTile;
            report[3] = enemyTile;
        }
        else if (ai == Unit.AIType.PURSUE)
        {
            //TODO move closer to the enemy or attack if possible
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.P)
            && selectionMode != SelectionMode.BATTLE && selectionMode != SelectionMode.IN_CONVO
            && selectionMode != SelectionMode.STATUS && selectionMode != SelectionMode.CONTROLS
            && selectionMode != SelectionMode.ITEM_NOTE && selectionMode != SelectionMode.ESCAPE_MENU
            && selectionMode != SelectionMode.PHASE
            && !selectionMode.ToString().StartsWith("ENEMYPHASE")
            && !selectionMode.ToString().StartsWith("ALLYPHASE") && !selectionMode.ToString().StartsWith("OTHERPHASE"))
        {
            camOrientation = (camOrientation + 1) % NUM_CAMERA_POSITIONS;
            setCameraPosition();
        }

        if (selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.ENEMYPHASE_SELECT_UNIT)
        {
            if (!objectiveComplete && objective.checkFailed(this))
            {
                selectionMode = SelectionMode.GAMEOVER;
                enableChild("HUD", false);
                enableChild("GameOverMenu", true);
                specialMenuIdx = 0;
                Destroy(music.gameObject);
                music = getAudioSource(AssetDictionary.getAudio("gameover-music"));
                music.Play();

                selectionMode = SelectionMode.GAMEOVER;
                Debug.Log("CHAPTER FAILED");
                return;
            }
            else if (!objectiveComplete && objective.checkComplete(this))
            {
                selectionMode = SelectionMode.STANDBY;
                if (turn < turnPar)
                {
                    StaticData.bonusEXP += (turnPar - turn) * 50;
                }
                timer = 3;
                enableChild("Victory", true);
                enableChild("HUD", false);

                foreach (Unit u in player)
                {
                    Destroy(u.model.gameObject);
                }
                foreach (Unit u in enemy)
                {
                    Destroy(u.model.gameObject);
                }
                foreach (Unit u in ally)
                {
                    Destroy(u.model.gameObject);
                }
                foreach (Unit u in other)
                {
                    Destroy(u.model.gameObject);
                }
                Destroy(music.gameObject);

                Debug.Log("CHAPTER COMPLETE");
                return;
            }
        }

        if (selectionMode == SelectionMode.STANDBY)
        {
            if (timer <= 0)
            {
                objectiveComplete = true;
            }
        }
        if (selectionMode == SelectionMode.ITEM_NOTE)
        {
            if (timer <= 0)
            {
                enableChild("Notification", false);
                selectionMode = SelectionMode.ROAM;
            }
        }

        if (selectionMode == SelectionMode.TRAVEL && selectedUnit.model.reachedDestination())
        {
            initiateMenu();
        }
        else if (selectionMode == SelectionMode.ENEMYPHASE_COMBAT_PAUSE || selectionMode == SelectionMode.ALLYPHASE_COMBAT_PAUSE
            || selectionMode == SelectionMode.OTHERPHASE_COMBAT_PAUSE)
        {
            if (timer <= 0)
            {
                targetEnemy = targetTile.getOccupant().getUnit();
                startBattle();
            }
        }
        else if (selectionMode == SelectionMode.ENEMYPHASE_MOVE_PAUSE)
        {
            if (timer <= 0)
            {
                selectedUnit.model.setPath(getPath());
                setCursor(moveDest);
                selectionMode = SelectionMode.ENEMYPHASE_MOVE;
            }
        }
        else if (selectionMode == SelectionMode.ALLYPHASE_MOVE_PAUSE)
        {
            if (timer <= 0)
            {
                selectedUnit.model.setPath(getPath());
                setCursor(moveDest);
                selectionMode = SelectionMode.ALLYPHASE_MOVE;
            }
        }
        else if (selectionMode == SelectionMode.OTHERPHASE_MOVE_PAUSE)
        {
            if (timer <= 0)
            {
                selectedUnit.model.setPath(getPath());
                setCursor(moveDest);
                selectionMode = SelectionMode.OTHERPHASE_MOVE;
            }
        }
        else if (selectionMode == SelectionMode.ENEMYPHASE_MOVE && selectedUnit.model.reachedDestination())
        {
            finalizeMove();
            setCameraPosition();
            if ((Unit.AIType)npcAction[0] == Unit.AIType.GUARD || (Unit.AIType)npcAction[0] == Unit.AIType.ATTACK
                || ((Unit.AIType)npcAction[0] == Unit.AIType.PURSUE && (Tile)npcAction[3] != null))
            {
                timer = 1;
                targetTile = (Tile)npcAction[3];
                setCursor(targetTile);

                selectionMode = SelectionMode.ENEMYPHASE_COMBAT_PAUSE;
            }
            else if ((Unit.AIType)npcAction[0] == Unit.AIType.BURN)
            {
                //TODO
            }
            else
            {
                npcIdx++;

                selectionMode = SelectionMode.ENEMYPHASE_SELECT_UNIT;
            }
        }
        else if (selectionMode == SelectionMode.ALLYPHASE_MOVE && selectedUnit.model.reachedDestination())
        {
            finalizeMove();
            setCursor(moveDest);
            setCameraPosition();
            //TODO
        }
        else if (selectionMode == SelectionMode.OTHERPHASE_MOVE && selectedUnit.model.reachedDestination())
        {
            finalizeMove();
            setCursor(moveDest);
            setCameraPosition();
            //TODO
        }
        else if (selectionMode == SelectionMode.PHASE)
        {
            if (timer <= 0)
            {
                enableChild("PhaseBack", false);
                SelectionMode[] selection = { SelectionMode.ROAM, SelectionMode.ENEMYPHASE_SELECT_UNIT,
                    SelectionMode.ALLYPHASE_SELECT_UNIT, SelectionMode.OTHERPHASE_SELECT_UNIT };
                selectionMode = selection[teamPhase];
                if (teamPhase == 0)
                {
                    enableChild("HUD", true);
                    setCursor(player[0].model.getTile());
                    setCameraPosition();
                }

                string mapMusic = teamMusic[teamPhase % teamMusic.Length];
                music = getAudioSource(AssetDictionary.getAudio(mapMusic));
                music.loop = true;
                music.Play();

            }
        }
        else if (selectionMode == SelectionMode.ENEMYPHASE_SELECT_UNIT)
        {
            if (npcIdx >= enemy.Count)
            {
                nextTeamPhase();
                return;
            }
            selectedUnit = enemy[npcIdx];
            selectedTile = selectedUnit.model.getTile();
            setCameraPosition();
            npcAction = new object[4];
            List<Tile> targets = testAISuccess(selectedUnit.ai1);
            if (targets.Count > 0)
            {
                actOnUnitAI(selectedUnit.ai1, targets, npcAction, enemy);
            }
            else
            {
                targets = testAISuccess(selectedUnit.ai2);
                if (targets.Count > 0)
                {
                    actOnUnitAI(selectedUnit.ai2, targets, npcAction, enemy);
                }
                else
                {
                    npcIdx++;
                    return;
                }
            }
            moveDest = (Tile)npcAction[2];

            timer = 1;
            setCursor(selectedTile);
            selectionMode = SelectionMode.ENEMYPHASE_MOVE_PAUSE;
        }
    }


    public enum SelectionMode
    {
        ROAM, MOVE, TRAVEL, MENU, SELECT_ENEMY, SELECT_TALKER, SELECT_WEAPON, FORECAST, BATTLE, MAP_MENU, IN_CONVO,
        SELECT_GEM, STATUS, STATS_PAGE, CONTROLS, ITEM_NOTE, ITEM_MENU, SELECT_TRADER, SELECT_WEAPON_TRADER, USE_ITEM,
        ENEMYPHASE_SELECT_UNIT, ENEMYPHASE_MOVE_PAUSE, ENEMYPHASE_MOVE, ENEMYPHASE_ATTACK, ENEMYPHASE_BURN, ENEMYPHASE_COMBAT_PAUSE,
        ALLYPHASE_SELECT_UNIT, ALLYPHASE_MOVE_PAUSE, ALLYPHASE_MOVE, ALLYPHASE_ATTACK, ALLYPHASE_BURN, ALLYPHASE_COMBAT_PAUSE,
        OTHERPHASE_SELECT_UNIT, OTHERPHASE_MOVE_PAUSE, OTHERPHASE_MOVE, OTHERPHASE_ATTACK, OTHERPHASE_BURN, OTHERPHASE_COMBAT_PAUSE,
        STANDBY, GAMEOVER, ESCAPE_MENU, PHASE
    }

    public enum MenuChoice
    {
        TALK, ATTACK, ESCAPE, SEIZE, ITEM, WEAPON, GEM, PICKED_GEM, CHEST, WAIT, STATUS, OPTIONS, END,
        USE_PERSONAL, USE_HELD, TRADE, DROP, EQUIP_PERSONAL, EQUIP_HELD, EQUIP_NONE, TRADE_WEAPON,
        DROP_WEAPON
    }


}

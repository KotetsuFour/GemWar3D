using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] private float cameraDistance, minCamDistance, maxCamDistance;
    [SerializeField] private float zoomSpeed;

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

        menuOptions = new List<Button>();

        interactableUnits = new List<Tile>();
        cursor = Instantiate(cursorPrefab);
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

    private void setCursor(int x, int y)
    {
        cursorX = Mathf.Clamp(x, 0, width - 1);
        cursorY = Mathf.Clamp(y, 0, height - 1);
        cursor.transform.position = new Vector3(cursorX, map[cursorX, cursorY].getCursorPosition().y, cursorY);
    }
    private void setCursor(Tile tile)
    {
        setCursor(tile.x, tile.y);
    }
    private void moveCursor(int xDirection, int yDirection)
    {
        setCursor(cursorX + xDirection, cursorY + yDirection);

        setCameraPosition();
    }
    private void setCameraPosition()
    {
        Vector3 pos = cursor.transform.position;
        pos += new Vector3(0, 1, -1);
        Vector3 newPos = cursor.transform.position - ((cursor.transform.position - pos).normalized * cameraDistance);
        getCamera().transform.position = newPos;
        getCamera().transform.rotation = Quaternion.LookRotation(cursor.transform.position - getCamera().transform.position);
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
                    initiateMove(hit.collider.GetComponent<UnitModel>().getTile());
                }
                else if (hit.collider.GetComponent<Tile>() != null)
                {
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

    }
    public override void RIGHT_MOUSE()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            //TODO options menu
        }
        else if (selectionMode == SelectionMode.MOVE)
        {
            cancelMove();
        }
        else if (selectionMode == SelectionMode.TRAVEL)
        {
            cancelTravel(true);
        }
        else if (selectionMode == SelectionMode.MENU)
        {
            cancelMenu(true);
        }
    }

    public override void Z()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            Tile currentTile = map[cursorX, cursorY];
            if (currentTile.getOccupant() == null)
            {
                //TODO options menu
            }
            else
            {
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
            finalizeMove();
            startBattle();
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
            /*
            if (!nextSaying())
            {
                deconstructConversation();
                if (talkerTile.getOccupant() != null)
                {
                    talkerTile.getOccupant().talkConvo = null;
                }
                selectionMode = SelectionMode.ROAM;
            }
            */
        }
        else if (selectionMode == SelectionMode.SELECT_TRADER)
        {
            Unit trader = interactableUnits[interactIdx].getOccupant().getUnit();

            enableChild("Menu", false);

            unfillAttackableTiles();
            finalizeMove();

            tradeItem(trader);
        }
        else if (selectionMode == SelectionMode.SELECT_WEAPON_TRADER)
        {
            Unit trader = interactableUnits[interactIdx].getOccupant().getUnit();

            enableChild("Menu", false);

            unfillAttackableTiles();
            finalizeMove();

            tradeWeapon(trader);
        }
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            if (specialMenuIdx == 0)
            {
                SpecialMenuLogic.restartChapter();
            }
            else if (specialMenuIdx == 1)
            {
                SpecialMenuLogic.mainMenu();
            }
        }

    }
    public override void X()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            //TODO options menu
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

            getAttackOptions();
        }
        else if (selectionMode == SelectionMode.MAP_MENU)
        {
            /*
            Destroy(instantiatedMenuBackground);
            foreach (TextMeshProUGUI t in menuOptions)
            {
                Destroy(t);
            }
            selectionMode = SelectionMode.ROAM;
            */
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            /*
            deconstructStatsPage();

            selectionMode = SelectionMode.ROAM;
            */
        }
        else if (selectionMode == SelectionMode.CONTROLS)
        {
            /*
            controlsPage.SetActive(false);
            Camera camCam = cam.GetComponent<Camera>();
            camCam.orthographicSize = cameraOrthographicSize;
            setCameraPosition(cursorX, cursorY);
            instantiatedMapHUD.SetActive(true);

            selectionMode = SelectionMode.ROAM;
            */
        }
        else if (selectionMode == SelectionMode.STATUS)
        {
            /*
            statusPage.SetActive(false);
            Camera camCam = cam.GetComponent<Camera>();
            camCam.orthographicSize = cameraOrthographicSize;
            setCameraPosition(cursorX, cursorY);
            instantiatedMapHUD.SetActive(true);

            selectionMode = SelectionMode.ROAM;
            */
        }
        else if (selectionMode == SelectionMode.ESCAPE_MENU)
        {
            /*
            Destroy(instantiatedSpecialMenu);
            instantiatedMapHUD.SetActive(true);
            selectionMode = SelectionMode.ROAM;
            */
        }
    }
    public override void A()
    {
        //TODO
    }
    public override void S()
    {
        //TODO
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
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            /*
            instantiatedSpecialMenu.transform.GetChild(0).GetChild(specialMenuIdx).GetComponent<TextMeshProUGUI>().color = Color.white;
            specialMenuIdx--;
            if (specialMenuIdx < 0)
            {
                specialMenuIdx = instantiatedSpecialMenu.transform.GetChild(0).childCount - 1;
            }
            instantiatedSpecialMenu.transform.GetChild(0).GetChild(specialMenuIdx).GetComponent<TextMeshProUGUI>().color = Color.cyan;
            */
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
        else if (selectionMode == SelectionMode.GAMEOVER || selectionMode == SelectionMode.ESCAPE_MENU)
        {
            /*
            instantiatedSpecialMenu.transform.GetChild(0).GetChild(specialMenuIdx).GetComponent<TextMeshProUGUI>().color = Color.white;
            specialMenuIdx = (specialMenuIdx + 1) % instantiatedSpecialMenu.transform.GetChild(0).childCount;
            instantiatedSpecialMenu.transform.GetChild(0).GetChild(specialMenuIdx).GetComponent<TextMeshProUGUI>().color = Color.cyan;
            */
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
    }

    public override void ENTER()
    {
        //TODO maybe something?
    }
    public override void ESCAPE()
    {
        /*
        if (selectionMode == SelectionMode.ROAM)
        {
            instantiatedSpecialMenu = Instantiate(escapeMenu);
            instantiatedSpecialMenu.transform.position = new Vector3(cam.transform.position.x,
                cam.transform.position.y, SPECIAL_MENU_LAYER);
            instantiatedMapHUD.SetActive(false);
            instantiatedSpecialMenu.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.cyan;
            specialMenuIdx = 0;
            selectionMode = SelectionMode.ESCAPE_MENU;
        }
        */
    }


    private void cancelMove()
    {
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
        setCursor(selectedTile);
        if (!mouse)
        {
            setCameraPosition();
        }
        initiateMove(selectedTile);
    }
    private void initiateTravel(Tile tile)
    {
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
        StaticData.findDeepChild(transform, "Menu").gameObject.SetActive(false);
        cancelTravel(mouse);
    }
    private void initiateMenu()
    {
        StaticData.findDeepChild(transform, "Menu").gameObject.SetActive(true);
        getMenuOptions();

        selectionMode = SelectionMode.MENU;
    }
    private void cancelOtherUnitSelection()
    {
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
        foreach (Button b in menuOptions)
        {
            Destroy(b);
        }
        mi.DetachChildren();
        menuOptions = new List<Button>();
        menuElements = new List<MenuChoice>();
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

        for (int q = 0; q < menuOptions.Count; q++)
        {
            menuOptions[q].gameObject.SetActive(true);
            menuOptions[q].GetComponent<MenuOption>().setIdx(q);
        }

        menuIdx = 0;
        updateMenu();
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
    private void finalizeMove()
    {
        selectedTile.setOccupant(null);
        moveDest.setOccupant(selectedUnit.model);
        selectedUnit.isExhausted = true;
        //TODO change outline to grey
    }
    private void tryTakeLoot(Unit taker, Tile takeFrom)
    {
        if (moveDest.hasLoot() && Random.Range(0, 100) < selectedUnit.luck) //OR selectedUnit is a thief
        {
            string note = takeFrom.takeLoot(taker);
            makeNotification(note, null);
            timer = 2;

            selectionMode = SelectionMode.NOTIFICATION;
        }
        else
        {
            selectionMode = SelectionMode.ROAM;
        }
    }
    private void makeNotification(string note, AudioClip soundEffect)
    {
        //TODO
        if (soundEffect != null)
        {
            //TODO
        }
    }
    public void selectMenuOption(int idx)
    {
        Debug.Log($"selected option {idx}, {menuElements[idx]}");

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
            unfillAttackableTiles();

            getAttackOptions();
        }
        else if (choice == MenuChoice.WEAPON)
        {
            Transform mi = StaticData.findDeepChild(transform, "MenuItems");

            foreach (Button t in menuOptions)
            {
                Destroy(t);
            }
            mi.DetachChildren();
            menuOptions.Clear();
            menuElements.Clear();
            if (selectedUnit.personalItem is Weapon)
            {
                Button wep = Instantiate(menuOption, mi);
                StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .text = "Equip " + selectedUnit.personalItem.itemName;
                StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                    .fontSize = 13;
                if (selectedUnit.equipped == 0)
                {
                    StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                        .color = Color.cyan;
                }
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
                    if (selectedUnit.equipped == 1)
                    {
                        StaticData.findDeepChild(wep.transform, "Text").GetComponent<TextMeshProUGUI>()
                            .color = Color.cyan;
                    }
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

            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
                .color = Color.cyan;

            selectionMode = SelectionMode.SELECT_WEAPON;
        }
        else if (choice == MenuChoice.EQUIP_PERSONAL)
        {
            selectedUnit.equip(0);
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.EQUIP_HELD)
        {
            selectedUnit.equip(1);
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.EQUIP_NONE)
        {
            selectedUnit.equip(2);
            unfillAttackableTiles();
            fillAttackableTiles();
        }
        else if (choice == MenuChoice.ESCAPE)
        {
            unfillAttackableTiles();

            player.Remove(selectedUnit);
            StaticData.registerSupportUponEscape(selectedUnit, player, turn);
            selectedTile.setOccupant(null);
            Destroy(selectedUnit.model);

            //TODO play warp animation

            enableChild("Menu", false);

            selectionMode = SelectionMode.ROAM;
        }
        else if (choice == MenuChoice.ITEM)
        {
            Transform mi = StaticData.findDeepChild(transform, "MenuItems");

            foreach (Button t in menuOptions)
            {
                Destroy(t);
            }
            mi.DetachChildren();
            menuOptions.Clear();
            menuElements.Clear();
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
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
                .color = Color.cyan;

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

            string note = moveDest.takeLoot(selectedUnit);
            makeNotification(note, null);
            timer = 2;

            selectionMode = SelectionMode.NOTIFICATION;
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
            foreach (Button t in menuOptions)
            {
                Destroy(t);
            }
            mi.DetachChildren();
            menuOptions.Clear();
            menuElements.Clear();
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
            StaticData.findDeepChild(menuOptions[menuIdx].transform, "Text")
                .GetComponent<TextMeshProUGUI>().color = Color.cyan;

            selectionMode = SelectionMode.SELECT_GEM;

        }
        else if (choice == MenuChoice.PICKED_GEM)
        {
            selectedUnit.heldItem = moveDest.getGemstones()[menuIdx];
            moveDest.getGemstones().Remove((Gemstone)selectedUnit.heldItem);

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
            /*
            //TODO show status page
            Destroy(instantiatedMenuBackground);
            foreach (TextMeshProUGUI t in menuOptions)
            {
                Destroy(t);
            }

            cam.transform.position = new Vector3(0, 0, CAMERA_LAYER);
            Camera camCam = cam.GetComponent<Camera>();
            camCam.orthographicSize = (float)17.5;
            instantiatedMapHUD.SetActive(false);
            statusPage.SetActive(true);
            Transform canvas = statusPage.transform.GetChild(0);
            canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = chapterName;
            canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = teamNames[0] + ": " + player.Count;
            canvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = teamNames[1] + ": " + enemy.Count;
            canvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = teamNames[2] + (ally.Count > 0 ? (": " + ally.Count) : "");
            canvas.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Objective: " + objective.getName() + "\n\nDefeat: " + objective.getFailure();
            canvas.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Turn " + turn;
            canvas.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = "(Par: " + turnPar + ")";
            canvas.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "Iron: " + CampaignData.iron
                + "\nSteel: " + CampaignData.steel
                + "\nSilver: " + CampaignData.silver;

            selectionMode = SelectionMode.STATUS;
        }
        else if (choice == MenuChoice.CONTROLS)
        {
            //TODO show controls page
            Destroy(instantiatedMenuBackground);
            foreach (TextMeshProUGUI t in menuOptions)
            {
                Destroy(t);
            }
            cam.transform.position = new Vector3(0, 0, CAMERA_LAYER);
            Camera camCam = cam.GetComponent<Camera>();
            camCam.orthographicSize = (float)17.5;
            instantiatedMapHUD.SetActive(false);
            controlsPage.SetActive(true);

            selectionMode = SelectionMode.CONTROLS;
        }
        else if (choice == MenuChoice.END)
        {
            Destroy(instantiatedMenuBackground);
            foreach (TextMeshProUGUI t in menuOptions)
            {
                Destroy(t);
            }

            foreach (Tile t in healTiles)
            {
                if (t.getOccupant() != null && t.getOccupant().team == Unit.UnitTeam.ENEMY)
                {
                    t.getOccupant().heal(10);
                }
            }
            enemyIdx = -1; //ENEMYPHASE_SELECT_UNIT increments it to 0 first
            instantiatedMapHUD.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = Color.red;
            selectionMode = SelectionMode.ENEMYPHASE_SELECT_UNIT;

            */
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
        //TODO use A Star

        return new Vector3[] { moveDest.getStage().position };
    }

    private void createForecast()
    {
        unfillAttackableTiles();
        targetTile = map[cursorX, cursorY];
        targetEnemy = targetTile.getOccupant().getUnit();

        int[] forecast = Battle.getForecast(selectedUnit.model, targetEnemy.model, player, enemy,
            selectedUnit.getEquippedWeapon(), targetEnemy.getEquippedWeapon(), moveDest, targetTile);

        if (selectedUnit.fusionSkillBonus == Unit.FusionSkill.FUTURE_VISION
            ||selectedUnit.fusionSkill1 == Unit.FusionSkill.FUTURE_VISION
            || selectedUnit.fusionSkill2 == Unit.FusionSkill.FUTURE_VISION)
        {
            Battle result = new Battle(selectedUnit.model, targetEnemy.model, player, enemy,
                selectedUnit.getEquippedWeapon(), targetEnemy.getEquippedWeapon(), moveDest, targetTile);
            //TODO
        }
        else
        {
            enableChild("Forecast", true);

            StaticData.findDeepChild(transform, "PlayerName").GetComponent<TextMeshProUGUI>()
                .text = selectedUnit.unitName;
            StaticData.findDeepChild(transform, "PlayerPortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(selectedUnit.unitName);
            StaticData.findDeepChild(transform, "PlayerHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[0];
            StaticData.findDeepChild(transform, "PlayerATK").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[1] - forecast[8])}" + (forecast[5] != 1 ? $" x {forecast[5]}" : "");
            StaticData.findDeepChild(transform, "PlayerHIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[3])}";
            StaticData.findDeepChild(transform, "PlayerCRIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[4])}";
            if (selectedUnit.getEquippedWeapon() == null)
            {
                StaticData.findDeepChild(transform, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                    .text = "-";
                //TODO set image
                StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                    .sprite = null;
            }
            else
            {
                StaticData.findDeepChild(transform, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                    .text = selectedUnit.getEquippedWeapon().itemName;
                //TODO set image
                StaticData.findDeepChild(transform, "PlayerWeaponImage").GetComponent<Image>()
                    .sprite = null;
            }


            StaticData.findDeepChild(transform, "EnemyName").GetComponent<TextMeshProUGUI>()
                .text = targetEnemy.unitName;
            StaticData.findDeepChild(transform, "EnemyPortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(targetEnemy.unitName);
            StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
                .text = "" + forecast[6];
            StaticData.findDeepChild(transform, "EnemyATK").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[7] - forecast[2])}" + (forecast[11] != 1 ? $" x {forecast[11]}" : "");
            StaticData.findDeepChild(transform, "EnemyHIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[9])}";
            StaticData.findDeepChild(transform, "EnemyCRIT").GetComponent<TextMeshProUGUI>()
                .text = $"{Mathf.Max(0, forecast[10])}";
            if (targetEnemy.getEquippedWeapon() == null)
            {
                StaticData.findDeepChild(transform, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                    .text = "-";
                //TODO set image
                StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                    .sprite = null;
            }
            else
            {
                StaticData.findDeepChild(transform, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                    .text = targetEnemy.getEquippedWeapon().itemName;
                //TODO set image
                StaticData.findDeepChild(transform, "EnemyWeaponImage").GetComponent<Image>()
                    .sprite = null;
            }
        }

        selectionMode = SelectionMode.FORECAST;
    }
    private void startBattle()
    {
        //TODO

        selectionMode = SelectionMode.BATTLE;
    }

    private void performTalk()
    {
        //TODO

        selectionMode = SelectionMode.IN_CONVO;
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
        SELECT_GEM, STATUS, STATS_PAGE, CONTROLS, NOTIFICATION, ITEM_MENU, SELECT_TRADER, SELECT_WEAPON_TRADER, USE_ITEM,
        ENEMYPHASE_SELECT_UNIT, ENEMYPHASE_MOVE, ENEMYPHASE_ATTACK, ENEMYPHASE_BURN, ENEMYPHASE_COMBAT_PAUSE,
        ALLYPHASE_SELECT_UNIT, ALLYPHASE_MOVE, ALLYPHASE_ATTACK, ALLYPHASE_BURN, ALLYPHASE_COMBAT_PAUSE,
        STANDBY, GAMEOVER, ESCAPE_MENU
    }

    public enum MenuChoice
    {
        TALK, ATTACK, ESCAPE, SEIZE, ITEM, WEAPON, GEM, PICKED_GEM, CHEST, WAIT, STATUS, CONTROLS, END,
        USE_PERSONAL, USE_HELD, TRADE, DROP, EQUIP_PERSONAL, EQUIP_HELD, EQUIP_NONE, TRADE_WEAPON,
        DROP_WEAPON
    }


}

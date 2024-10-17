using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class PreBattleMenu : SequenceMember
{

    private int width;
    private int height;

    public List<Unit> enemy;
    public List<Unit> ally;
    public List<Unit> other;
    private int numUnitsAllowed;

    [SerializeField] private UnitModel unitModelPrefab;

    private Objective objective;
    private string chapterName;

    private Tile[,] map;
    private List<Tile> playerTiles;
    private int turnPar;

    private Unit statsUnit;

    [SerializeField] private Transform cursor;
    [SerializeField] private float cameraDistance;
    private int camOrientation;
    private Quaternion playerRotation;

    public int cursorX;
    public int cursorY;

    private Dictionary<Tile, object> traversableTiles;
    private Dictionary<Tile, object> attackableTiles;
    private Tile selectedTile;
    private Unit selectedUnit;
    private int itemType;
    private int selectedItem;

    private int menuIdx;
    private int specialMenuIdx;

    private string[] teamNames;

    private AudioSource music;

    private SelectionMode selectionMode;
    private bool done = false;

    public void constructor(Tile[,] map, List<Tile> playerTiles,
        Unit[] enemyUnits, Unit[] allyUnits, Unit[] otherUnits, Quaternion playerRotation,
        Objective objective, string chapterName, string[] teamNames, int turnPar, string prepMusic)
    {
        enemy = new List<Unit>(enemyUnits);
        ally = new List<Unit>(allyUnits);
        other = new List<Unit>(otherUnits);
        numUnitsAllowed = StaticData.positions.Length;
        this.objective = objective;
        this.chapterName = chapterName;

        this.map = map;
        this.playerTiles = playerTiles;
        this.teamNames = teamNames;
        this.turnPar = turnPar;
        this.playerRotation = playerRotation;

        for (int q = 0; q < StaticData.positions.Length; q++)
        {
            int idx = StaticData.positions[q];
            if (idx != -1)
            {
                createAndPlaceUnit(StaticData.members[idx], playerTiles[q]);
            }
        }

        constructPickUnits();
        constructItemMenu();

        initializeCursorPosition();

        music = getAudioSource(prepMusic);
        music.loop = true;
        music.Play();

        enableChild("MainMenu", true);
    }

    private void enableChild(string hudName, bool enable)
    {
        StaticData.findDeepChild(transform, hudName).gameObject.SetActive(enable);
    }

    public void setTooltip(int menuIdx)
    {
        if (menuIdx == 0)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Pick a limited number of units to deploy";
        }
        else if (menuIdx == 1)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Give units weapons and items";
        }
        else if (menuIdx == 2)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Look at the map to change your battle positions and plan your strategy";
        }
        else if (menuIdx == 3)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Save your progress along with your units' inventories and battle positions";
        }
        else if (menuIdx == 4)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Begin the battle! REMEMBER TO SAVE FIRST!";
        }
        else if (menuIdx == 5)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Exit the chapter. REMEMBER TO SAVE FIRST!";
        }
    }
    public void initializeCursorPosition()
    {
        Tile tile = playerTiles[0];
        setCursor(tile);
        setCameraPosition();
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
    public void createAndPlaceUnit(Unit unit, Tile tile)
    {
        Transform pos = tile.getStage();
        UnitModel myUnit = Instantiate(unitModelPrefab,
            new Vector3(pos.position.x, pos.position.y, pos.position.z), Quaternion.identity);
        myUnit.setUnit(unit);
        tile.setOccupant(myUnit);
        myUnit.setStandingRotation(playerRotation);
        myUnit.equip();
    }
    private void constructPickUnits()
    {
        Transform list = StaticData.findDeepChild(transform, "UnitsToPick");
        Transform optionPrefab = StaticData.findDeepChild(transform, "UnitOption");
        for (int q = 0; q < StaticData.members.Count; q++)
        {
            Unit unit = StaticData.members[q];
            if (unit.isAlive())
            {
                Transform option = Instantiate(optionPrefab, list);
                option.gameObject.SetActive(true);

                StaticData.findDeepChild(option, "UnitOptionPortrait").GetComponent<Image>()
                    .sprite = AssetDictionary.getPortrait(unit.unitName);
                Button.ButtonClickedEvent stats = new Button.ButtonClickedEvent();
                stats.AddListener(delegate { statsPage(unit); selectionMode = SelectionMode.STATS_PAGE_PICK_UNITS; });
                StaticData.findDeepChild(option, "UnitOptionPortrait").GetComponent<Button>()
                    .onClick = stats;

                StaticData.findDeepChild(option, "UnitOptionName").GetComponent<TextMeshProUGUI>()
                    .text = unit.unitName;

                updateDeploymentToggle(unit, option);
                Button.ButtonClickedEvent deploy = new Button.ButtonClickedEvent();
                deploy.AddListener(delegate { toggleUnitDeployment(unit, option); });
                StaticData.findDeepChild(option, "DeployedToggle").GetComponent<Button>()
                    .onClick = deploy;
            }
        }
    }
    private void toggleUnitDeployment(Unit unit, Transform deployOption)
    {
        if (unit.deployed)
        {
            unit.deployed = false;
            unit.model.getTile().setOccupant(null);
            Destroy(unit.model.gameObject);
            int idx = StaticData.members.IndexOf(unit);
            for (int q = 0; q < StaticData.positions.Length; q++)
            {
                if (StaticData.positions[q] == idx)
                {
                    StaticData.positions[q] = -1;
                }
            }
            playOneTimeSound("back");
        }
        else
        {
            int idx = -1;
            for (int q = 0; q < StaticData.positions.Length; q++)
            {
                if (StaticData.positions[q] == -1)
                {
                    idx = q;
                    break;
                }
            }
            if (idx == -1)
            {
                playOneTimeSound("back");
            }
            else
            {
                unit.deployed = true;
                StaticData.positions[idx] = StaticData.members.IndexOf(unit);
                createAndPlaceUnit(unit, playerTiles[idx]);
                playOneTimeSound("select");
            }
        }
        updateDeploymentToggle(unit, deployOption);
    }
    private void updateDeploymentToggle(Unit unit, Transform deployOption)
    {
        if (unit.deployed)
        {
            StaticData.findDeepChild(deployOption, "DeployedToggle").GetComponent<Image>()
                .sprite = AssetDictionary.getImage("green-check");
            StaticData.findDeepChild(deployOption, "DeployedToggle").GetComponent<Image>()
                .color = Color.white;
        }
        else
        {
            StaticData.findDeepChild(deployOption, "DeployedToggle").GetComponent<Image>()
                .sprite = null;
            StaticData.findDeepChild(deployOption, "DeployedToggle").GetComponent<Image>()
                .color = Color.black;
        }
    }
    public void backFromPickUnits()
    {
        enableChild("PickUnits", false);
        enableChild("MainMenu", true);

        Transform list = StaticData.findDeepChild(transform, "UnitsToPick");
        for (int q = 0; q < list.childCount; q++)
        {
            Transform child = list.GetChild(q);
            if (StaticData.findDeepChild(child, "DeployedToggle").GetComponent<Image>()
                .color == Color.black)
            {
                child.SetParent(null);
                child.SetParent(list);
            }
        }
    }

    private void constructItemMenu()
    {
        Transform unitList = StaticData.findDeepChild(transform, "UnitsForConvoy");
        Transform unitOption = StaticData.findDeepChild(transform, "ConvoyUnit");
        foreach (Unit unit in StaticData.members)
        {
            if (unit.isAlive())
            {
                Transform option = Instantiate(unitOption, unitList);
                option.gameObject.SetActive(true);

                Button.ButtonClickedEvent pick = new Button.ButtonClickedEvent();
                pick.AddListener(delegate { selectUnitForConvoy(unit); });
                option.GetComponent<Button>().onClick = pick;

                StaticData.findDeepChild(option, "ConvoyUnitPortrait").GetComponent<Image>()
                    .sprite = AssetDictionary.getPortrait(unit.unitName);
                Button.ButtonClickedEvent stats = new Button.ButtonClickedEvent();
                stats.AddListener(delegate { statsPage(unit); selectionMode = SelectionMode.STATS_PAGE_ITEMS; });
                StaticData.findDeepChild(option, "ConvoyUnitPortrait").GetComponent<Button>()
                    .onClick = stats;

                StaticData.findDeepChild(option, "ConvoyUnitName").GetComponent<TextMeshProUGUI>()
                    .text = unit.unitName;
            }
        }
        switchToItemType(0);
    }

    private void selectUnitForConvoy(Unit unit)
    {
        selectedUnit = unit;
        enableChild("PickUnitForConvoy", false);
        enableChild("UnitPickedForConvoy", true);

        StaticData.findDeepChild(transform, "PickedConvoyUnitPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(unit.unitName);
        Button.ButtonClickedEvent stats = new Button.ButtonClickedEvent();
        stats.AddListener(delegate { statsPage(unit); selectionMode = SelectionMode.STATS_PAGE_ITEMS; });
        StaticData.findDeepChild(transform, "PickedConvoyUnitPortrait").GetComponent<Button>()
            .onClick = stats;


        if (unit.personalItem == null)
        {
            enableChild("ConvoyPersonalItem", false);
        }
        else
        {
            enableChild("ConvoyPersonalItem", true);
            StaticData.findDeepChild(transform, "ConvoyPersonalItemName").GetComponent<TextMeshProUGUI>()
                .text = unit.personalItem.itemName
                + (unit.personalItem.uses > 0 ? $" {unit.personalItem.usesLeft}/{unit.personalItem.uses}"
                : " --/--");
            StaticData.findDeepChild(transform, "ConvoyPersonalItemIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(unit.personalItem is Weapon ? Weapon.weaponTypeName(((Weapon)unit.personalItem).weaponType)
                : "Item");
        }
        if (unit.heldWeapon == null)
        {
            enableChild("ConvoyHeldWeapon", false);
        }
        else
        {
            enableChild("ConvoyHeldWeapon", true);
            StaticData.findDeepChild(transform, "ConvoyHeldWeaponName").GetComponent<TextMeshProUGUI>()
                .text = unit.heldWeapon.itemName
                + (unit.heldWeapon.uses > 0 ? $" {unit.heldWeapon.usesLeft}/{unit.heldWeapon.uses}"
                : " --/--");
            StaticData.findDeepChild(transform, "ConvoyHeldWeaponIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(unit.heldWeapon.weaponType));
        }
        if (unit.heldItem == null)
        {
            enableChild("ConvoyHeldItem", false);
        }
        else
        {
            enableChild("ConvoyHeldItem", true);
            StaticData.findDeepChild(transform, "ConvoyHeldItemName").GetComponent<TextMeshProUGUI>()
                .text = unit.heldItem.itemName
                + (unit.heldItem.uses > 0 ? $" {unit.heldItem.usesLeft}/{unit.heldItem.uses}"
                : " --/--");
        }
    }

    public void backFromConvoyUnit()
    {
        enableChild("UnitPickedForConvoy", false);
        enableChild("PickUnitForConvoy", true);
        playOneTimeSound("back");
    }

    public void switchToItemType(int type)
    {
        itemType = type;
        Transform itemsList = StaticData.findDeepChild(transform, "ConvoyItems");
        Transform optionPrefab = StaticData.findDeepChild(transform, "ConvoyItem");

        for (int q = 0; q < itemsList.childCount; q++)
        {
            Destroy(itemsList.GetChild(q).gameObject);
        }
        itemsList.DetachChildren();

        for (int q = 0; q < StaticData.convoyIds[type].Count; q++)
        {
            Transform option = Instantiate(optionPrefab, itemsList);
            Item item = Item.itemIndex[StaticData.convoyIds[type][q]];
            int usesLeft = StaticData.convoyDurabilities[type][q];

            int idx = q;
            Button.ButtonClickedEvent pick = new Button.ButtonClickedEvent();
            pick.AddListener(delegate { pickItem(idx, true); selectionMode = SelectionMode.ITEM_MENU_CONVOY; });
            option.GetComponent<Button>().onClick = pick;

            StaticData.findDeepChild(option, "ConvoyItemName").GetComponent<TextMeshProUGUI>()
                .text = item.uses > 0 ? $"{item.itemName} {usesLeft}/{item.uses}" : $"{item.itemName} --/--";

            string iconName = item is Weapon ? Weapon.weaponTypeName(((Weapon)item).weaponType)
                : "Item";
            StaticData.findDeepChild(option, "ConvoyItemIcon").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(iconName);
        }
    }

    public void pickInventoryItem(int idx)
    {
        pickItem(idx, false);
        selectionMode = SelectionMode.ITEM_MENU_INVENTORY;
    }
    private void pickItem(int idx, bool convoy)
    {
        selectedItem = idx;
        if (convoy)
        {
            StaticData.findDeepChild(transform, "ConvoyItemDesc").GetComponent<TextMeshProUGUI>()
                .text = Item.itemIndex[StaticData.convoyIds[itemType][idx]].description();
        }
        else
        {
            Item item = idx == 0 ? selectedUnit.personalItem : idx == 1 ? selectedUnit.heldWeapon : selectedUnit.heldItem;
            StaticData.findDeepChild(transform, "ConvoyItemDesc").GetComponent<TextMeshProUGUI>()
                .text = item.description();
        }
    }

    public void giveItemToUnit()
    {
        if (selectedUnit == null || selectionMode != SelectionMode.ITEM_MENU_CONVOY)
        {
            playOneTimeSound("back");
            return;
        }
        Item reference = Item.itemIndex[StaticData.convoyIds[itemType][selectedItem]];
        if ((reference is Weapon && selectedUnit.heldWeapon != null)
            || (!(reference is Weapon) && selectedUnit.heldItem != null))
        {
            playOneTimeSound("back");
            return;
        }
        Item item = StaticData.takeFromConvoy(itemType, selectedItem);
        if (item is Weapon)
        {
            selectedUnit.heldWeapon = (Weapon)item;
        }
        else
        {
            selectedUnit.heldItem = item;
        }
        if (selectedUnit.deployed)
        {
            selectedUnit.model.equip();
        }
        playOneTimeSound("select");
    }

    public void giveItemToConvoy()
    {
        if (selectionMode != SelectionMode.ITEM_MENU_INVENTORY || selectedItem == 0)
        {
            playOneTimeSound("back");
            return;
        }
        if (selectedItem == 1)
        {
            Weapon wep = selectedUnit.heldWeapon;
            selectedUnit.heldWeapon = null;
            StaticData.addToConvoy(wep);
        }
        else if (selectedItem == 2)
        {
            Item item = selectedUnit.heldItem;
            selectedUnit.heldItem = null;
            StaticData.addToConvoy(item);
        }
        if (selectedUnit.deployed)
        {
            selectedUnit.model.equip();
        }
        playOneTimeSound("select");
    }

    public void backFromConvoy()
    {
        backFromConvoyUnit();
        switchToItemType(0);
        enableChild("Convoy", false);
        enableChild("MainMenu", true);
    }

    public override void LEFT_MOUSE()
    {
        //TODO
    }
    public override void RIGHT_MOUSE()
    {
        if (selectionMode == SelectionMode.ROAM)
        {
            enableChild("MainMenu", true);
            selectionMode = SelectionMode.MENU;
        }
        else if (selectionMode == SelectionMode.MOVE)
        {
            unfillTraversableTiles();
            selectionMode = SelectionMode.ROAM;
            //                selectedUnit = null;
        }
        else if (selectionMode == SelectionMode.SWITCH)
        {
            selectedTile.highlightMove();

            selectionMode = SelectionMode.ROAM;
        }
        else if (selectionMode == SelectionMode.STATS_PAGE)
        {
            enableChild("StatsPage", false);

            selectionMode = SelectionMode.ROAM;
        }
    }
    public override void Z()
    {
        /*
        if (selectionMode == SelectionMode.MAIN_MENU)
        {
            if (mainMenuIdx == 0)
            {
                Destroy(instantiatedMainMenu);
                constructPickUnits();

                selectionMode = SelectionMode.PICK_UNITS;
            }
            else if (mainMenuIdx == 1)
            {
                //TODO
                Destroy(instantiatedMainMenu);
                constructItemMenu();

                selectionMode = SelectionMode.ITEM_MENU_PICK_UNIT;
            }
            else if (mainMenuIdx == 2)
            {
                Destroy(instantiatedMainMenu);
                initializeCamera();
                instantiatedMapHUD.SetActive(true);
                foreach (Tile t in playerTiles)
                {
                    t.traverseHighlight.SetActive(true);
                }
                selectionMode = SelectionMode.ROAM;
            }
            else if (mainMenuIdx == 3)
            {
                CampaignData.chapterPrep = CampaignData.scene;
                CampaignData.positions = player.ToArray();
                Destroy(instantiatedMainMenu);
                instantiatedSaveScreen = Instantiate(saveScreen);
                instantiatedSaveScreen.constructor(cam.GetComponent<Camera>());
                selectionMode = SelectionMode.SAVE;
            }
            else if (mainMenuIdx == 4)
            {
                //TODO anything else
                finish();
            }
            else if (mainMenuIdx == 5)
            {
                SpecialMenuLogic.mainMenu(instantiatableUnit);
            }
        }
        if (selectionMode == SelectionMode.SAVE)
        {
            instantiatedSaveScreen.Z();
        }
        */
        if (selectionMode == SelectionMode.ROAM)
        {
            if (playerTiles.Contains(map[cursorX, cursorY]))
            {
                selectedTile = map[cursorX, cursorY];
                selectedTile.highlightInteract();

                selectionMode = SelectionMode.SWITCH;
            }
            else if (map[cursorX, cursorY].getOccupant() != null)
            {
                selectedTile = map[cursorX, cursorY];
                selectedUnit = selectedTile.getOccupant().getUnit();
                fillTraversableTiles(selectedUnit, cursorX, cursorY);
                selectionMode = SelectionMode.MOVE;
            }
        }
        else if (selectionMode == SelectionMode.SWITCH)
        {
            if (playerTiles.Contains(map[cursorX, cursorY]))
            {
                Tile second = map[cursorX, cursorY];
                Unit temp = second.getOccupant().getUnit();
                second.setOccupant(selectedTile.getOccupant());
                selectedTile.setOccupant(temp.model);
                selectedTile.highlightInteract();

                int pos1 = playerTiles.IndexOf(selectedTile);
                int pos2 = playerTiles.IndexOf(second);
                int temporary = StaticData.positions[pos1];
                StaticData.positions[pos1] = StaticData.positions[pos2];
                StaticData.positions[pos2] = temporary;

                selectionMode = SelectionMode.ROAM;
            }
        }
        /*
        else if (selectionMode == SelectionMode.PICK_UNITS)
        {
            Unit u = pickerUnits[pickCursorX, pickCursorY];
            if (!u.isLeader && u.isAlive())
            {
                if (u.deployed)
                {
                    u.deployed = false;
                    int idx = CampaignData.members.IndexOf(u);
                    int mapIdx = player.IndexOf(idx);
                    playerTiles[mapIdx].setOccupant(null);
                    player[mapIdx] = -1;
                    u.transform.position = unusedPosition;
                    SpriteRenderer sr = pickerArray[pickCursorX, pickCursorY].transform.GetChild(1).GetComponent<SpriteRenderer>();
                    sr.sprite = ImageDictionary.getImage("blank_square");
                    sr.color = Color.blue;
                }
                else if (player.IndexOf(-1) != -1)
                {
                    u.deployed = true;
                    int idx = CampaignData.members.IndexOf(u);
                    int mapIdx = player.IndexOf(-1);
                    playerTiles[mapIdx].setOccupant(u);
                    player[mapIdx] = idx;
                    SpriteRenderer sr = pickerArray[pickCursorX, pickCursorY].transform.GetChild(1).GetComponent<SpriteRenderer>();
                    sr.size = new Vector2(1, 1);
                    sr.sprite = ImageDictionary.getImage("checkmark.png");
                    sr.color = Color.white;
                }
            }
        }
        else if (selectionMode == SelectionMode.ITEM_MENU_PICK_UNIT)
        {
            if (CampaignData.members[unitForConvoyIdx].isAlive())
            {
                inventoryIdx = 0;
                selectedUnit = CampaignData.members[unitForConvoyIdx];
                instantiatedItemsMenu.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;
                updateItemDescription(SelectionMode.ITEM_MENU_INVENTORY);

                selectionMode = SelectionMode.ITEM_MENU_INVENTORY;
            }
        }
        else if (selectionMode == SelectionMode.ITEM_MENU_INVENTORY)
        {
            if (inventoryIdx == 1 && CampaignData.members[unitForConvoyIdx].heldWeapon != null)
            {
                Weapon w = CampaignData.members[unitForConvoyIdx].heldWeapon;
                CampaignData.addToConvoy(w);
                CampaignData.members[unitForConvoyIdx].heldWeapon = null;
                switchToItemType(itemTypeIdx, false);
                instantiatedItemsMenu.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text
                    = "_";
            }
            else if (inventoryIdx == 2 && CampaignData.members[unitForConvoyIdx].heldItem != null)
            {
                Item w = CampaignData.members[unitForConvoyIdx].heldItem;
                CampaignData.addToConvoy(w);
                CampaignData.members[unitForConvoyIdx].heldItem = null;
                switchToItemType(itemTypeIdx, false);
                instantiatedItemsMenu.transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text
                    = "_";
            }
        }
        else if (selectionMode == SelectionMode.ITEM_MENU_CONVOY)
        {
            if (itemScrollListMembers.Count > 0)
            {
                if (itemTypeIdx == CampaignData.getConvoyIds().Length - 1 && selectedUnit.heldItem == null)
                {
                    Item item = CampaignData.takeFromConvoy(itemTypeIdx, itemIdx);
                    Transform itemInList = instantiatedItemsMenu.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0)
                        .GetChild(itemIdx);
                    itemInList.SetParent(null);
                    itemScrollListMembers.RemoveAt(itemIdx);
                    itemIdx = Mathf.Max(0, itemIdx - 1);
                    if (itemScrollListMembers.Count > 0)
                    {
                        itemScrollListMembers[itemIdx].color = Color.white;
                    }
                    selectedUnit.heldItem = item;
                    instantiatedItemsMenu.transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text
                        = item.itemName;
                }
                else if (itemTypeIdx < CampaignData.getConvoyIds().Length - 1 && selectedUnit.heldWeapon == null)
                {
                    Item item = CampaignData.takeFromConvoy(itemTypeIdx, itemIdx);
                    Transform itemInList = instantiatedItemsMenu.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0)
                        .GetChild(itemIdx);
                    itemInList.SetParent(null);
                    itemScrollListMembers.RemoveAt(itemIdx);
                    itemIdx = Mathf.Max(0, itemIdx - 1);
                    if (itemScrollListMembers.Count > 0)
                    {
                        itemScrollListMembers[itemIdx].color = Color.white;
                    }
                    selectedUnit.heldWeapon = (Weapon)item;
                    instantiatedItemsMenu.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text
                        = item.itemName;
                }
            }
        }
        */
    }
    public override void X()
    {
        RIGHT_MOUSE();
    }
    public override void A()
    {
        if (selectionMode == SelectionMode.ROAM && map[cursorX, cursorY].getOccupant() != null)
        {
            selectedTile = map[cursorX, cursorY];
            selectedUnit = selectedTile.getOccupant().getUnit();
            statsPage(selectedUnit);

            selectionMode = SelectionMode.STATS_PAGE;
        }
    }
    public override void UP()
    {
        if ((selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.SWITCH
            || selectionMode == SelectionMode.MOVE)
            && cursorY != height - 1)
        {
            moveCursor(0, 1);
        }
    }
    public override void LEFT()
    {
        if ((selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.SWITCH
            || selectionMode == SelectionMode.MOVE)
            && cursorX != 0)
        {
            moveCursor(-1, 0);
        }
    }
    public override void DOWN()
    {
        if ((selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.SWITCH
          || selectionMode == SelectionMode.MOVE)
          && cursorY != 0)
        {
            moveCursor(0, -1);
        }
    }
    public override void RIGHT()
    {
        if ((selectionMode == SelectionMode.ROAM || selectionMode == SelectionMode.SWITCH
            || selectionMode == SelectionMode.MOVE)
                    && cursorX != width - 1)
        {
            moveCursor(1, 0);
        }
    }
    public override void ENTER()
    {
        if (selectionMode == SelectionMode.MAIN_MENU)
        {
            finish();
        }
    }
    public override bool completed()
    {
        return done;
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


    private void addKeyAndValue(Dictionary<Tile, object> dict, Tile key, object val)
    {
        if (dict.ContainsKey(key))
        {
            dict.Remove(key);
        }
        dict.Add(key, val);
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

    // Update is called once per frame
    void Update()
    {

    }

    public void finish()
    {
        selectionMode = SelectionMode.STANDBY;

        foreach (Tile tile in playerTiles)
        {
            if (tile.getOccupant() != null)
            {
                Destroy(tile.getOccupant().gameObject);
            }
        }
        foreach (Unit unit in enemy)
        {
            Destroy(unit.model.gameObject);
        }
        foreach (Unit unit in ally)
        {
            Destroy(unit.model.gameObject);
        }
        foreach (Unit unit in other)
        {
            Destroy(unit.model.gameObject);
        }
        Destroy(music.gameObject);

        done = true;
    }

    public enum SelectionMode
    {
        MAIN_MENU, ROAM, SWITCH, MOVE, MENU, SELECT_WEAPON, MAP_MENU, STATUS, STATS_PAGE,
        STATS_PAGE_PICK_UNITS, STATS_PAGE_ITEMS,
        ITEM_MENU_PICK_UNIT, ITEM_MENU_INVENTORY, ITEM_MENU_CONVOY, PICK_UNITS,
        STANDBY, ESCAPE_MENU, SAVE
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CrystalBase : SequenceMember
{

    [SerializeField] private UnitModel unitModelPrefab;
    [SerializeField] private SparringBattleAnimation sparAnimation;

    private Unit statsUnit;

    private Unit selectedUnit;
    private int itemType;
    private int selectedItem;

    private int menuIdx;
    private int specialMenuIdx;

    private AudioSource music;

    private SelectionMode selectionMode;
    private bool done;

    private List<Unit> sparPartners;

    private int recruitQuoteIdx;

    private int comparativeEXP;
    private int givenEXP;

    private SparringBattleAnimation instantiatedSparAnimation;

    public void constructor(string prepMusic)
    {
        constructItemMenu();

        music = getAudioSource(prepMusic);
        music.loop = true;
        music.Play();

        switchToPage("MainMenu");
        selectionMode = SelectionMode.MAIN_MENU;
    }

    public void setTooltip(int menuIdx)
    {
        enableChild("Tooltip", true);
        if (menuIdx == 0)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Train your units by pitting them against each other";
        }
        else if (menuIdx == 1)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Attempt to recruit the Gems you've captured";
        }
        else if (menuIdx == 2)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Give units weapons and items";
        }
        else if (menuIdx == 3)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Give units Bonus EXP that you've earned by completing chapters early";
        }
        else if (menuIdx == 4)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Build relationships between units who have spent enough time on the battlefield together";
        }
        else if (menuIdx == 5)
        {
            if (StaticData.getUnitByName("Bismuth") == null)
            {
                StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                    = "Have Bismuth make weapons with materials you've collected";
            }
            else
            {
                StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                    = "See what Bismuth might have left behind";
            }
        }
        else if (menuIdx == 6)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Manage animation and sound settings";
        }
        else if (menuIdx == 7)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Save your progress along with your units' inventories";
        }
        else if (menuIdx == 8)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Start the mission. You won't be able to return to the base during this chapter. REMEMBER TO SAVE FIRST!";
        }
        else if (menuIdx == 9)
        {
            StaticData.findDeepChild(transform, "Tooltip").GetComponent<TextMeshProUGUI>().text
                = "Return to the game's main menu. REMEMBER TO SAVE FIRST!";
        }
    }

    private void enableChild(string hudName, bool enable)
    {
        StaticData.findDeepChild(transform, hudName).gameObject.SetActive(enable);
    }
    public void switchToPage(string page)
    {
        Transform canvas = StaticData.findDeepChild(transform, "Canvas");
        for (int q = 0; q < canvas.childCount; q++)
        {
            canvas.GetChild(q).gameObject.SetActive(false);
        }
        enableChild(page, true);
        enableChild("Tooltip", false);
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
        selectionMode = SelectionMode.ITEM_MENU_PICK_UNIT;
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
            option.gameObject.SetActive(true);
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
        switchToItemType(itemType);
        selectUnitForConvoy(selectedUnit);
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
        switchToItemType(itemType);
        selectUnitForConvoy(selectedUnit);
        playOneTimeSound("select");
    }

    public void backFromConvoy()
    {
        backFromConvoyUnit();
        switchToItemType(0);
        enableChild("Convoy", false);
        enableChild("MainMenu", true);
        selectionMode = SelectionMode.MAIN_MENU;
    }

    public void saveFile(int file)
    {
        SaveMechanism.saveGame(file);
        backFromSave();
    }
    public void backFromSave()
    {
        enableChild("SaveMenu", false);
        enableChild("MainMenu", true);
        selectionMode = SelectionMode.MAIN_MENU;
    }
    public void toGameMenu()
    {
        SpecialMenuLogic.mainMenu();
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
                    .sprite = AssetDictionary.getImage("Item");
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
                .sprite = AssetDictionary.getImage("Item");
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
    public void nextStatsPage()
    {
        specialMenuIdx = (specialMenuIdx + 1) % 3;
    }
    public void prevStatsPage()
    {
        specialMenuIdx--;
        if (specialMenuIdx == -1)
        {
            specialMenuIdx = 2;
        }
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
    public void switchToConvoy()
    {
        switchToPage("Convoy");
        selectionMode = SelectionMode.ITEM_MENU_PICK_UNIT;
    }

    public void switchToSparring()
    {
        Transform content = StaticData.findDeepChild(transform, "UnitsForSparContent");
        Transform prefab = StaticData.findDeepChild(transform, "UnitForSpar");

        for (int q = 0; q < content.childCount; q++)
        {
            Destroy(content.GetChild(q).gameObject);
        }
        content.DetachChildren();

        sparPartners = new List<Unit>();

        foreach (Unit u in StaticData.members)
        {
            Transform opt = Instantiate(prefab, content);
            opt.gameObject.SetActive(true);

            opt.GetComponent<Image>().sprite = AssetDictionary.getPortrait(u.unitName);
            if (u.isExhausted)
            {
                opt.GetComponent<Button>().interactable = false;
            }

            StaticData.findDeepChild(opt, "UnitForSparName").GetComponent<TextMeshProUGUI>()
                .text = u.unitName;

            Button.ButtonClickedEvent optAction = new Button.ButtonClickedEvent();
            optAction.AddListener(delegate { selectUnitForSpar(u); });
            opt.GetComponent<Button>().onClick = optAction;
        }
    }

    private void selectUnitForSpar(Unit u)
    {
        if (sparPartners.Contains(u))
        {
            sparPartners.Remove(u);
            playOneTimeSound("back");
        }
        else if (sparPartners.Count < 2)
        {
            sparPartners.Add(u);
            playOneTimeSound("select");
        }
        else
        {
            playOneTimeSound("back");
        }

        if (sparPartners.Count > 0)
        {
            StaticData.findDeepChild(transform, "FirstSparUnit").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(sparPartners[0].unitName);
            StaticData.findDeepChild(transform, "FirstSparUnit").GetComponent<Image>()
                .color = Color.white;
        }
        else
        {
            StaticData.findDeepChild(transform, "FirstSparUnit").GetComponent<Image>()
                .sprite = null;
            StaticData.findDeepChild(transform, "FirstSparUnit").GetComponent<Image>()
                .color = Color.black;
        }
        if (sparPartners.Count > 1)
        {
            StaticData.findDeepChild(transform, "SecondSparUnit").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(sparPartners[1].unitName);
            StaticData.findDeepChild(transform, "SecondSparUnit").GetComponent<Image>()
                .color = Color.white;
        }
        else
        {
            StaticData.findDeepChild(transform, "SecondSparUnit").GetComponent<Image>()
                .sprite = null;
            StaticData.findDeepChild(transform, "SecondSparUnit").GetComponent<Image>()
                .color = Color.black;
        }
    }

    public void startSparring()
    {
        if (sparPartners.Count != 2)
        {
            playOneTimeSound("back");
            return;
        }

        int[] forecast = Battle.getSparringForecast(sparPartners[0].model, sparPartners[1].model,
            selectedUnit.getEquippedWeapon(), sparPartners[1].getEquippedWeapon());

        Transform forecastDisplay = null;

        if (sparPartners[0].hasSkill(Unit.FusionSkill.FUTURE_VISION)
            || sparPartners[1].hasSkill(Unit.FusionSkill.FUTURE_VISION))
        {
            forecastDisplay = StaticData.findDeepChild(transform, "FutureVision");

            Battle result = new Battle.SparBattle(sparPartners[0].model, sparPartners[1].model,
                sparPartners[0].getEquippedWeapon(), sparPartners[1].getEquippedWeapon());

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
            / (0.0f + sparPartners[0].maxHP);
        float dfdPotentialPercentage = (forecast[Battle.DFDHP] - ((forecast[Battle.ATKMT] - forecast[Battle.DFDDEF]) * forecast[Battle.ATKCOUNT]))
            / (0.0f + sparPartners[1].maxHP);
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
            .text = sparPartners[0].unitName;
        StaticData.findDeepChild(forecastDisplay, "PlayerPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(sparPartners[0].unitName, atkExpression);

        StaticData.findDeepChild(forecastDisplay, "EnemyName").GetComponent<TextMeshProUGUI>()
            .text = sparPartners[1].unitName;
        StaticData.findDeepChild(forecastDisplay, "EnemyPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(sparPartners[1].unitName, dfdExpression);

        if (sparPartners[0].getEquippedWeapon() == null)
        {
            StaticData.findDeepChild(forecastDisplay, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(forecastDisplay, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = null;
        }
        else
        {
            StaticData.findDeepChild(forecastDisplay, "PlayerWeapon").GetComponent<TextMeshProUGUI>()
                .text = sparPartners[0].getEquippedWeapon().itemName;
            StaticData.findDeepChild(forecastDisplay, "PlayerWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(sparPartners[0].getEquippedWeapon().weaponType));
        }

        if (sparPartners[1].getEquippedWeapon() == null)
        {
            StaticData.findDeepChild(forecastDisplay, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = "-";
            StaticData.findDeepChild(forecastDisplay, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = null;
        }
        else
        {
            StaticData.findDeepChild(forecastDisplay, "EnemyWeapon").GetComponent<TextMeshProUGUI>()
                .text = sparPartners[1].getEquippedWeapon().itemName;
            StaticData.findDeepChild(forecastDisplay, "EnemyWeaponImage").GetComponent<Image>()
                .sprite = AssetDictionary.getImage(Weapon.weaponTypeName(sparPartners[1].getEquippedWeapon().weaponType));
        }
    }

    public void confirmSpar()
    {
        enableChild("Forecast", false);
        enableChild("FutureVision", false);

        UnitModel partner0 = Instantiate(unitModelPrefab);
        UnitModel partner1 = Instantiate(unitModelPrefab);
        partner0.setUnit(sparPartners[0]);
        partner1.setUnit(sparPartners[1]);

        Battle battle = new Battle.SparBattle(partner0, partner1,
            sparPartners[0].getEquippedWeapon(), sparPartners[1].getEquippedWeapon());
        Battle.RNGSTORE = new List<int>();

        StaticData.findDeepChild(partner0.transform, "TeamCircle").gameObject.SetActive(false);
        StaticData.findDeepChild(partner1.transform, "TeamCircle").gameObject.SetActive(false);

        instantiatedSparAnimation = Instantiate(sparAnimation);
        music.Pause();

        instantiatedSparAnimation.constructor(battle, "spar-music", this);

        selectionMode = SelectionMode.SPARRING;
    }

    public void cancelSpar()
    {
        enableChild("Forecast", false);
        enableChild("FutureVision", false);
    }

    public void endBattleAnimation()
    {
        music.UnPause();
        Destroy(instantiatedSparAnimation.gameObject);
        switchToSparring();

        selectionMode = SelectionMode.MAIN_MENU;
    }

    public void switchToBubbles()
    {
        Transform content = StaticData.findDeepChild(transform, "BubblesContent");
        Transform prefab = StaticData.findDeepChild(transform, "UnitInBubble");

        enableChild("BubbleOptions", false);

        for (int q = 0; q < content.childCount; q++)
        {
            Destroy(content.GetChild(q).gameObject);
        }
        content.DetachChildren();

        foreach (Gemstone gem in StaticData.prisoners)
        {
            Transform opt = Instantiate(prefab, content);
            opt.gameObject.SetActive(true);

            StaticData.findDeepChild(opt, "BubblePortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(gem.unit.unitName);

            StaticData.findDeepChild(opt, "BubbleName").GetComponent<TextMeshProUGUI>()
                .text = gem.unit.unitName;

            Button.ButtonClickedEvent optAction = new Button.ButtonClickedEvent();
            optAction.AddListener(delegate { attemptRecruit(gem.unit); });
            opt.GetComponent<Button>().onClick = optAction;
        }
    }

    public void attemptRecruit(Unit u)
    {
        selectedUnit = u;
        enableChild("BubbleOptions", true);
    }

    public void attemptRecruit()
    {
        selectionMode = SelectionMode.RECRUIT;

        enableChild("RecruitScene", true);

        StaticData.findDeepChild(transform, "RecruitPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(selectedUnit.unitName);

        if (Random.Range(0, 100) < selectedUnit.recruitability)
        {
            recruitQuoteIdx = 0;
            StaticData.findDeepChild(transform, "RecruitSpeakerName").GetComponent<TextMeshProUGUI>()
                .text = selectedUnit.unitName;
            nextRecruitQuote();
        }
        else
        {
            StaticData.findDeepChild(transform, "RecruitSpeakerName").GetComponent<TextMeshProUGUI>()
                .text = "";
            StaticData.findDeepChild(transform, "RecruitDialogue").GetComponent<TextMeshProUGUI>()
                .text = $"{selectedUnit} refuses to join you.";
            selectedUnit = null;
        }
    }
    private void nextRecruitQuote()
    {
        if (selectedUnit == null || recruitQuoteIdx >= selectedUnit.recruitQuote.Length)
        {
            selectionMode = SelectionMode.MAIN_MENU;
            switchToBubbles();
        }
        else
        {
            StaticData.findDeepChild(transform, "RecruitDialogue").GetComponent<TextMeshProUGUI>()
                .text = selectedUnit.recruitQuote[recruitQuoteIdx];
        }
        recruitQuoteIdx++;
    }

    public void switchToBonusEXP()
    {
        Transform content = StaticData.findDeepChild(transform, "BonusContent");
        Transform prefab = StaticData.findDeepChild(transform, "UnitForBonus");

        enableChild("PickedUnitForEXP", false);

        for (int q = 0; q < content.childCount; q++)
        {
            Destroy(content.GetChild(q).gameObject);
        }
        content.DetachChildren();

        foreach (Unit unit in StaticData.members)
        {
            Transform opt = Instantiate(prefab, content);
            opt.gameObject.SetActive(true);

            StaticData.findDeepChild(opt, "BonusPortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(unit.unitName);

            StaticData.findDeepChild(opt, "BonusName").GetComponent<TextMeshProUGUI>()
                .text = unit.unitName;

            Button.ButtonClickedEvent optAction = new Button.ButtonClickedEvent();
            optAction.AddListener(delegate { selectUnitForBonus(unit); });
            opt.GetComponent<Button>().onClick = optAction;
        }
    }

    public void selectUnitForBonus(Unit u)
    {
        selectedUnit = u;
        enableChild("PickedUnitForEXP", true);

        comparativeEXP = (selectedUnit.level * 100) + selectedUnit.experience;
        givenEXP = 0;

        StaticData.findDeepChild(transform, "PickedBonusPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(selectedUnit.unitName);

        refreshBonusEXP();
    }

    private void refreshBonusEXP()
    {
        int display = comparativeEXP + givenEXP;
        StaticData.findDeepChild(transform, "EXPGiveCount").GetComponent<TextMeshProUGUI>()
            .text = "" + givenEXP;
        StaticData.findDeepChild(transform, "TotalLevel").GetComponent<TextMeshProUGUI>()
            .text = $"{display / 100}";
        StaticData.findDeepChild(transform, "TotalEXP").GetComponent<TextMeshProUGUI>()
            .text = $"{display % 100}";
        StaticData.findDeepChild(transform, "LevelBox").GetComponent<Image>()
            .color = (display % 100) > (comparativeEXP % 100) ? Color.green : Color.black;
        StaticData.findDeepChild(transform, "EXPBox").GetComponent<Image>()
            .color = new Color(0, (display % 100) / 100.0f, 0);
    }

    public void plus1EXP()
    {
        givenEXP = Mathf.Min(StaticData.bonusEXP, givenEXP + 1);
        refreshBonusEXP();
    }
    public void plus10EXP()
    {
        givenEXP = Mathf.Min(StaticData.bonusEXP, givenEXP + 10);
        refreshBonusEXP();
    }
    public void plusLevelEXP()
    {
        int display = comparativeEXP + givenEXP;
        givenEXP = Mathf.Min(StaticData.bonusEXP, givenEXP + (100 - (display % 100)));
        refreshBonusEXP();
    }
    public void minus1EXP()
    {
        givenEXP = Mathf.Max(0, givenEXP - 1);
        refreshBonusEXP();
    }
    public void minus10EXP()
    {
        givenEXP = Mathf.Max(0, givenEXP - 10);
        refreshBonusEXP();
    }
    public void minusLevelEXP()
    {
        int display = comparativeEXP + givenEXP;
        if (display % 100 == 0)
        {
            givenEXP -= Mathf.Min(100, givenEXP);
        }
        else
        {
            givenEXP -= Mathf.Min(100, display % 100);
        }
        refreshBonusEXP();
    }

    public void confirmBonusEXP()
    {
        if (givenEXP < 0)
        {
            Debug.LogError("Less than 0 EXP");
        }
        else if (givenEXP == 0)
        {
            playOneTimeSound("back");
            return;
        }
        else
        {
            StaticData.bonusEXP -= givenEXP;
            nextLevel();
        }
    }
    public void nextLevel()
    {
        if ((givenEXP + (comparativeEXP % 100)) / 100 == 0)
        {
            playOneTimeSound("exp");
            selectedUnit.addExperience(givenEXP);
            switchToBonusEXP();
        }
        else
        {
            int amountTowardsLevel = 100 - selectedUnit.experience;
            givenEXP -= amountTowardsLevel;
            bool[] levelup = selectedUnit.addExperience(amountTowardsLevel);
            playOneTimeSound("level");

            StaticData.findDeepChild(transform, "LevelUpPortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(selectedUnit.unitName);
            StaticData.findDeepChild(transform, "LevelUpClass").GetComponent<TextMeshProUGUI>()
                .text = "" + selectedUnit.unitClass.className;
            StaticData.findDeepChild(transform, "LevelUpLevel").GetComponent<TextMeshProUGUI>()
                .text = "" + selectedUnit.level;
            StaticData.findDeepChild(transform, "LevelUpMaxHP").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.maxHP}^".Replace("^", levelup[0] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpSTR").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.strength}^".Replace("^", levelup[1] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpMAG").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.magic}^".Replace("^", levelup[2] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpSKL").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.skill}^".Replace("^", levelup[3] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpSPD").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.speed}^".Replace("^", levelup[4] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpLUK").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.luck}^".Replace("^", levelup[5] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpDEF").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.defense}^".Replace("^", levelup[6] ? "^" : "");
            StaticData.findDeepChild(transform, "LevelUpRES").GetComponent<TextMeshProUGUI>()
                .text = $"^{selectedUnit.resistance}^".Replace("^", levelup[7] ? "^" : "");

            StaticData.findDeepChild(transform, "LevelUpMaxHP").GetComponent<TextMeshProUGUI>()
                .color = levelup[0] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpSTR").GetComponent<TextMeshProUGUI>()
                .color = levelup[1] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpMAG").GetComponent<TextMeshProUGUI>()
                .color = levelup[2] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpSKL").GetComponent<TextMeshProUGUI>()
                .color = levelup[3] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpSPD").GetComponent<TextMeshProUGUI>()
                .color = levelup[4] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpLUK").GetComponent<TextMeshProUGUI>()
                .color = levelup[5] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpDEF").GetComponent<TextMeshProUGUI>()
                .color = levelup[6] ? Color.cyan : Color.black;
            StaticData.findDeepChild(transform, "LevelUpRES").GetComponent<TextMeshProUGUI>()
                .color = levelup[7] ? Color.cyan : Color.black;
        }

    }

    public void changePlayerAnimations()
    {
        StaticData.playerAnimations = (StaticData.AnimationSetting)(((int)StaticData.playerAnimations + 1) % 3);
        StaticData.findDeepChild(transform, "PlayerAnimState").GetComponent<TextMeshProUGUI>()
            .text = "" + StaticData.playerAnimations;
        StaticData.findDeepChild(transform, "TogglePersonal").gameObject.SetActive(StaticData.playerAnimations == StaticData.AnimationSetting.PERSONAL);
    }
    public void changeAllyAnimations()
    {
        StaticData.allyAnimations = (StaticData.AnimationSetting)(((int)StaticData.allyAnimations + 1) % 2);
        StaticData.findDeepChild(transform, "AllyAnimState").GetComponent<TextMeshProUGUI>()
            .text = "" + StaticData.allyAnimations;
    }
    public void changeOtherAnimations()
    {
        StaticData.otherAnimations = (StaticData.AnimationSetting)(((int)StaticData.otherAnimations + 1) % 2);
        StaticData.findDeepChild(transform, "OtherAnimState").GetComponent<TextMeshProUGUI>()
            .text = "" + StaticData.otherAnimations;
    }
    public void changeMusicVolume(float vol)
    {
        StaticData.musicVolume = vol;
        if (music != null)
        {
            music.volume = StaticData.musicVolume;
        }
    }
    public void changeSFXVolume(float vol)
    {
        StaticData.sfxVolume = vol;
    }
    public void switchToPersonalAnimationOptions()
    {
        Transform list = StaticData.findDeepChild(transform, "AnimOptions");
        for (int q = 0; q < list.childCount; q++)
        {
            Destroy(list.GetChild(q).gameObject);
        }
        list.DetachChildren();

        Transform prefab = StaticData.findDeepChild(transform, "AnimUnit");
        for (int q = 0; q < StaticData.members.Count; q++)
        {
            Transform opt = Instantiate(prefab, list);
            opt.gameObject.SetActive(true);
            Unit unit = StaticData.members[q];
            StaticData.findDeepChild(opt, "AnimUnitPortrait").GetComponent<Image>()
                .sprite = AssetDictionary.getPortrait(unit.unitName);
            StaticData.findDeepChild(opt, "AnimUnitName").GetComponent<TextMeshProUGUI>()
                .text = unit.unitName;
            StaticData.findDeepChild(opt, "AnimUnitMode").GetComponent<TextMeshProUGUI>()
                .text = unit.animationOn ? "" + StaticData.AnimationSetting.CINEMATIC
                : "" + StaticData.AnimationSetting.MAP;

            Button.ButtonClickedEvent toggle = new Button.ButtonClickedEvent();
            toggle.AddListener(delegate { toggleUnitAnimations(unit, StaticData.findDeepChild(opt, "ToggleAnim")); });
            StaticData.findDeepChild(opt, "ToggleAnim").GetComponent<Button>()
                .onClick = toggle;
        }

        enableChild("OptionsMenu", false);
        enableChild("PersonalAnims", true);

        selectionMode = SelectionMode.ANIM_OPTS;
    }
    public void toggleUnitAnimations(Unit unit, Transform toggleButton)
    {
        unit.animationOn = !unit.animationOn;
        StaticData.findDeepChild(toggleButton, "AnimUnitMode").GetComponent<TextMeshProUGUI>()
            .text = unit.animationOn ? "" + StaticData.AnimationSetting.CINEMATIC
            : "" + StaticData.AnimationSetting.MAP;
    }

    public override bool completed()
    {
        return done;
    }

    public override void Z()
    {
        if (selectionMode == SelectionMode.RECRUIT)
        {
            nextRecruitQuote();
        }
    }

    public override void ENTER()
    {
        if (selectionMode == SelectionMode.SPARRING)
        {
            instantiatedSparAnimation.skip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum SelectionMode
    {
        MAIN_MENU, ROAM, SWITCH, MOVE,
        ANIM_OPTS,
        SPARRING, RECRUIT,
        STATS_PAGE, STATS_PAGE_PICK_UNITS, STATS_PAGE_ITEMS,
        ITEM_MENU_PICK_UNIT, ITEM_MENU_INVENTORY, ITEM_MENU_CONVOY, PICK_UNITS,
        STANDBY
    }

}

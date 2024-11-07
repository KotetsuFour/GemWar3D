using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class AbstractBattleAnimation : MonoBehaviour
{
    public UnitModel playerUnit;
    public UnitModel enemyUnit;
    public Weapon playerWep;
    public Weapon enemyWep;
    public Tile playerTile;
    public Tile enemyTile;
    public int enemyEXPYield;

    public bool isPlayerAttack;

    public UnitModel currentActor;
    public UnitModel target;

    public Battle battle;
    public GridMap gridmap;

    public Phase phase;
    public Battle.BattleEvent currentEvent;
    public float timer;

    public bool[] levelup;
    public abstract void constructor(Battle battle, string musicName, GridMap gridmap);

    public abstract void backToGridMap();

    public void skip()
    {
        if (phase > Phase.END)
        {
            return;
        }
        if (isPlayerAttack)
        {
            playerUnit.getUnit().currentHP = battle.getATKFinalHP();
            enemyUnit.getUnit().currentHP = battle.getDFDFinalHP();
            if (playerWep != null)
            {
                playerWep.usesLeft = battle.atkFinalWepDurability();
            }
            if (enemyWep != null)
            {
                enemyWep.usesLeft = battle.dfdFinalWepDurability();
            }
        }
        else
        {
            playerUnit.getUnit().currentHP = battle.getDFDFinalHP();
            enemyUnit.getUnit().currentHP = battle.getATKFinalHP();
            if (playerWep != null)
            {
                playerWep.usesLeft = battle.dfdFinalWepDurability();
            }
            if (enemyWep != null)
            {
                enemyWep.usesLeft = battle.atkFinalWepDurability();
            }
        }
        if (!playerUnit.getUnit().isAlive())
        {
            poof(playerUnit, true);
        }
        else if (playerUnit.getUnit().team == Unit.UnitTeam.PLAYER)
        {
            playerUnit.getUnit().addExperience(calculateEXP());
        }
        if (!enemyUnit.getUnit().isAlive())
        {
            poof(enemyUnit, true);
        }
        if (playerWep != null && playerWep.uses > 0 && playerWep.usesLeft <= 0)
        {
            playerUnit.getUnit().breakEquippedWeapon();
            playerUnit.equip();
        }
        if (enemyWep != null && enemyWep.uses > 0 && enemyWep.usesLeft <= 0)
        {
            enemyUnit.getUnit().breakEquippedWeapon();
            enemyUnit.equip();
        }
        backToGridMap();
    }
    public abstract void getNextEvent();
    public void poof(UnitModel unit, bool skipping)
    {
        gridmap.player.Remove(unit.getUnit());
        gridmap.enemy.Remove(unit.getUnit());
        gridmap.ally.Remove(unit.getUnit());
        gridmap.other.Remove(unit.getUnit());
        if (!skipping)
        {
            gridmap.playOneTimeSound(AssetDictionary.getAudio("poof"));
            ParticleAnimation poof = AssetDictionary.getParticles("poof");
            Instantiate(poof, unit.transform.position, poof.transform.rotation);
        }
        if (!unit.getUnit().isEssential
            && (unit.getUnit().team == Unit.UnitTeam.PLAYER || unit.getUnit().team == Unit.UnitTeam.ENEMY))
        {
            unit.getTile().getGemstones().Add(new Gemstone(unit.getUnit()));
            if (unit.getUnit().heldItem is Gemstone)
            {
                unit.getTile().getGemstones().Add((Gemstone)unit.getUnit().heldItem);
            }
        }
        unit.getTile().updateGemstones();
        Destroy(unit.gameObject);
    }

    public void setEXPDisplay(bool leveledUp)
    {
        StaticData.findDeepChild(transform, "EXPMeter").gameObject.SetActive(true);
        if (leveledUp)
        {
            StaticData.findDeepChild(transform, "LevelBack").GetComponent<Image>()
                .color = Color.green;
            StaticData.findDeepChild(transform, "Level").GetComponent<TextMeshProUGUI>()
                .color = Color.black;
        }
        StaticData.findDeepChild(transform, "Level").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().level;
        StaticData.findDeepChild(transform, "EXPBack").GetComponent<Image>()
            .color = new Color(0, playerUnit.getUnit().experience / 100f, 0);
        StaticData.findDeepChild(transform, "EXP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().experience;
    }
    public void levelUpFanfare()
    {
        gridmap.playOneTimeSound("level");
        StaticData.findDeepChild(transform, "EXPMeter").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "StatsAndInfo").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "LevelUpBanner").gameObject.SetActive(true);
    }
    public void levelUpOK()
    {
        if (phase != Phase.LEVELSTATS)
        {
            return;
        }
        if (playerWep != null && playerWep.uses > 0 && playerWep.usesLeft <= 0)
        {
            setBrokenDisplay();
            playerUnit.getUnit().breakEquippedWeapon();
            playerUnit.equip();
            timer = 3;
            phase = Phase.BROKENWEP;
        }
        else
        {
            backToGridMap();
        }
    }
    public void setLevelUpDisplay()
    {
        StaticData.findDeepChild(transform, "LevelUpBanner").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "LevelUp").gameObject.SetActive(true);
        StaticData.findDeepChild(transform, "LevelUpPortrait").GetComponent<Image>()
            .sprite = AssetDictionary.getPortrait(playerUnit.getUnit().unitName);
        StaticData.findDeepChild(transform, "LevelUpClass").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().unitClass.className;
        StaticData.findDeepChild(transform, "LevelUpLevel").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().level;
        StaticData.findDeepChild(transform, "LevelUpMaxHP").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().maxHP}^".Replace("^", levelup[0] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpSTR").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().strength}^".Replace("^", levelup[1] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpMAG").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().magic}^".Replace("^", levelup[2] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpSKL").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().skill}^".Replace("^", levelup[3] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpSPD").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().speed}^".Replace("^", levelup[4] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpLUK").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().luck}^".Replace("^", levelup[5] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpDEF").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().defense}^".Replace("^", levelup[6] ? "^" : "");
        StaticData.findDeepChild(transform, "LevelUpRES").GetComponent<TextMeshProUGUI>()
            .text = $"^{playerUnit.getUnit().resistance}^".Replace("^", levelup[7] ? "^" : "");

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
    public void setBrokenDisplay()
    {
        gridmap.playOneTimeSound("broken");
        StaticData.findDeepChild(transform, "EXPMeter").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "StatsAndInfo").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "LevelUpBanner").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "LevelUp").gameObject.SetActive(false);
        StaticData.findDeepChild(transform, "BrokenBanner").gameObject.SetActive(true);
        StaticData.findDeepChild(transform, "BrokenMessage").GetComponent<TextMeshProUGUI>()
            .text = $"A {playerWep.itemName} broke!";

    }

    public int calculateEXP()
    {
        if (enemyUnit == null)
        {
            return enemyEXPYield;
        }
        if (isPlayerAttack)
        {
            if (battle.dfdTookDamage())
            {
                return 10;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (battle.atkTookDamage())
            {
                return 10;
            }
            else
            {
                return 1;
            }
        }
    }

    public void updateHPs()
    {
        playerUnit.getUnit().currentHP = Mathf.Max(0, isPlayerAttack ? currentEvent.atkFinalHP : currentEvent.dfdFinalHP);
        enemyUnit.getUnit().currentHP = Mathf.Max(0, isPlayerAttack ? currentEvent.dfdFinalHP : currentEvent.atkFinalHP);
        StaticData.findDeepChild(transform, "PlayerHP").GetComponent<TextMeshProUGUI>()
            .text = "" + playerUnit.getUnit().currentHP;
        StaticData.findDeepChild(transform, "EnemyHP").GetComponent<TextMeshProUGUI>()
            .text = "" + enemyUnit.getUnit().currentHP;
    }


    public enum Phase
    {
        BEGIN, ACTIVATE_BATTLE_SKILL, MOVE, ACTIVATE_CRITICAL, ATTACKANIM, RANGEANIM,
        PROJECTILE, HITANIM, ACTIVATE_AFTER_SKILL, AFTERHP, POOF, END, EXP, EXPCHANGE,
        LEVELUP, LEVELSTATS, BROKENWEP
    }
}

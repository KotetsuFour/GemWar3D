using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	public string unitName;
	public UnitClass unitClass;
	public string description;
	public int maxHP;
	public int currentHP;
	public int strength;
	public int magic;
	public int skill;
	public int speed;
	public int luck;
	public int defense;
	public int resistance;
	public int constitution;
	public int movement;

	public int hpGrowth;
	public int strengthGrowth;
	public int magicGrowth;
	public int skillGrowth;
	public int speedGrowth;
	public int luckGrowth;
	public int defenseGrowth;
	public int resistanceGrowth;
	public int level;
	public int experience;

	public Item personalItem;
	public Weapon heldWeapon;
	public Item heldItem;

	public Weapon.WeaponType weaponType;
	public int proficiency;

	public bool isEssential;
	public bool isLeader;
	public int equipped; //0 = personal, 1 = held, 2 = none
	public bool isExhausted;

	public string[] talkConvo;
	public string[] battleQuote;
	public string[] recruitQuote;
	public bool talkRestricted;
	public Item talkReward;
	public bool dropsHeldItem;
	public int recruitability;

	public List<Color> palette;

	public int supportId1;
	public int supportId2;
	public FusionSkill fusionSkill1;
	public FusionSkill fusionSkill2;
	public FusionSkill fusionSkillBonus;
	public Affinity affinity;

	public int timesUsedMapSkill1;
	public int timesUsedMapSkill2;
	public int timesUsedMapSkillBonus;

	public AIType ai1;
	public AIType ai2;
	public UnitTeam team;

	public int battles;
	public int wins;
	public int losses;

	public string talkIconName;

	public bool deployed;

	public UnitModel model;

	public bool animationOn;

	public static int MAX_PALETTE_COLORS = 7;

	public void constructor(string unitName, UnitClass unitClass, string description, int maxHP, int strength, int magic,
		int skill, int speed, int luck, int defense, int resistance, int constitution, int movement,
		int hpGrowth, int strengthGrowth, int magicGrowth, int skillGrowth, int speedGrowth, int luckGrowth,
		int defenseGrowth, int resistanceGrowth, Item personalItem, Weapon.WeaponType weaponType, int weaponProf,
		UnitTeam team, int supportId1, int supportId2, Affinity aff, float[] colors)
	{
		this.unitName = unitName;
		this.description = description;
		this.unitClass = unitClass;
		this.maxHP = Mathf.Max(maxHP, unitClass.minMaxHP);
		this.currentHP = this.maxHP;
		this.strength = Mathf.Max(strength, unitClass.minStrength);
		this.magic = Mathf.Max(magic, unitClass.minMagic);
		this.skill = Mathf.Max(skill, unitClass.minSkill);
		this.speed = Mathf.Max(speed, unitClass.minSpeed);
		this.luck = Mathf.Max(luck, unitClass.minLuck);
		this.defense = Mathf.Max(defense, unitClass.minDefense);
		this.resistance = Mathf.Max(resistance, unitClass.minResistance);
		this.constitution = Mathf.Max(constitution, unitClass.minConstitution);
		this.movement = Mathf.Max(movement, unitClass.minMovement);
		this.hpGrowth = hpGrowth;
		this.strengthGrowth = strengthGrowth;
		this.magicGrowth = magicGrowth;
		this.skillGrowth = skillGrowth;
		this.speedGrowth = speedGrowth;
		this.luckGrowth = luckGrowth;
		this.defenseGrowth = defenseGrowth;
		this.resistanceGrowth = resistanceGrowth;
		this.personalItem = personalItem;
		this.weaponType = weaponType;
		this.proficiency = weaponProf;
		this.team = team;
		this.supportId1 = supportId1;
		this.supportId2 = supportId2;
		this.affinity = aff;
		level = 1;

		palette = new List<Color>();
		for (int q = 0; q < Mathf.Min(colors.Length, MAX_PALETTE_COLORS * 3); q += 3)
        {
			palette.Add(new Color(colors[q], colors[q + 1], colors[q + 2]));
        }
	}
	public int getBaseAccuracy()
	{
		return (skill * 2) + luck;
	}
	public int getBaseAvoidance()
	{
		return (speed * 2) + luck;
	}
	public int getBaseCrit()
	{
		return skill;
	}
	public int getAttackPower()
	{
		Weapon w = getEquippedWeapon();
		if (w != null)
		{
			if (w.magic)
			{
				return magic + w.might;
			}
			else
			{
				return strength + w.might;
			}
		}
		return strength;
	}
	public int getAccuracy()
	{
		Weapon w = getEquippedWeapon();
		if (w == null)
		{
			return getBaseAccuracy();
		}
		return getBaseAccuracy() + w.hit;
	}
	public int getCrit()
	{
		Weapon w = getEquippedWeapon();
		if (w == null)
		{
			return getBaseCrit();
		}
		return getBaseCrit() + w.crit;
	}
	public int getAvoidance()
	{
		return getBaseAvoidance();
	}
	public bool[] addExperience(int exp)
	{
		experience += exp;
		if (experience >= 100)
		{
			experience -= 100;
			return levelUp();
		}
		return null;
	}
	private bool[] levelUp()
	{
		bool[] ret = new bool[8];
		level++;
		if (Random.Range(0, 99) < hpGrowth)
		{
			maxHP++;
			ret[0] = true;
		}
		if (Random.Range(0, 99) < strengthGrowth)
		{
			strength++;
			ret[1] = true;
		}
		if (Random.Range(0, 99) < magicGrowth)
		{
			magic++;
			ret[2] = true;
		}
		if (Random.Range(0, 99) < skillGrowth)
		{
			skill++;
			ret[3] = true;
		}
		if (Random.Range(0, 99) < speedGrowth)
		{
			speed++;
			ret[4] = true;
		}
		if (Random.Range(0, 99) < luckGrowth)
		{
			luck++;
			ret[5] = true;
		}
		if (Random.Range(0, 99) < defenseGrowth)
		{
			defense++;
			ret[6] = true;
		}
		if (Random.Range(0, 99) < resistanceGrowth)
		{
			resistance++;
			ret[7] = true;
		}
		return ret;
	}
	public int rawEXPReward()
	{
		return unitClass.rawEXPReward;
	}
	public bool isAlive()
	{
		return currentHP > 0;
	}
	public bool takeDamage(int damage, bool crit)
	{
		if (crit)
		{
			damage *= 3;
		}
		currentHP = Mathf.Max(0, currentHP - damage);
		return isAlive();
	}
	public void heal(int amount)
	{
		currentHP = Mathf.Min(currentHP + amount, maxHP);
	}
	public bool isFlying()
	{
		for (int q = 0; q < unitClass.unitTypes.Length; q++)
		{
			if (unitClass.unitTypes[q] == UnitClass.UnitType.FLYING)
			{
				return true;
			}
		}
		return false;
	}
	public Weapon getEquippedWeapon()
	{
		if (equipped == 0)
		{
			if (personalItem is Weapon)
			{
				return (Weapon)personalItem;
			}
		}
		else if (equipped == 1)
		{
			if (heldWeapon != null)
			{
				if (weaponType == heldWeapon.weaponType
						&& proficiency >= heldWeapon.proficiency)
				{
					return heldWeapon;
				}
			}
		}
		return null;
	}
	public string getEquippedWeaponName()
	{
		Weapon w = getEquippedWeapon();
		if (w == null)
		{
			return "NONE";
		}
		return w.itemName;
	}
	public void equipForDistance(int distance)
	{
		if (equipped == 0)
		{
			if (personalItem is Weapon)
			{
				Weapon s = (Weapon)personalItem;
				if (s.minRange <= distance && s.maxRange >= distance)
				{
					return;
				}
			}
			if (heldItem is Weapon)
			{
				Weapon h = (Weapon)heldItem;
				if (h.weaponType == weaponType && proficiency >= h.proficiency &&
						h.minRange <= distance && h.maxRange >= distance)
				{
					equipped = 1;
					return;
				}
			}
		}
		else if (equipped == 1)
		{
			if (heldItem is Weapon)
			{
				Weapon h = (Weapon)heldItem;
				if (h.weaponType == weaponType && proficiency >= h.proficiency &&
						h.minRange <= distance && h.maxRange >= distance)
				{
					return;
				}
			}
			if (personalItem is Weapon)
			{
				Weapon s = (Weapon)personalItem;
				if (s.minRange <= distance && s.maxRange >= distance)
				{
					equipped = 0;
					return;
				}
			}
		}
		else
		{
			if (distance == 1)
			{
				return;
			}
			if (heldItem is Weapon)
			{
				Weapon h = (Weapon)heldItem;
				if (h.weaponType == weaponType && proficiency >= h.proficiency &&
						h.minRange <= distance && h.maxRange >= distance)
				{
					equipped = 1;
					return;
				}
			}
			if (personalItem is Weapon)
			{
				Weapon s = (Weapon)personalItem;
				if (s.minRange <= distance && s.maxRange >= distance)
				{
					equipped = 0;
					return;
				}
			}
		}
	}
	public void equipSpecial()
	{
		equipped = 0;
	}
	public void equipHeld()
	{
		equipped = 1;
	}
	public void equipNone()
	{
		equipped = 2;
	}
	public void equip(int wep)
	{
		equipped = wep;
	}
	public void breakEquippedWeapon()
	{
		if (equipped == 0)
		{
			personalItem = null;
			if (heldWeapon != null)
			{
				equip(1);
			}
		}
		else if (equipped == 1)
		{
			heldWeapon = null;
			equip(0);
		}
	}
	public void setTalkConvo(string[] de, bool restriction, Item reward)
	{
		this.talkConvo = de;
		this.talkRestricted = restriction;
		this.talkReward = reward;
	}

	/*
	public void setTalkIcon(string iconName)
	{
		talkIconName = iconName;
		if (iconName == null || iconName == "")
		{
			transform.GetChild(1).gameObject.SetActive(false);
		}
		else
		{
			transform.GetChild(1).gameObject.SetActive(true);
			transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = ImageDictionary.getImage(iconName);
		}
	}
	*/

	public void setBattleQuote(string[] de)
	{
		this.battleQuote = de;
	}
	public void setDeathQuote(string[] de)
	{
		this.recruitQuote = de;
	}

	public bool isFusion()
	{
		return fusionSkillBonus != FusionSkill.LOCKED;
	}

	public bool hasSkill(FusionSkill skillToCheck)
    {
		return fusionSkillBonus == skillToCheck || fusionSkill1 == skillToCheck || fusionSkill2 == skillToCheck;
    }

	public enum AIType
	{
		IDLE, GUARD, ATTACK, PURSUE, BURN
	}

	public enum UnitTeam
	{
		PLAYER, ENEMY, ALLY, OTHER
	}

	public enum Affinity
	{
		NONE, FIRE, WIND, WATER, EARTH, LIGHTNING, ICE, ANIMA, LIGHT, DARK, HEAVEN
	}

	public enum FusionSkill
	{
		LOCKED, FUTURE_VISION, PLANTS, HOLOGRAMS, SPINDASH, FIRE, FREEZE, VANTAGE, SOL, LUNA, ASTRA,
		HEALING, ABSORPTION, 
	}

}

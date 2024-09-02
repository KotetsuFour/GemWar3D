using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
	public int proficiency;
	public int might;
	public int hit;
	public int crit;
	public int weight;
	public int minRange;
	public int maxRange;
	public bool magic;
	public bool brave;
	public WeaponType weaponType;
	public UnitClass.UnitType[] effectiveTypes;

	public Weapon(string name, int proficiency, int might, int hit, int crit, int weight, int minRange, int maxRange, int uses,
		bool magic, UnitClass.UnitType[] effectiveTypes, int id) : base(name, uses, id)
	{
		this.itemName = name;
		this.proficiency = proficiency;
		this.might = might;
		this.hit = hit;
		this.crit = crit;
		this.weight = weight;
		this.minRange = minRange;
		this.maxRange = maxRange;
		this.magic = magic;
		this.effectiveTypes = effectiveTypes;
	}

	public bool isEffectiveAgainst(Unit u)
    {
		if (effectiveTypes == null)
        {
			return false;
        }
		for (int q = 0; q < u.unitClass.unitTypes.Length; q++)
        {
			for (int w = 0; w < effectiveTypes.Length; w++)
            {
				if (u.unitClass.unitTypes[q] == effectiveTypes[w])
                {
					return true;
                }
            }
        }
		return false;
    }

	public void loseDurability(int loss)
    {
		if (uses > 0)
        {
			usesLeft -= loss;
        }
    }
	public abstract bool isAdvantageousAgainst(Weapon w);

	public enum WeaponType
    {
		SWORD, LANCE, AXE, FIST, ARMOR, WHIP, BOW, CLUB, SPECIAL
    }

	public static string weaponTypeName(WeaponType type)
    {
		if (type == WeaponType.SWORD)
        {
			return "Sword";
        } else if (type == WeaponType.LANCE)
        {
			return "Lance";
		}
		else if (type == WeaponType.AXE)
		{
			return "Axe";
		}
		else if (type == WeaponType.FIST)
		{
			return "Fist";
		}
		else if (type == WeaponType.ARMOR)
		{
			return "Armor";
		}
		else if (type == WeaponType.WHIP)
		{
			return "Whip";
		}
		else if (type == WeaponType.BOW)
		{
			return "Bow";
		}
		else if (type == WeaponType.CLUB)
		{
			return "Club";
		}
		else if (type == WeaponType.SPECIAL)
		{
			return "Special";
		}
		return null;
	}

}

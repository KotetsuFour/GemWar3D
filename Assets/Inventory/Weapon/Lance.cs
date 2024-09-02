using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : LanceAndArmor
{
	public Lance(string name, int proficiency, int might, int hit, int crit, int weight, int minRange, int maxRange, int uses, bool magic, UnitClass.UnitType[] effective, int id)
		: base(name, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effective, id)
	{
		weaponType = WeaponType.LANCE;
	}
	public override Item clone()
	{
		Lance ret = new Lance(itemName, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effectiveTypes, id);
		return ret;
	}
	public override string description()
	{
		return "Lance:" + (proficiency > -1 ? proficiency : "--") + " MT:" + might + " HIT:" + hit + " CRIT:" + crit
			+ " WT:" + weight + " RNG:" + minRange + "~" + maxRange + "\nUSE:" + (uses > -1 ? (usesLeft + "/" + uses) : "--/--");
	}

}

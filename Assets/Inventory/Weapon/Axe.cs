using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : AxeAndWhip
{
	public Axe(string name, int proficiency, int might, int hit, int crit, int weight, int minRange, int maxRange, int uses, bool magic, UnitClass.UnitType[] effective, int id)
		: base(name, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effective, id)
	{
		weaponType = WeaponType.AXE;
	}

	public override Item clone()
	{
		Axe ret = new Axe(itemName, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effectiveTypes, id);
		return ret;
	}
	public override string description()
	{
		return "Axe:" + (proficiency > -1 ? proficiency : "--") + " MT:" + might + " HIT:" + hit + " CRIT:" + crit
			+ " WT:" + weight + " RNG:" + minRange + "~" + maxRange + "\nUSE:" + (uses > -1 ? (usesLeft + "/" + uses) : "--/--");
	}

}

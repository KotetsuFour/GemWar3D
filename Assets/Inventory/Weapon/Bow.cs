using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    public Bow(string name, int proficiency, int might, int hit, int crit, int weight, int minRange, int maxRange, int uses, bool magic, UnitClass.UnitType[] effective, int id)
        : base(name, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effective, id)
    {
        weaponType = WeaponType.BOW;
        effectiveTypes = new UnitClass.UnitType[] {UnitClass.UnitType.FLYING};
    }

    public override bool isAdvantageousAgainst(Weapon w)
    {
        return false;
    }

    public override Item clone()
    {
        Bow ret = new Bow(itemName, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effectiveTypes, id);
        return ret;
    }
    public override string description()
    {
        return "Bow:" + (proficiency > -1 ? proficiency : "--") + " MT:" + might + " HIT:" + hit + " CRIT:" + crit
            + " WT:" + weight + " RNG:" + minRange + "~" + maxRange + "\nUSE:" + (uses > -1 ? (usesLeft + "/" + uses) : "--/--");
    }

}

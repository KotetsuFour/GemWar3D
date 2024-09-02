using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwordAndFist : Weapon
{
    public SwordAndFist(string name, int proficiency, int might, int hit, int crit, int weight, int minRange, int maxRange, int uses, bool magic, UnitClass.UnitType[] effective, int id)
        : base(name, proficiency, might, hit, crit, weight, minRange, maxRange, uses, magic, effective, id)
    {
    }
    public override bool isAdvantageousAgainst(Weapon w)
    {
        return w is AxeAndWhip;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gemstone : Item
{
    public Unit unit;
    //TODO stuff
    public Gemstone(Unit unit) : base("Gemstone", 0, 0)
    {
        this.unit = unit;
    }
    public override Item clone()
    {
        return new Gemstone(unit);
    }
    public override string description()
    {
        return unit.unitName + "'s Gemstone";
    }
}

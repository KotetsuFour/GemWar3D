using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeObjective : Objective
{
	public override bool checkComplete(GridMap chpt)
    {
		foreach (Unit u in chpt.player)
        {
            if (u.isEssential)
            {
                return false;
            }
        }
        return true;
    }

	public override string getName(GridMap map)
    {
        return starredUnits(map) + "must escape to Warp Pad";
    }
}

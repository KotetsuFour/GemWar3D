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

	public override string getName()
    {
        //TODO make it more clear that it's only the essential units that need to escape
        return "Starred units escape to Warp Pad";
    }
}

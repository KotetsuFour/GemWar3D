using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutObjective : Objective
{
	public override bool checkComplete(GridMap chpt)
    {
		return chpt.enemy.Count == 0;
    }

	public override string getName()
    {
        return "Defeat all enemies";
    }
}

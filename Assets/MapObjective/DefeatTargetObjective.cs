using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatTargetObjective : Objective
{
	private Unit target;
	public DefeatTargetObjective(Unit target)
    {
		this.target = target;
    }
	public override bool checkComplete(GridMap chpt)
    {
		return !chpt.enemy.Contains(target);
    }

	public override string getName()
    {
		return "Defeat " + target.unitName;
    }

}

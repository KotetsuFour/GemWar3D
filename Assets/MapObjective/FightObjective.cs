using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightObjective : Objective
{
	private int unitsToDefeat;

	public FightObjective (int unitsToDefeat)
    {
		this.unitsToDefeat = unitsToDefeat;
    }
	public override bool checkComplete(GridMap chpt)
    {
		//TODO if the number of needed units have been defeated, return true;
		return false;
    }

	public override string getName(GridMap map)
    {
		//TODO change to Defeat left/total enemies
		return "Defeat " + unitsToDefeat + " enemies";
    }
}

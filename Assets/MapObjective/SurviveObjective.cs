using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveObjective : Objective
{
	private int turnsToSurvive;
	public SurviveObjective(int turnsToSurvive)
    {
		this.turnsToSurvive = turnsToSurvive;
    }
	public override bool checkComplete(GridMap chpt)
    {
		return chpt.turn > turnsToSurvive;
    }
	public override string getName()
    {
		return "Survive for " + turnsToSurvive + " turns";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendObjective : Objective
{
    private int turnsToDefendFor;
    public DefendObjective(int turnsToDefendFor)
    {
        this.turnsToDefendFor = turnsToDefendFor;
    }
	public override bool checkComplete(GridMap chpt)
    {
        return chpt.turn > turnsToDefendFor;
    }

	public new bool checkFailed(GridMap chpt)
    {
        return base.checkFailed(chpt) || chpt.seized;
    }

    public override string getName()
    {
        return "Defend for " + turnsToDefendFor + " turns";
    }

    public new string getFailure()
    {
        return "Rose Quartz is poofed or defend point is captured";
    }
}

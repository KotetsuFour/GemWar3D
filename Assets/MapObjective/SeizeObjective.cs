using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeizeObjective : Objective
{
	public override bool checkComplete(GridMap chpt)
    {
		return chpt.seized;
    }

	public override string getName()
    {
		return "Seize the seize point";
    }
}

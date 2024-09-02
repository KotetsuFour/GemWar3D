using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective
{
	public abstract bool checkComplete(GridMap chpt);

	public bool checkFailed(GridMap chpt)
    {
        foreach (Unit u in CampaignData.members)
        {
            if (u.isLeader && !u.isAlive())
            {
                return true;
            }
        }
        return false;
    }

    public abstract string getName();

    public string getFailure()
    {
        return "Rose Quartz is poofed";
    }

}

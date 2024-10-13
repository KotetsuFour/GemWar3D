using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective
{
	public abstract bool checkComplete(GridMap chpt);

	public bool checkFailed(GridMap chpt)
    {
        foreach (Unit u in StaticData.members)
        {
            if (u.isLeader && !u.isAlive())
            {
                return true;
            }
        }
        return false;
    }

    public abstract string getName(GridMap map);

    public string getFailure()
    {
        return "Rose Quartz is poofed";
    }

    public string starredUnits(GridMap map)
    {
        if (map == null)
        {
            return "Starred units";
        }
        List<Unit> starredUnits = new List<Unit>();
        foreach (Unit u in map.player)
        {
            if (u.isEssential)
            {
                starredUnits.Add(u);
            }
        }
        if (starredUnits.Count == 1)
        {
            return "Rose Quartz ";
        }
        if (starredUnits.Count == 2)
        {
            return $"{starredUnits[0].unitName} and {starredUnits[1].unitName} ";
        }
        string ret = $"{starredUnits[0]}, ";
        for (int q = 1; q < starredUnits.Count; q++)
        {
            if (q == starredUnits.Count - 1)
            {
                ret += $"and {starredUnits[q]} ";
            }
            else
            {
                ret += $"{starredUnits[q]}, ";
            }
        }
        return ret;
    }

}

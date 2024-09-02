using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public static int iron;
    public static int steel;
    public static int silver;
    public static int bonusEXP;

    public static List<Unit> members = new List<Unit>();
    public static List<Gemstone> prisoners = new List<Gemstone>();

    public static string[][] supportLog;
    public static int[] supportLevels;
    public static int[][] supportRequirements;

    public static int scene = 1;

    public static Unit unitToInstantiate;

    public static List<int>[] convoyIds = new List<int>[10];
    public static List<int>[] convoyDurabilities = new List<int>[10];

    public static int savefile;

    public static int chapterPrep;
    public static int[] positions;

    public static Unit findUnit(string name)
    {
        foreach (Unit u in members)
        {
            if (u.unitName.Equals(name))
            {
                return u;
            }
        }
        return null;
    }


    public static void reset()
    {
        iron = 0;
        steel = 0;
        silver = 0;

        members = new List<Unit>();
        prisoners = new List<Gemstone>();

        supportLog = null;
        supportLevels = null;

        scene = 1;

        convoyIds = new List<int>[10];
        convoyDurabilities = new List<int>[10];

        savefile = 0;

        chapterPrep = 0;
        positions = null;
    }

    public static Transform findDeepChild(Transform parent, string childName)
    {
        LinkedList<Transform> kids = new LinkedList<Transform>();
        for (int q = 0; q < parent.childCount; q++)
        {
            kids.AddLast(parent.GetChild(q));
        }
        while (kids.Count > 0)
        {
            Transform current = kids.First.Value;
            kids.RemoveFirst();
            if (current.name == childName || current.name + "(Clone)" == childName)
            {
                return current;
            }
            for (int q = 0; q < current.childCount; q++)
            {
                kids.AddLast(current.GetChild(q));
            }
        }
        return null;
    }
}

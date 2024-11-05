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

    public static List<int>[] convoyIds = new List<int>[10];
    public static List<int>[] convoyDurabilities = new List<int>[10];

    public static int savefile;

    public static int chapterPrep;
    public static int[] positions;

    public static bool copyrightMusic;

    public static AnimationSetting playerAnimations;
    public static AnimationSetting allyAnimations;
    public static AnimationSetting otherAnimations;
    public static float musicVolume = 1;
    public static float sfxVolume = 1;
    
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
    public static List<int>[] getConvoyIds()
    {
        if (convoyIds[0] == null)
        {
            for (int q = 0; q < convoyIds.Length; q++)
            {
                convoyIds[q] = new List<int>();
                convoyDurabilities[q] = new List<int>();
            }
        }
        return convoyIds;
    }
    public static List<int>[] getConvoyDurabilities()
    {
        if (convoyDurabilities[0] == null)
        {
            for (int q = 0; q < convoyIds.Length; q++)
            {
                convoyIds[q] = new List<int>();
                convoyDurabilities[q] = new List<int>();
            }
        }
        return convoyDurabilities;
    }
    public static void addToConvoy(Item item)
    {
        if (item is Weapon)
        {
            convoyIds[(int)((Weapon)item).weaponType].Add(item.id);
            convoyDurabilities[(int)((Weapon)item).weaponType].Add(item.usesLeft);
        }
        else if (item is UsableItem)
        {
            convoyIds[9].Add(item.id);
            convoyDurabilities[9].Add(item.usesLeft);
        }
    }
    public static Item takeFromConvoy(int type, int idx)
    {
        Item ret = Item.itemIndex[getConvoyIds()[type][idx]].clone();
        ret.usesLeft = getConvoyDurabilities()[type][idx];

        getConvoyIds()[type].RemoveAt(idx);
        getConvoyDurabilities()[type].RemoveAt(idx);

        return ret;
    }

    public static void registerSupportUponEscape(Unit escapee, List<Unit> player, int turn)
    {
        for (int q = 0; q < player.Count; q++)
        {
            if (escapee.supportId1 > -1
                && player[q] != escapee
                && (player[q].supportId1 == escapee.supportId1 || player[q].supportId2 == escapee.supportId1))
            {
                SupportLog.supportLog[escapee.supportId1].supportAmount += turn;
            }
            if (escapee.supportId2 > -1
                && player[q] != escapee
                && (player[q].supportId1 == escapee.supportId2 || player[q].supportId2 == escapee.supportId2))
            {
                SupportLog.supportLog[escapee.supportId2].supportAmount += turn;
            }
        }
    }

    public static void registerRemainingSupports(List<Unit> player, int turn)
    {
        for (int q = 0; q < player.Count; q++)
        {
            for (int w = q + 1; w < player.Count; w++)
            {
                Unit first = player[q];
                Unit second = player[w];
                if (first.supportId1 > -1
                    && (second.supportId1 == first.supportId1 || second.supportId2 == first.supportId1))
                {
                    SupportLog.supportLog[first.supportId1].supportAmount += turn;
                }
                if (first.supportId2 > -1
                    && (second.supportId1 == first.supportId2 || second.supportId2 == first.supportId2))
                {
                    SupportLog.supportLog[first.supportId2].supportAmount += turn;
                }
            }
        }
    }
    public static void dealWithGemstones()
    {
        foreach (Unit u in members)
        {
            if (u.heldItem is Gemstone)
            {
                Gemstone gem = (Gemstone)u.heldItem;
                gem.unit.currentHP = gem.unit.maxHP;
                gem.unit.heldWeapon = null;
                gem.unit.heldItem = null;
                if (gem.unit.team == Unit.UnitTeam.ENEMY)
                {
                    prisoners.Add(gem);
                }
                u.heldItem = null;
            }
        }
    }
    public static void refreshUnits()
    {
        foreach (Unit u in members)
        {
            if (u.isAlive() || u.isEssential)
            {
                Debug.Log(u.unitName + " " + u.currentHP);
                u.currentHP = u.maxHP;
                u.isExhausted = false;
            }
        }
    }

    public static Unit[] findLivingSupportPartners(Unit seeker)
    {
        int[] supportIdx = { seeker.supportId1, seeker.supportId2 };
        Unit[] ret = new Unit[supportIdx.Length];
        for (int q = 0; q < supportIdx.Length; q++)
        {
            if (supportIdx[q] == -1)
            {
                continue;
            }
            for (int w = 0; w < members.Count; w++)
            {
                int[] compare = { members[w].supportId1, members[w].supportId2 };
                if (members[w] != seeker && compare[q] == supportIdx[q])
                {
                    ret[q] = members[w];
                    break;
                }
            }
        }

        return ret;
    }
    public static Unit getUnitByName(string name)
    {
        foreach (Unit unit in members)
        {
            if (unit.unitName == name)
            {
                return unit;
            }
        }
        return null;
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
    public static Material getMaterialByName(Material[] materials, string matName)
    {
        foreach (Material m in materials)
        {
            if (m.name.Replace(" ", "").Replace("1", "").Replace("(Instance)", "")
                == matName.Replace(" ", "").Replace("1", "").Replace("(Instance)", ""))
            {
                return m;
            }
        }
        Debug.Log(matName);
        return null;
    }

    public enum AnimationSetting
    {
        CINEMATIC, MAP, PERSONAL
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOption : MonoBehaviour
{
    private int idx;
    public void setIdx(int idx)
    {
        this.idx = idx;
    }

    public void select(GridMap map)
    {
        map.selectMenuOption(idx);
    }
}

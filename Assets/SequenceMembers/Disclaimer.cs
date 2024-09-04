using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Disclaimer : SequenceMember
{

    public float timer;
    public void constructor()
    {
        StaticData.findDeepChild(transform, "Content").GetComponent<TextMeshProUGUI>().text
            = "This is a fanmade project, not in affiliation\nwith Intelligent Systems or Cartoon Network." +
            "\nThis project is not for profit, so please\ndon't sue me!";
        timer = 4;

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public override bool completed()
    {
        if (timer <= 0)
        {
            return true;
        }
        return false;
    }
    public new void LEFT_MOUSE()
    {
        Z();
    }
    public new void Z()
    {
        timer = 0;
    }
    public new void ENTER()
    {
        Z();
    }
}

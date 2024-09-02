using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChapterTitle : SequenceMember
{
    public float timer;
    public void constructor(Camera cam, string title)
    {
        StaticData.findDeepChild(transform, "Background").GetComponent<Image>().sprite
            = AssetDictionary.getImage("crystal_gem_star.png");
        StaticData.findDeepChild(transform, "Title").GetComponent<TextMeshProUGUI>().text
            = title;
        timer = 3;
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
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = null;
            return true;
        }
        return false;
    }

    public new void LEFT_MOUSE(float mouseX, float mouseY)
    {
        Z();
    }
    public new void Z()
    {
        timer = 0;
    }
    public new void ENTER()
    {
        timer = 0;
    }
}

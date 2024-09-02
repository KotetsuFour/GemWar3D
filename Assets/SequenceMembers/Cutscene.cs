using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cutscene : SequenceMember
{
    private string[] dialogue;
    private bool storyComplete;
    private int idx;
    private float timer;

    public void constructor(string[] dialogue, Camera cam)
    {
        this.dialogue = dialogue;
        nextSaying();
    }
    private bool nextSaying()
    {
        if (idx == dialogue.Length)
        {
            return false;
        }
        while (idx < dialogue.Length &&
            (dialogue[idx][0] == '@' || (dialogue[idx][0] == '$' && timer <= 0)))
        {
            processLine(dialogue[idx]);
            idx++;
        }
        if (idx < dialogue.Length && timer <= 0)
        {
            StaticData.findDeepChild(transform, "DialogueBox").gameObject.SetActive(true);
            StaticData.findDeepChild(transform, "Dialogue").GetComponent<TextMeshProUGUI>().text
                = dialogue[idx];
        }
        return true;
    }

    private void processLine(string line)
    {
        //TODO
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void finish()
    {
        storyComplete = true;
    }

    public override bool completed()
    {
        return storyComplete;
    }
    public new void LEFT_MOUSE(float mouseX, float mouseY)
    {
          Z();
    }
    public new void Z()
    {
        if (!nextSaying())
        {
            finish();
        }
    }
    public new void ENTER()
    {
        finish();
    }
    public new void ESCAPE()
    {
        //TODO maybe something
    }

}

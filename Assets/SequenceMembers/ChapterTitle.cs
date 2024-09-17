using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChapterTitle : SequenceMember
{
    public static float timeToFadeInBackground = 2;
    public static float timeToFadeInTitle = 2;
    public static float timeToStayOnTitle = 3;

    private SelectionMode selectionMode;

    private float timer;

    private bool done;

    private List<AudioSource> audioSources;

    public void constructor(string title)
    {
        StaticData.findDeepChild(transform, "Title").GetComponent<TextMeshProUGUI>().text = title;
        selectionMode = SelectionMode.BACK;

        audioSources = new List<AudioSource>();
        AudioSource ambient = getAudioSource(AssetDictionary.getAudio("chapter_title_tone"));
        audioSources.Add(ambient);
        ambient.Play();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (selectionMode == SelectionMode.BACK)
        {
            StaticData.findDeepChild(transform, "Blackscreen").GetComponent<CanvasGroup>().alpha
                = (timeToFadeInBackground - timer) / timeToFadeInBackground;
            if (timer >= timeToFadeInBackground)
            {
                timer = 0;
                selectionMode = SelectionMode.TITLE;

                AudioSource jingle = getAudioSource(AssetDictionary.getAudio("chapter_title_jingle"));
                audioSources.Add(jingle);
                jingle.Play();
            }
        }
        else if (selectionMode == SelectionMode.TITLE)
        {
            StaticData.findDeepChild(transform, "TitleBack").GetComponent<CanvasGroup>().alpha
                = timer / timeToFadeInTitle;
            if (timer >= timeToFadeInTitle)
            {
                timer = 0;
                selectionMode = SelectionMode.LOOK;
            }
        }
        else if (selectionMode == SelectionMode.LOOK)
        {
            if (timer >= timeToStayOnTitle)
            {
                finish();
            }
        }
    }

    private void finish()
    {
        done = true;
        foreach (AudioSource source in audioSources)
        {
            Destroy(source);
        }
    }

    public override bool completed()
    {
        return done;
    }

    public override void LEFT_MOUSE()
    {
        Z();
    }
    public override void Z()
    {
        finish();
    }
    public override void ENTER()
    {
        Z();
    }

    private enum SelectionMode
    {
        STANDBY, BACK, TITLE, LOOK
    }
}

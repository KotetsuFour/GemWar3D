using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveScreen : SequenceMember
{
    private int menuIdx;
    private bool done;
    [SerializeField] private List<Button> saveButtons;
    public void Start()
    {
        menuIdx = 0;
    }

    public void selectSaveFile(int file)
    {
        //We'll treat "Continue without saving" like saving to a fourth file
        SaveMechanism.saveGame(file + 1);

        gameObject.SetActive(false);
        done = true;
    }
    public override bool completed()
    {
        return done;
    }
    public override void Z()
    {
        selectSaveFile(menuIdx);
    }
    public override void UP()
    {
        StaticData.findDeepChild(saveButtons[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
            .color = Color.white;
        menuIdx--;
        if (menuIdx < 0)
        {
            menuIdx = saveButtons.Count - 1;
        }
        StaticData.findDeepChild(saveButtons[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
            .color = Color.cyan;
    }
    public override void DOWN()
    {
        StaticData.findDeepChild(saveButtons[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
            .color = Color.white;
        menuIdx = (menuIdx + 1) % saveButtons.Count;
        StaticData.findDeepChild(saveButtons[menuIdx].transform, "Text").GetComponent<TextMeshProUGUI>()
            .color = Color.cyan;
    }
}

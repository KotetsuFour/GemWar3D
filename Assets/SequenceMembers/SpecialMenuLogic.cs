using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class SpecialMenuLogic
{
    public static void restartChapter(Unit unit)
    {
        SaveMechanism.loadGame(CampaignData.savefile, unit);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void mainMenu(Unit unit)
    {
        SaveMechanism.loadGame(CampaignData.savefile, unit);
        SceneManager.LoadScene("Main Menu");
    }
}

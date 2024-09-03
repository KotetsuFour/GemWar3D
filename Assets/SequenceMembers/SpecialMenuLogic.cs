using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class SpecialMenuLogic
{
    public static void restartChapter()
    {
        SaveMechanism.loadGame(StaticData.savefile);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void mainMenu()
    {
        SaveMechanism.loadGame(StaticData.savefile);
        SceneManager.LoadScene("Main Menu");
    }
}

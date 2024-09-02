using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : SequenceMember
{
    public GameObject titleScreen;
    public GameObject mainPage;
    public GameObject filesPage;
    public GameObject controlsPage;
    public GameObject extrasPage;
    public GameObject deleteCheck;
    public GameObject progressNote;

    public GameObject instructions;

    public Unit unit;

    private GameObject currentMenu;
    private MenuPage currentPage;

    private int menuIdx;
    private int file;
    private int nextTODOChapter = 4;

    public void activate(Camera cam)
    {
        cam.orthographicSize = 5;
        currentMenu = titleScreen;
        currentPage = MenuPage.TITLE;
        titleScreen.SetActive(true);
    }
    public override bool completed()
    {
        return false; //We exit by switching scenes
    }
    public new void LEFT_MOUSE(float mouseX, float mouseY)
    {
        //TODO
    }
    public new void RIGHT_MOUSE(float mouseX, float mouseY)
    {
        //TODO
    }

    public new void Z()
    {
        if (currentPage == MenuPage.TITLE)
        {
            currentMenu.SetActive(false);
            toMainPage();
            instructions.SetActive(true);
        }
        else if (currentPage == MenuPage.MAIN)
        {
            if (menuIdx == 0)
            {
                currentMenu.SetActive(false);
                currentMenu = filesPage;
                currentMenu.SetActive(true);
                resetIdx();
                currentPage = MenuPage.FILE;
            }
            else if (menuIdx == 1)
            {
                currentMenu.SetActive(false);
                currentMenu = controlsPage;
                currentMenu.SetActive(true);
                currentPage = MenuPage.CONTROLS;
            }
            else if (menuIdx == 2)
            {
                currentMenu.SetActive(false);
                currentMenu = extrasPage;
                currentMenu.SetActive(true);
                resetIdx();
                currentPage = MenuPage.EXTRAS;
            }
            else if (menuIdx == 3)
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
            }
        }
        else if (currentPage == MenuPage.FILE)
        {
            file = (menuIdx / 2) + 1;
            if (menuIdx % 2 == 0)
            {
                if (!SaveMechanism.findFile(file))
                {
                    SaveMechanism.saveGame(file);
                }
                SaveMechanism.loadGame(file, unit);
                if (StaticData.scene < 0)
                {
                    //TODO go to base
                }
                else
                {
                    if (StaticData.scene == nextTODOChapter)
                    {
                        currentMenu.SetActive(false);
                        currentMenu = progressNote;
                        currentMenu.SetActive(true);
                        currentPage = MenuPage.PROGRESS_NOTE;
                    }
                    else
                    {
                        SceneManager.LoadScene("Chapter" + StaticData.scene);
                    }
                }
            }
            else if (SaveMechanism.findFile(file))
            {
                currentMenu.SetActive(false);
                currentMenu = deleteCheck;
                currentMenu.SetActive(true);
                resetIdx();
                currentPage = MenuPage.DELETE;
            }
        }
        else if (currentPage == MenuPage.EXTRAS)
        {
            //TODO add extras functionality
        }
        else if (currentPage == MenuPage.DELETE)
        {
            if (menuIdx == 0)
            {
                currentMenu.SetActive(false);
                currentMenu = filesPage;
                currentMenu.SetActive(true);
                resetIdx();
                currentPage = MenuPage.FILE;
            }
            else if (menuIdx == 1)
            {
                SaveMechanism.deleteFile(file);
                currentMenu.SetActive(false);
                currentMenu = filesPage;
                currentMenu.SetActive(true);
                resetIdx();
                currentPage = MenuPage.FILE;
            }
        }
    }
    public new void X()
    {
        if (currentPage == MenuPage.FILE || currentPage == MenuPage.CONTROLS || currentPage == MenuPage.EXTRAS)
        {
            currentMenu.SetActive(false);
            toMainPage();
        }
        else if (currentPage == MenuPage.MAIN)
        {
            currentMenu.SetActive(false);
            currentMenu = titleScreen;
            currentMenu.SetActive(true);
            instructions.SetActive(false);
            currentPage = MenuPage.TITLE;
        }
        else if (currentPage == MenuPage.DELETE)
        {
            currentMenu.SetActive(false);
            currentMenu = filesPage;
            currentMenu.SetActive(true);
            resetIdx();
            currentPage = MenuPage.FILE;
        } else if (currentPage == MenuPage.PROGRESS_NOTE)
        {
            currentMenu.SetActive(false);
            currentMenu = filesPage;
            currentMenu.SetActive(true);
            resetIdx();
            currentPage = MenuPage.FILE;
        }
    }
    public new void UP()
    {
        if (currentPage == MenuPage.MAIN || currentPage == MenuPage.FILE || currentPage == MenuPage.EXTRAS
            || currentPage == MenuPage.DELETE)
        {
            currentMenu.transform.GetChild(0).GetChild(menuIdx).GetComponent<TextMeshProUGUI>().color = Color.white;
            menuIdx--;
            if (menuIdx < 0)
            {
                menuIdx = currentMenu.transform.GetChild(0).childCount - 1;
            }
            currentMenu.transform.GetChild(0).GetChild(menuIdx).GetComponent<TextMeshProUGUI>().color = Color.cyan;
        }
    }
    public new void DOWN()
    {
        if (currentPage == MenuPage.MAIN || currentPage == MenuPage.FILE || currentPage == MenuPage.EXTRAS
            || currentPage == MenuPage.DELETE)
        {
            currentMenu.transform.GetChild(0).GetChild(menuIdx).GetComponent<TextMeshProUGUI>().color = Color.white;
            menuIdx = (menuIdx + 1) % currentMenu.transform.GetChild(0).childCount;
            currentMenu.transform.GetChild(0).GetChild(menuIdx).GetComponent<TextMeshProUGUI>().color = Color.cyan;
        }
    }
    public new void ENTER()
    {
        if (currentPage == MenuPage.TITLE)
        {
            currentMenu.SetActive(false);
            toMainPage();
            instructions.SetActive(true);
        }
    }

    private void toMainPage()
    {
        mainPage.SetActive(true);
        currentMenu = mainPage;
        resetIdx();
        currentPage = MenuPage.MAIN;
    }

    private void resetIdx()
    {
        menuIdx = 0;
        for (int q = 0; q < currentMenu.transform.GetChild(0).childCount; q++)
        {
            currentMenu.transform.GetChild(0).GetChild(q).GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        currentMenu.transform.GetChild(0).GetChild(menuIdx).GetComponent<TextMeshProUGUI>().color = Color.cyan;
    }

    public enum MenuPage
    {
        TITLE, MAIN, FILE, CONTROLS, EXTRAS, DELETE, PROGRESS_NOTE
    }
}

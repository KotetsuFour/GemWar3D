using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : SequenceMember
{
    private string currentPage;

    private int selectedFile;
    private AudioSource music;
    void Start()
    {
        music = getAudioSource(AssetDictionary.getAudio("main-theme"));
        music.loop = true;
        music.Play();

        loadFiles();
        currentPage = "TitleScreen";

    }
    private void loadFiles()
    {
        if (SaveMechanism.findFile(1))
        {
            SaveMechanism.loadGame(1);
            StaticData.findDeepChild(transform, "File1Progress").GetComponent<TextMeshProUGUI>()
                .text = $"Chapter {Mathf.Abs(StaticData.scene)}" + (StaticData.scene < 0 ? " - Crystal Base"
                : " - Map");
        }
        else
        {
            StaticData.findDeepChild(transform, "File1Progress").GetComponent<TextMeshProUGUI>()
                .text = "Empty";
        }
        if (SaveMechanism.findFile(2))
        {
            SaveMechanism.loadGame(2);
            StaticData.findDeepChild(transform, "File2Progress").GetComponent<TextMeshProUGUI>()
                .text = $"Chapter {Mathf.Abs(StaticData.scene)}" + (StaticData.scene < 0 ? " - Crystal Base"
                : " - Map");
        }
        else
        {
            StaticData.findDeepChild(transform, "File2Progress").GetComponent<TextMeshProUGUI>()
                .text = "Empty";
        }
        if (SaveMechanism.findFile(3))
        {
            SaveMechanism.loadGame(3);
            StaticData.findDeepChild(transform, "File3Progress").GetComponent<TextMeshProUGUI>()
                .text = $"Chapter {Mathf.Abs(StaticData.scene)}" + (StaticData.scene < 0 ? " - Crystal Base"
                : " - Map");
        }
        else
        {
            StaticData.findDeepChild(transform, "File3Progress").GetComponent<TextMeshProUGUI>()
                .text = "Empty";
        }
        CampaignSaveData.wipeMemory();
    }
    public override bool completed()
    {
        return false; //We exit by switching scenes
    }
    public void switchToPage(string page)
    {
        StaticData.findDeepChild(transform, currentPage).gameObject.SetActive(false);
        currentPage = page;
        StaticData.findDeepChild(transform, currentPage).gameObject.SetActive(true);
    }
    public void quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void selectFile(int file)
    {
        if (!SaveMechanism.findFile(file))
        {
            CampaignSaveData.wipeMemory();
            SaveMechanism.saveGame(file);
        }
        SaveMechanism.loadGame(file);
        if (Mathf.Abs(StaticData.scene) == Chapter.nextTODOChapter)
        {
            switchToPage("ProgressNote");
        }
        else
        {
            Chapter.goToChapter(StaticData.scene);
        }
    }
    public void deleteFile(int file)
    {
        if (!SaveMechanism.findFile(file))
        {
            playOneTimeSound("back");
            return;
        }
        selectedFile = file;
        StaticData.findDeepChild(transform, "DeleteMessage").GetComponent<TextMeshProUGUI>()
            .text = $"DELETE File {selectedFile}?";
        playOneTimeSound("select");
        switchToPage("DeleteCheck");
    }
    public void confirmDeleteFile()
    {
        SaveMechanism.deleteFile(selectedFile);
        loadFiles();
        switchToPage("FilesPage");
    }
    public override void LEFT_MOUSE()
    {
        if (currentPage == "TitleScreen")
        {
            //TODO end animation
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapEventExecutor : MonoBehaviour
{
    private string[] script;
    private GridMap gridmap;
    private Unit speaker;
    private Unit listener;

    private int idx;
    private float timer = float.MinValue;
    private Stack<string> ifStack;
    private bool skipping;

    private AudioSource music;

    public void constructor(string[] script, Unit speaker, Unit listener, GridMap gridmap)
    {
        this.script = script;
        this.speaker = speaker;
        this.listener = listener;
        this.gridmap = gridmap;
        ifStack = new Stack<string>();

        nextAction();
    }
    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        } else if (timer != float.MinValue)
        {
            timer = float.MinValue;
            Z();
        }
    }

    private void nextAction()
    {
        if (idx == script.Length)
        {
            backToGridMap();
        }
        while (idx < script.Length &&
            (script[idx][0] == '@' || (script[idx][0] == '$' && timer <= 0)))
        {
            processLine(script[idx]);
            idx++;
        }
        if (idx < script.Length && timer <= 0)
        {
            StaticData.findDeepChild(gridmap.transform, "DialogueBox").gameObject.SetActive(true);
            string[] speakerAndText = script[idx].Split(' ');
            StaticData.findDeepChild(gridmap.transform, "SpeakerName").GetComponent<TextMeshProUGUI>()
                .text = speakerAndText[0].Replace('_', ' ');
            if (speakerAndText[1] == "null")
            {
                StaticData.findDeepChild(gridmap.transform, "Portrait").GetComponent<Image>().color
                    = new Color(0, 0, 0, 0);
                StaticData.findDeepChild(gridmap.transform, "Portrait").GetComponent<Image>().sprite
                    = null;
            }
            else
            {
                StaticData.findDeepChild(gridmap.transform, "Portrait").GetComponent<Image>().color
                    = Color.white;
                StaticData.findDeepChild(gridmap.transform, "Portrait").GetComponent<Image>().sprite
                    = AssetDictionary.getPortrait(speakerAndText[1]);
            }
            StaticData.findDeepChild(gridmap.transform, "Dialogue").GetComponent<TextMeshProUGUI>().text
                = script[idx].Substring(speakerAndText[0].Length + speakerAndText[1].Length + 2);
            idx++;
            gridmap.playOneTimeSound("next-dialogue");
        }
    }
    private void backToGridMap()
    {
        StaticData.findDeepChild(gridmap.transform, "DialogueBox").gameObject.SetActive(false);
        StaticData.findDeepChild(gridmap.transform, "LeftSpeaker").gameObject.SetActive(false);
        StaticData.findDeepChild(gridmap.transform, "RightSpeaker").gameObject.SetActive(false);
        if (music != null)
        {
            Destroy(music.gameObject);
        }
        gridmap.endMapEvent();
    }

    public void Z()
    {
        nextAction();
    }
    public void skip()
    {
        skipping = true;
        for (int q = idx; q < script.Length; q++)
        {
            if (script[q][0] == '$' || script[q][0] == '@')
            {
                processLine(script[q]);
            }
        }
        backToGridMap();
    }

    private void processLine(string line)
    {
        string[] parts = line.Split(' ');
        string comm = parts[0].Substring(1);
        if (comm == "if")
        {
            if (ifStack.Count == 0 || ifStack.Peek() == "dontElse" || ifStack.Peek() == "doingElse")
            {
                bool worked = false;

                if (parts[1] == "alive")
                {
                    string gemName = parts[2].Replace('_', ' ');
                    foreach (Unit unit in StaticData.members)
                    {
                        if (unit.unitName == gemName)
                        {
                            worked = true;
                            break;
                        }
                    }
                }
                if (worked)
                {
                    ifStack.Push("dontElse");
                }
                else
                {
                    ifStack.Push("doElse");
                }
            }
            else
            {
                ifStack.Push("skip");
            }
        }
        else if (comm == "else")
        {
            string top = ifStack.Pop();
            if (top == "doElse")
            {
                ifStack.Push("doingElse");
            }
            else if (top == "dontElse")
            {
                ifStack.Push("skipElse");
            }
            else if (top == "skip")
            {
                ifStack.Push("skip");
            }
        }
        else if (comm == "endif")
        {
            ifStack.Pop();
        }

        if (ifStack.Count == 0 || ifStack.Peek() == "dontElse" || ifStack.Peek() == "doingElse")
        {
            Debug.Log("allowed action " + comm);
            if (comm == "pause" && !skipping)
            {
                float time = float.Parse(parts[1]);
                timer = time;
            }
            else if (comm == "sound" && !skipping)
            {
                string soundName = parts[1];
                if (parts[2] == "stopMusic")
                {
                    music.Pause();
                }
                else if (parts[2] == "playMusic")
                {
                    if (music != null)
                    {
                        Destroy(music.gameObject);
                    }
                    music = gridmap.getAudioSource(AssetDictionary.getAudio(soundName));
                    music.loop = true;
                    music.Play();
                }
                else if (parts[2] == "play")
                {
                    gridmap.playOneTimeSound(soundName);
                }
            }
            /*
            else if (comm == "moveCharacter")
            {
                int modelIdx = int.Parse(parts[1]);
                CutsceneModel model = models[modelIdx];

                Transform pos = positions[int.Parse(parts[2])];

                float speed = float.Parse(parts[3]);

                model.getAgent().speed = speed;
                model.getAgent().angularSpeed = 360 * 4;
                model.getAgent().destination = pos.position;
            }
            else if (comm == "rotateCharacter")
            {
                int modelIdx = int.Parse(parts[1]);
                CutsceneModel model = models[modelIdx];

                Transform lookAt = positions[int.Parse(parts[2])];
                Quaternion rotation = Quaternion.LookRotation(lookAt.position - model.transform.position);

                model.transform.rotation = rotation;
            }
            else if (comm == "teleportCharacter")
            {
                int modelIdx = int.Parse(parts[1]);
                CutsceneModel model = models[modelIdx];

                Transform pos = positions[int.Parse(parts[2])];
                Transform lookAt = positions[int.Parse(parts[3])];
                Quaternion rotation = Quaternion.LookRotation(lookAt.position - pos.position);

                model.transform.SetPositionAndRotation(pos.position, rotation);
            }
            else if (comm == "image")
            {
                string imageName = parts[1];
                Sprite image = AssetDictionary.getImage(imageName);
                Image display = StaticData.findDeepChild(transform, "ImageDisplay").GetComponent<Image>();
                display.sprite = image;
                display.gameObject.SetActive(true);
            }
            else if (comm == "removeImage")
            {
                StaticData.findDeepChild(transform, "ImageDisplay").gameObject.SetActive(false);
            }
            */
            else if (comm == "silence")
            {
                StaticData.findDeepChild(gridmap.transform, "DialogueBox").gameObject.SetActive(false);
                StaticData.findDeepChild(transform, "LeftSpeaker").gameObject.SetActive(false);
                StaticData.findDeepChild(transform, "RightSpeaker").gameObject.SetActive(false);
            }
            else if (comm == "removeConvo" && !skipping)
            {
                speaker.talkConvo = null;
            }
            else if (comm == "give")
            {
                string receiverName = parts[1].Replace('_', ' ');
                Unit receiver = StaticData.findUnit(receiverName);
                if (speaker.talkReward is Weapon && receiver.heldWeapon == null)
                {
                    receiver.heldWeapon = (Weapon)speaker.talkReward.clone();
                    Debug.Log("Weapon " + receiver.heldWeapon.might + "," + receiver.heldWeapon.hit);
                }
                else if (!(speaker.talkReward is Weapon) && receiver.heldItem == null)
                {
                    receiver.heldItem = speaker.talkReward.clone();
                    Debug.Log("Item");
                }
                else
                {
                    StaticData.addToConvoy(speaker.talkReward.clone());
                }
            }
            else if (comm == "giveall")
            {
                foreach (Unit u in gridmap.player)
                {
                    if (speaker.talkReward is Weapon && u.heldWeapon == null)
                    {
                        u.heldWeapon = (Weapon)speaker.talkReward.clone();
                        Debug.Log("Weapon " + u.heldWeapon.might + "," + u.heldWeapon.hit);
                    }
                    else if (!(speaker.talkReward is Weapon) && u.heldItem == null)
                    {
                        u.heldItem = speaker.talkReward.clone();
                        Debug.Log("Item");
                    }
                    else
                    {
                        StaticData.addToConvoy(speaker.talkReward.clone());
                    }
                }
            }
            else if (comm == "leave")
            {
                Tile talkerTile = speaker.model.getTile();
                gridmap.player.Remove(speaker);
                gridmap.enemy.Remove(speaker);
                gridmap.ally.Remove(speaker);
                gridmap.other.Remove(speaker);
                Destroy(speaker.model.gameObject);
                talkerTile.setOccupant(null);
            }
            else if (comm == "join")
            {
                gridmap.enemy.Remove(speaker);
                gridmap.ally.Remove(speaker);
                gridmap.other.Remove(speaker);
                gridmap.player.Add(speaker);
                speaker.team = Unit.UnitTeam.PLAYER;
                speaker.model.setCircleColor();
                StaticData.members.Add(speaker);
            }
            else if (comm == "right")
            {
                if (parts[1] == "null")
                {
                    StaticData.findDeepChild(gridmap.transform, "RightSpeaker").gameObject.SetActive(false);
                }
                else
                {
                    StaticData.findDeepChild(gridmap.transform, "RightSpeaker").gameObject.SetActive(true);
                    if (parts.Length > 2)
                    {
                        StaticData.findDeepChild(gridmap.transform, "RightSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1], parts[2]);
                    }
                    else
                    {
                        StaticData.findDeepChild(gridmap.transform, "RightSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1]);
                    }
                }
            }
            else if (comm == "left")
            {
                if (parts[1] == "null")
                {
                    StaticData.findDeepChild(gridmap.transform, "LeftSpeaker").gameObject.SetActive(false);
                }
                else
                {
                    StaticData.findDeepChild(gridmap.transform, "LeftSpeaker").gameObject.SetActive(true);
                    if (parts.Length > 2)
                    {
                        StaticData.findDeepChild(gridmap.transform, "LeftSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1], parts[2]);
                    }
                    else
                    {
                        StaticData.findDeepChild(gridmap.transform, "LeftSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1]);
                    }
                }
            }
        }
    }
}

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
    private float timer = float.MinValue;

    private Stack<string> ifStack;
    private List<string> stringStorage;

    private Dictionary<string, AudioSource> audioPlaying;

    [SerializeField] private CutsceneModel[] models;
    [SerializeField] private Transform[] positions;

    private Transform cameraDestination;
    private Transform cameraLookAtPoint;
    private float cameraSpeed;

    public static float ACCEPTABLE_DISTANCE_FROM_DESTINATION = 0.1f;

    public void constructor(string[] dialogue)
    {
        this.dialogue = dialogue;
        ifStack = new Stack<string>();
        stringStorage = new List<string>();
        audioPlaying = new Dictionary<string, AudioSource>();
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
            string[] speakerAndText = dialogue[idx].Split(' ');
            StaticData.findDeepChild(transform, "SpeakerName").GetComponent<TextMeshProUGUI>()
                .text = speakerAndText[0].Replace('_', ' ');
            if (speakerAndText[1] == "null")
            {
                StaticData.findDeepChild(transform, "Portrait").GetComponent<Image>().color
                    = new Color(0, 0, 0, 0);
                StaticData.findDeepChild(transform, "Portrait").GetComponent<Image>().sprite
                    = null;
            }
            else
            {
                StaticData.findDeepChild(transform, "Portrait").GetComponent<Image>().color
                    = Color.white;
                StaticData.findDeepChild(transform, "Portrait").GetComponent<Image>().sprite
                    = AssetDictionary.getPortrait(speakerAndText[1]);
            }
            StaticData.findDeepChild(transform, "Dialogue").GetComponent<TextMeshProUGUI>().text
                = dialogue[idx].Substring(speakerAndText[0].Length + speakerAndText[1].Length + 2);
            playOneTimeSound("next-dialogue");
            idx++;
        }
        return true;
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
                else if (parts[1] == "hasString")
                {
                    worked = stringStorage.Contains(parts[2]);
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
            if (comm == "putString")
            {
                stringStorage.Add(parts[1]);
            }
            else if (comm == "removeString")
            {
                stringStorage.Remove(parts[1]);
            }
            else if (comm == "pause")
            {
                float time = float.Parse(parts[1]);
                timer = time;
            }
            else if (comm == "sound")
            {
                string soundName = parts[1];
                if (parts[2] == "stop")
                {
                    audioPlaying[soundName].Stop();
                }
                else if (parts[2] == "start")
                {
                    if (audioPlaying.ContainsKey(soundName))
                    {
                        audioPlaying[soundName].Play();
                    }
                    else
                    {
                        AudioSource source = getAudioSource(AssetDictionary.getAudio(soundName));
                        audioPlaying.Add(soundName, source);
                        audioPlaying[soundName].Play();
                    }
                    if (parts.Length > 3 && parts[3] == "wait")
                    {
                        timer = audioPlaying[soundName].clip.length;
                    }
                }
                else if (parts[2] == "startFresh")
                {
                    if (audioPlaying.ContainsKey(soundName))
                    {
                        Destroy(audioPlaying[soundName]);
                    }
                    AudioSource source = getAudioSource(AssetDictionary.getAudio(soundName));
                    audioPlaying.Add(soundName, source);
                    audioPlaying[soundName].Play();
                }
                else if (parts[2] == "loop")
                {
                    audioPlaying[soundName].loop = true;
                }
            }
            else if (comm == "animate")
            {
                int modelIdx = int.Parse(parts[1]);
                CutsceneModel model = models[modelIdx];
                Animator anim = model.getAnimator();

                string animationName = parts[2].Replace('_', ' ');
                anim.Play(animationName);
            }
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
            else if (comm == "moveCam")
            {
                cameraDestination = positions[int.Parse(parts[1])];
                cameraSpeed = float.Parse(parts[2]);
                if (parts.Length > 3 && parts[3] == "wait")
                {
                    timer = (cameraDestination.position - getCamera().transform.position).magnitude / cameraSpeed;
                }
            }
            else if (comm == "trainCam")
            {
                if (parts[1] == "none")
                {
                    cameraLookAtPoint = null;
                }
                else
                {
                    cameraLookAtPoint = positions[int.Parse(parts[1])];
                }
            }
            else if (comm == "equip")
            {
                int modelIdx = int.Parse(parts[1]);
                if (parts[2] == "none")
                {
                    models[modelIdx].equip(null);
                }
                else
                {
                    models[modelIdx].equip(parts[2].Replace('_', ' '));
                }
            }
            else if (comm == "teleportCam")
            {
                getCamera().transform.position = positions[int.Parse(parts[1])].position;
            }
            else if (comm == "image")
            {
                string imageName = parts[1];
                Sprite image = imageName == "null" ? null : AssetDictionary.getImage(imageName);
                Image display = StaticData.findDeepChild(transform, "DisplayImage").GetComponent<Image>();
                display.sprite = image;
                display.color = image == null ? Color.clear : Color.white;
                display.gameObject.SetActive(true);
            }
            else if (comm == "solidColor")
            {
                Color solid = new Color(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                Image display = StaticData.findDeepChild(transform, "DisplayImage").GetComponent<Image>();
                display.sprite = null;
                display.color = solid;
                display.gameObject.SetActive(true);
            }
            else if (comm == "removeImage")
            {
                StaticData.findDeepChild(transform, "ImageDisplay").gameObject.SetActive(false);
            }
            else if (comm == "silence")
            {
                StaticData.findDeepChild(transform, "DialogueBox").gameObject.SetActive(false);
                StaticData.findDeepChild(transform, "LeftSpeaker").gameObject.SetActive(false);
                StaticData.findDeepChild(transform, "RightSpeaker").gameObject.SetActive(false);
            }
            else if (comm == "right")
            {
                if (parts[1] == "null")
                {
                    StaticData.findDeepChild(transform, "RightSpeaker").gameObject.SetActive(false);
                }
                else
                {
                    StaticData.findDeepChild(transform, "RightSpeaker").gameObject.SetActive(true);
                    if (parts.Length > 2)
                    {
                        StaticData.findDeepChild(transform, "RightSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1], parts[2]);
                    }
                    else
                    {
                        StaticData.findDeepChild(transform, "RightSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1]);
                    }
                }
            }
            else if (comm == "left")
            {
                if (parts[1] == "null")
                {
                    StaticData.findDeepChild(transform, "LeftSpeaker").gameObject.SetActive(false);
                }
                else
                {
                    StaticData.findDeepChild(transform, "LeftSpeaker").gameObject.SetActive(true);
                    if (parts.Length > 2)
                    {
                        StaticData.findDeepChild(transform, "LeftSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1], parts[2]);
                    }
                    else
                    {
                        StaticData.findDeepChild(transform, "LeftSpeaker").GetComponent<Image>()
                            .sprite = AssetDictionary.getPortrait(parts[1]);
                    }
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer != float.MinValue)
        {
            timer = float.MinValue;
            Z();
        }
        if (cameraDestination != null && (cameraDestination.position - getCamera().transform.position).magnitude > ACCEPTABLE_DISTANCE_FROM_DESTINATION)
        {
            Vector3 direction = (cameraDestination.position - getCamera().transform.position).normalized;
            getCamera().transform.position += direction * Time.deltaTime;
        }
        if (cameraLookAtPoint != null)
        {
            getCamera().transform.rotation = Quaternion.LookRotation(cameraLookAtPoint.position - getCamera().transform.position);
        }
    }

    private void finish()
    {
        foreach (AudioSource source in audioPlaying.Values)
        {
            Destroy(source.gameObject);
        }
        foreach (CutsceneModel model in models)
        {
            Destroy(model.gameObject);
        }
        storyComplete = true;
    }

    public override bool completed()
    {
        return storyComplete;
    }
    public override void LEFT_MOUSE()
    {
          Z();
    }
    public override void Z()
    {
        if (!nextSaying())
        {
            finish();
        }
    }
    public override void ENTER()
    {
        finish();
    }
    public override void ESCAPE()
    {
        //TODO maybe something
    }

}

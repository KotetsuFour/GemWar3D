using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoDialogue : MonoBehaviour
{
    [SerializeField] private bool repeatable;
    private bool visited;
    private string[] dialogue;
    public bool canVisit()
    {
        return dialogue != null && (!visited || repeatable);
    }
    public string[] visit()
    {
        visited = true;
        return dialogue;
    }
    public void setDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
    }
}

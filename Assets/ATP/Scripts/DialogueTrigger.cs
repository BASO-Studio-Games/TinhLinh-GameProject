using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    public LocalizedString line;
    public GameObject guidePointPrefab;
    public bool isGuidePoint;
    public Transform targetTransform;
    public GameObject targetPrefab;
    public GameObject targetAction;
}


[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "Player")
    //    {
    //        TriggerDialogue();
    //    }
    //}

    public void ButtonStart()
    {
        TriggerDialogue();
    }

    private void Start()
    {
        TriggerDialogue();
    }
}
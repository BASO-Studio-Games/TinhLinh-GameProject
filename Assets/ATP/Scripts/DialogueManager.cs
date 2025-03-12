using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    private GameObject currentGuidePoint;
    private GameObject currentTargetPrefab;
    private DialogueLine currentLine;
    private List<GameObject> spawnedPrefabs = new List<GameObject>(); 

    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;
    public Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (animator != null)
        {
            animator.SetBool("isShow", true);
        }

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (currentGuidePoint != null)
        {
            Destroy(currentGuidePoint);
            currentGuidePoint = null;
        }

        foreach (var prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                DisableAllScripts(prefab);
            }
        }

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        if (currentLine.isGuidePoint)
        {
            if (dialoguePanel != null)
            {
                StartCoroutine(HideDialoguePanel());
            }

            if (currentLine.guidePointPrefab != null)
            {
                currentGuidePoint = Instantiate(currentLine.guidePointPrefab, currentLine.targetTransform.position, Quaternion.identity);
                GuidePoint guideScript = currentGuidePoint.GetComponent<GuidePoint>();
                if (guideScript != null)
                {
                    guideScript.OnGuidePointClicked += OnGuidePointClicked;
                }
            }

            if (currentLine.targetPrefab != null)
            {
                GameObject spawnedPrefab = Instantiate(currentLine.targetPrefab, currentLine.targetTransform.position, Quaternion.identity);
                spawnedPrefabs.Add(spawnedPrefab); 
                currentTargetPrefab = spawnedPrefab; 

                UpdatePrefabStates();
            }
            return;
        }
        ShowDialogueText();
    }

    private void ShowDialogueText()
    {
        dialoguePanel.SetActive(true);
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    private void OnGuidePointClicked()
    {
        if (currentGuidePoint != null)
        {
            Destroy(currentGuidePoint);
            currentGuidePoint = null;
        }
        dialoguePanel.SetActive(true);
        animator.SetBool("isShow", true);
        DisplayNextDialogueLine();
    }

    public void OnPrefabClicked(GameObject clickedPrefab)
    {
        if (clickedPrefab == currentTargetPrefab)
        {
            DisableAllScripts(currentTargetPrefab);
            currentTargetPrefab = null;
            dialoguePanel.SetActive(true);
            animator.SetBool("isShow", true);
            DisplayNextDialogueLine();
        }
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        if (dialoguePanel != null)
        {
            StartCoroutine(HideDialoguePanel());
        }

        if (currentGuidePoint != null)
        {
            Destroy(currentGuidePoint);
            currentGuidePoint = null;
        }

        currentTargetPrefab = null;

        foreach (var prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                DisableAllScripts(prefab);
            }
        }
    }

    IEnumerator HideDialoguePanel()
    {
        if (animator != null)
        {
            animator.SetBool("isShow", false);
        }
        yield return new WaitForSeconds(0.75f);
        dialoguePanel.SetActive(false);
    }

    private void DisableAllScripts(GameObject obj)
    {
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false; 
        }
    }


    private void UpdatePrefabStates()
    {
        foreach (var prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                if (prefab == currentTargetPrefab)
                {
                    EnableAllScripts(prefab);
                }
                else
                {
                    DisableAllScripts(prefab);
                }
            }
        }
    }


    private void EnableAllScripts(GameObject obj)
    {
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }

    public bool IsCurrentTarget(GameObject prefab)
    {
        return prefab == currentTargetPrefab;
    }

}

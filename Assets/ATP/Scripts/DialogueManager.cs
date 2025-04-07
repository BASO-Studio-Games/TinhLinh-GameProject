using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

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

    [SerializeField]
    private bool isPointPath = false;
    public bool IsPointPath => isPointPath;


    private void Update()
    {
        if (isPointPath)
        {
            SetIsCollided();
            isPointPath = false;
        }
    }

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

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        if (currentLine.targetAction != null)
        {
            // currentLine.targetAction.SetActive(true);
            RandomPathGenerator randomPathGenerator = currentLine.targetAction.GetComponent<RandomPathGenerator>();
            if (randomPathGenerator != null)
            {
                randomPathGenerator.StartGame();
            }

            EndDialogue();
            return;
        }

        isPointPath = currentLine.isGuidePoint;

        if (isPointPath)
        {
            ResetAllPrefabButtonStates();

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
                spawnedPrefabs.Add(currentLine.targetPrefab);
                currentTargetPrefab = currentLine.targetPrefab;

                UpdateAllPrefabButtonStates();
            }

            return;
        }

        isPointPath = false;
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
            ClickablePrefab clickableScript = currentTargetPrefab.GetComponent<ClickablePrefab>();
            if (clickableScript != null)
            {
                clickableScript.isCollided = true;
            }
            currentTargetPrefab = null;
            dialoguePanel.SetActive(true);
            animator.SetBool("isShow", true);
            DisplayNextDialogueLine();

            UpdateAllPrefabButtonStates();
        }
    }


    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.GetLocalizedString())
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



    public bool IsCurrentTarget(GameObject prefab)
    {
        return prefab == currentTargetPrefab;
    }
    public void SetIsCollided()
    {
        if (isPointPath && currentTargetPrefab != null)
        {
            ClickablePrefab clickableScript = currentTargetPrefab.GetComponent<ClickablePrefab>();
            if (clickableScript != null)
            {
                clickableScript.isCollided = true;
            }
        }
        else
        {
            Debug.Log("erro");
        }
    }

    private void UpdateAllPrefabButtonStates()
    {
        ClickablePrefab[] allPrefabs = FindObjectsOfType<ClickablePrefab>();

        foreach (ClickablePrefab clickableScript in allPrefabs)
        {
            GameObject prefab = clickableScript.gameObject;

            if (prefab != currentTargetPrefab && !clickableScript.IsCollided)
            {
                clickableScript.UpdateButtonState();
            }
        }
    }

    private void ResetAllPrefabButtonStates()
    {
        ClickablePrefab[] allPrefabs = FindObjectsOfType<ClickablePrefab>();

        foreach (ClickablePrefab clickableScript in allPrefabs)
        {
            GameObject prefab = clickableScript.gameObject;

            if (prefab != currentTargetPrefab && !clickableScript.IsCollided)
            {
                clickableScript.ResetCollisionState();
            }
        }
    }


}
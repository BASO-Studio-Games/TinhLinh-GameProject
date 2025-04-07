using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TutorialClickManager : MonoBehaviour
{
    public GameObject tutorialClickPanel;
    public RectTransform pointClick;
    public GameObject[] targetClick;
    public LocalizedString[] tutorialString; // Sử dụng LocalizedString
    public Vector3[] tutorialPointSize;
    public TMP_Text textGuide;
    public RectTransform textHighlight;

    private int currentStep = 0;
    private int currentStringStep = 0;
    private int currentSizeStep = 0;

    void Start()
    {
        ShowStep();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentStep >= targetClick.Length) return;

            currentStep++;
            ShowStep();
        }
    }

    void ShowStep()
    {
        if (currentStep >= targetClick.Length)
        {
            EndTutorial();
            return;
        }

        GameObject currentTarget = targetClick[currentStep];

        if (textGuide != null && tutorialString.Length > currentStringStep)
        {
            StartCoroutine(UpdateLocalizedText());
        }

        if (currentTarget == null)
        {
            tutorialClickPanel.gameObject.SetActive(false);
        }
        else
        {
            tutorialClickPanel.gameObject.SetActive(true);
            pointClick.position = currentTarget.transform.position;

            if (tutorialPointSize == null || tutorialPointSize.Length == 0)
            {
                tutorialPointSize = new Vector3[targetClick.Length];
                for (int i = 0; i < tutorialPointSize.Length; i++)
                {
                    tutorialPointSize[i] = new Vector3(1, 1, 1);
                }
            }

            pointClick.localScale = tutorialPointSize[currentSizeStep];
            currentSizeStep++;
        }

        currentStringStep++;
    }

    IEnumerator UpdateLocalizedText()
    {
        var handle = tutorialString[currentStringStep].GetLocalizedStringAsync();
        yield return handle;
        textGuide.text = handle.Result;
        Invoke(nameof(UpdateTextHighlight), 0.05f);
    }

    void UpdateTextHighlight()
    {
        if (textGuide == null || textHighlight == null) return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(textGuide.rectTransform);

        textHighlight.gameObject.SetActive(true);
        textHighlight.position = textGuide.transform.position;

        float paddingX = 10f;
        float paddingY = 15f;
        textHighlight.sizeDelta = new Vector2(textGuide.preferredWidth + paddingX, textGuide.preferredHeight + paddingY);
    }

    void EndTutorial()
    {
        pointClick.gameObject.SetActive(false);
        textGuide.text = "Hướng dẫn hoàn tất!";
        gameObject.SetActive(false);
        textHighlight.gameObject.SetActive(false);
    }
}

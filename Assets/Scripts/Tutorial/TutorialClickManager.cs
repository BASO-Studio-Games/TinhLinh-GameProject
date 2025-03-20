using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialClickManager : MonoBehaviour
{
    public GameObject tutorialClickPanel; // Lỗ tròn hướng dẫn
    public RectTransform pointClick; // Lỗ tròn hướng dẫn
    public GameObject[] targetClick; // Danh sách các điểm cần click
    public string[] tutorialString;
    public Vector3[] tutorialPointSize;
    public TMP_Text textGuide; // Hướng dẫn hiển thị (có thể null nếu không cần)
    public RectTransform textHighlight; // Lỗ vuông cho text hướng dẫn

    private int currentStep = 0;
    private int currentStringStep = 0;
    private int currentSizeStep = 0;

    void Start()
    {
        ShowStep();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Nếu bấm chuột hoặc chạm vào màn hình
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

        if (textGuide != null)
        {
            textGuide.text = tutorialString[currentStringStep]; // Cập nhật nội dung text
            Invoke(nameof(UpdateTextHighlight), 0.05f); // Chờ 0.05 giây để TextMeshPro cập nhật layout
        }

        if (currentTarget == null)
        {
            // Nếu bước này chỉ là hiển thị text, ẩn lỗ bấm
            tutorialClickPanel.gameObject.SetActive(false);
        }
        else
        {
            // Hiển thị lỗ bấm và di chuyển nó
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

            pointClick.localScale = new Vector3(
                tutorialPointSize[currentSizeStep].x,
                tutorialPointSize[currentSizeStep].y,
                tutorialPointSize[currentSizeStep].z
            );

            currentSizeStep++;
        }

        currentStringStep++;
    }

    void UpdateTextHighlight()
    {
        if (textGuide == null || textHighlight == null) return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(textGuide.rectTransform);

        textHighlight.gameObject.SetActive(true);
        textHighlight.position = textGuide.transform.position;

        float paddingX = 10f; // Thêm padding cho rộng hơn một chút
        float paddingY = 15f; // Thêm padding cho cao hơn một chút
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

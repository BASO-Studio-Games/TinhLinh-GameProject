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

        if (currentTarget == null)
        {
            // Nếu bước này chỉ là hiển thị text, ẩn lỗ bấm
            tutorialClickPanel.gameObject.SetActive(false);
            textGuide.text = tutorialString[currentStringStep];
        }
        else
        {
            // Hiển thị lỗ bấm và di chuyển nó
            tutorialClickPanel.gameObject.SetActive(true);
            Vector3 targetPos = currentTarget.transform.position;
            pointClick.position = targetPos;

            if (tutorialPointSize == null || tutorialPointSize.Length == 0)
            {
                tutorialPointSize = new Vector3[targetClick.Length];
                for (int i = 0; i < tutorialPointSize.Length; i++)
                {
                    tutorialPointSize[i] = new Vector3(1, 1, 1);
                }
            }

            pointClick.localScale = new Vector3(tutorialPointSize[currentSizeStep].x, tutorialPointSize[currentSizeStep].y, tutorialPointSize[currentSizeStep].z);
            currentSizeStep++;
            textGuide.text = tutorialString[currentStringStep];
        }
        currentStringStep++;
    }


    // void ExecuteClick(GameObject target)
    // {
    //     PointerEventData eventData = new PointerEventData(EventSystem.current);
    //     eventData.position = target.transform.position;

    //     List<RaycastResult> results = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(eventData, results);

    //     foreach (var result in results)
    //     {
    //         if (result.gameObject == target)
    //         {
    //             ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerClickHandler);
    //             break;
    //         }
    //     }
    // }

    void EndTutorial()
    {
        pointClick.gameObject.SetActive(false);
        textGuide.text = "Hướng dẫn hoàn tất!";
        gameObject.SetActive(false);
    }
}

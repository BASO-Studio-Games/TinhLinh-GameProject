using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationMenu : MonoBehaviour
{
    public static NotificationMenu Instance; // Singleton

    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private float holdTimeNotification;
    
    private void Awake()
    {
        Instance = this;
    }

    public void ShowNotificationPanel(string message, Color color){
        notificationPanel.SetActive(true);

        TMP_Text textNotification = notificationPanel.GetComponentInChildren<TMP_Text>();
        if (textNotification != null){
            textNotification.text = message;
            textNotification.color = color;
        }

        StartCoroutine(HideNotificationPanel());
    }

    private IEnumerator HideNotificationPanel(){
        yield return new WaitForSeconds(holdTimeNotification);

        notificationPanel.SetActive(false);
    }
}

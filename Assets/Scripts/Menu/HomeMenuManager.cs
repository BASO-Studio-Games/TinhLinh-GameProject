using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Globalization;

public class HomeMenuManager : MonoBehaviour
{    
    [Header("Nút bấm:")]//----------
    [SerializeField] private Button informationButton;
    [SerializeField] private Button adventuresButton;
    [SerializeField] private Button miniGamesButton;
    [SerializeField] private Button bookButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button commentButton;

    [Header("Thông tin:")]//----------
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text diamondText;

    [SerializeField] private TMP_Text debugText;


    void Start()
    {
        // Cài đặt nút bấm
        informationButton.onClick.AddListener(OnClickInformationButton);
        adventuresButton.onClick.AddListener(OnClickAdventuresButton);
        miniGamesButton.onClick.AddListener(OnClickMiniGamesButtonButton);
        bookButton.onClick.AddListener(OnClickBookButton);
        shopButton.onClick.AddListener(OnClickShopButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        commentButton.onClick.AddListener(OnClickCommentButton);
    }

    
    void Update()
    {

    }

    private void OnClickInformationButton(){
        debugText.text = "Console Log: Thông tin người chơi.";
    }

    private void OnClickAdventuresButton(){
        SceneLoader loader = gameObject.GetComponentInParent<SceneLoader>();
        loader.LoadScene(2);
    }

    private void OnClickMiniGamesButtonButton(){
        debugText.text = "Console Log: Màn chơi phụ.";
    }

    private void OnClickBookButton(){
        debugText.text = "Console Log: Thông tin trò chơi.";
    }

    private void OnClickShopButton(){
        debugText.text = "Console Log: Cửa hàng.";
    }

    private void OnClickSettingButton(){
        debugText.text = "Console Log: Cài đặt.";
    }

    private void OnClickCommentButton(){
        debugText.text = "Console Log: Góp ý phát triển trò chơi.";
    }
}

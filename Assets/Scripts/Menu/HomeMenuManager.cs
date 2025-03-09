using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Globalization;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking;

public class HomeMenuManager : MonoBehaviour
{    
    [Header("Nút bấm:")]//----------
    [SerializeField] private Button backHomeButton;
    [SerializeField] private Button informationButton;
    [SerializeField] private Button adventuresButton;
    [SerializeField] private Button miniGamesButton;
    [SerializeField] private Button bookButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button commentButton;

    [Header("Màn hình:")]//----------
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject bookScreen;

    [Header("Thông tin:")]//----------
    private User user;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private Image avatarImage;

    [SerializeField] private TMP_Text debugText;

    private DatabaseReference dbReference;


    void Start()
    {
        // Cài đặt nút bấm
        backHomeButton.onClick.AddListener(OnClickBackHomeButton);
        informationButton.onClick.AddListener(OnClickInformationButton);
        adventuresButton.onClick.AddListener(OnClickAdventuresButton);
        miniGamesButton.onClick.AddListener(OnClickMiniGamesButtonButton);
        bookButton.onClick.AddListener(OnClickBookButton);
        shopButton.onClick.AddListener(OnClickShopButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        commentButton.onClick.AddListener(OnClickCommentButton);

        shopScreen.SetActive(false);

        // Firebase
        InitFirebase();
    }

    
    void Update()
    {

    }

    private void OnClickBackHomeButton(){
        string userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
        GetAllDataUser(userID);

        shopScreen.SetActive(false);
    }

    private void OnClickInformationButton(){
        debugText.text = "Console Log: Thông tin người chơi.";
    }

    private void OnClickAdventuresButton(){
        SceneLoader loader = gameObject.GetComponentInParent<SceneLoader>();
        loader.LoadScene("Map - 1");
    }

    private void OnClickMiniGamesButtonButton(){
        debugText.text = "Console Log: Màn chơi phụ.";
    }

    private void OnClickBookButton(){
        debugText.text = "Console Log: Thông tin trò chơi.";
    }

    private void OnClickShopButton(){
        shopScreen.SetActive(true);
        debugText.text = "Console Log: Cửa hàng.";
    }

    private void OnClickSettingButton(){
        debugText.text = "Console Log: Cài đặt.";
    }

    private void OnClickCommentButton(){
        debugText.text = "Console Log: Góp ý phát triển trò chơi.";
    }

    // ----------Firebase----------
    private void InitFirebase(){
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            // Kết nối tới Database
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            string userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
            GetAllDataUser(userID);
        });
    }

    // Đọc dữ liệu từ Firebase
    private void GetAllDataUser(string idPlayer)
    {
        Debug.Log($"Đọc dữ liệu từ ID {idPlayer}");
        dbReference.Child("Users").Child(idPlayer).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Chuyển đổi dữ liệu JSON thành đối tượng User
                string jsonData = snapshot.GetRawJsonValue();
                user = JsonUtility.FromJson<User>(jsonData);

                // Kiểm tra kết quả
                if (user != null)
                {
                    Debug.Log($"Lấy dữ liệu từ {user.username} thành công.");

                    usernameText.text = user.username;
                    diamondText.text = user.diamond + " Kim cương";
                    
                    if (!string.IsNullOrEmpty(user.avatarUrl))
                    {
                        // Bắt đầu tải ảnh từ URL
                        StartCoroutine(LoadImageFromUrl(user.avatarUrl));
                    }
                    else
                    {
                        Debug.LogWarning($"Không tìm thấy Url Avatar của {user.username}.");
                    }
                }
                else
                {
                    Debug.LogWarning("Dữ liệu từ Firebase không hợp lệ hoặc bị thiếu!");
                }
            }
            else
            {
                Debug.LogError($"Lỗi khi đọc dữ liệu: {task.Exception}");
            }
        });
    }

    private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Chuyển đổi Texture thành Sprite và gắn vào Image
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                avatarImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Tải ảnh thất bại: " + request.error);
            }
        }
    }
}

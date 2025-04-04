using System.Collections;
using TMPro;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Data.Common;

public class StartMenuManager : MonoBehaviour
{   
    [Header("Screen:")]//----------
    [SerializeField] private GameObject checkingVersionScreen;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject loaderScreen;

    [Header("Đăng nhập:")]//----------
    [SerializeField] private TMP_Text informationLoginText; 
    [SerializeField] private Button loginButton; 
    [SerializeField] private float intervalLogin; // Khoảng thời gian hiện ra thông báo nút bấm

    [Header("Kiểm tra phiên bản:")]//----------
    [SerializeField] private TMP_Text loadingCheckText; // Kéo thả Text UI vào đây trong Inspector
    [SerializeField] private float durationCheckInternet; // Thời gian chạy hiệu ứng
    [SerializeField] private float durationCheckVersion; // Thời gian chạy hiệu ứng
    [SerializeField] private float intervalLoading; // Khoảng thời gian giữa các thay đổi ("Load.", "Load..", ...)

    [Header("Tài nguyên khởi đầu:")]//----------
    [SerializeField] private int diamondUser;
    [SerializeField] private int rollNumberUser;
    [SerializeField] private int currencyLevelUser = 11;


    private DatabaseReference dbReference;

    private void Start()
    {
        // Màn hình khởi tạo
        checkingVersionScreen.SetActive(true);
        loginScreen.SetActive(false);
        loaderScreen.SetActive(false);

        StartCoroutine(LoadingCheckTextAnimation());

        // Thêm sự kiện cho nút đăng nhập
        loginButton.onClick.AddListener(OnClickLoginButton);
        loginButton.interactable = false;

        InitFirebase();
    }

    private IEnumerator LoadingCheckTextAnimation()
    {
        float elapsedTime = 0f;
        int dotCount = 0;

        while (elapsedTime < durationCheckInternet)
        {
            // Thay đổi nội dung Text
            loadingCheckText.text = "Đang kiểm tra kết nối" + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Quay lại 0 sau khi đạt 3 (Load...)
            
            // Chờ một khoảng thời gian
            yield return new WaitForSeconds(intervalLoading);

            elapsedTime += intervalLoading;

            // Kiểm tra trạng thái kết nối mạng
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                loadingCheckText.text = "Vui lòng kiểm tra kết nối mạng!!!";
                yield break;
            }
        }   

        elapsedTime = 0f;
        dotCount = 0;

        while (elapsedTime < durationCheckVersion)
        {
            // Thay đổi nội dung Text
            loadingCheckText.text = "Đang kiểm tra phiên bản" + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Quay lại 0 sau khi đạt 3 (Load...)
            
            // Chờ một khoảng thời gian
            yield return new WaitForSeconds(intervalLoading);

            elapsedTime += intervalLoading;
        }
        
        // Hoàn tất kiểm tra, chuyển màn hình
        checkingVersionScreen.SetActive(false);
        SignIn();
        loginScreen.SetActive(true);
    }

    // ----------Google Game Play----------
    private void SignIn()
    {
        informationLoginText.text = "Đang thực hiện đăng nhập.";

        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     // Chạy trên thiết bị Android thực sự
        //     PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        // }
        // else
        // {
        //     // Chạy trong Unity Editor nhưng giả lập Android
        //     string userID = SystemInfo.deviceUniqueIdentifier;
        //     string username = userID.Substring(userID.Length - 6);

        //     informationLoginText.text = "Xin chào tài khoản, " + username;
        //     PlayerPrefs.SetString("ID_User", userID);
        //     CheckTestDataExistence(userID, username, null, diamondUser.ToString(), rollNumberUser.ToString(), currencyLevelUser.ToString());
        //     StartCoroutine(LoadGame());
        // }
        string userID = SystemInfo.deviceUniqueIdentifier;
        string username = userID.Substring(userID.Length - 6);

        informationLoginText.text = "Xin chào tài khoản, " + username;
        PlayerPrefs.SetString("ID_User", userID);
        CheckTestDataExistence(userID, username, null, diamondUser.ToString(), rollNumberUser.ToString(), currencyLevelUser.ToString());
        StartCoroutine(LoadGame());
    }

    private void OnClickLoginButton()
    {
        StartCoroutine(ReLogin());
        // StartCoroutine(LoadGame());
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            string username = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

            informationLoginText.text = "Xin chào tài khoản, " + username;

            PlayerPrefs.SetString("ID_User", id);
            CheckTestDataExistence(id, username, ImgUrl, diamondUser.ToString(), rollNumberUser.ToString(), currencyLevelUser.ToString());
            StartCoroutine(LoadGame());
        }
        else
        {
            informationLoginText.text = "Đăng nhập thất bại!!\n[Chạm vào đây để thử lại.]";
            loginButton.interactable = true;
        }
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(intervalLogin);

        loginScreen.SetActive(false);
        loaderScreen.SetActive(true);

        SceneLoader loader = gameObject.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
    }

    private IEnumerator ReLogin()
    {
        informationLoginText.text = "Đang thực hiện đăng nhập.";
        loginButton.interactable = false;

        yield return new WaitForSeconds(intervalLogin);

        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    // ----------Firebase----------
    private void InitFirebase(){
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            // Kết nối tới Database
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    private void CreateUserToFirebase(string idPlayer, string username, string avatarUrl, string diamond, string roll, string currencyLevelUser)
    {    
        User newUser = new User(username, avatarUrl, diamond, roll, currencyLevelUser);
        string jsonData = JsonUtility.ToJson(newUser);

        dbReference.Child("Users").Child(idPlayer).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Dữ liệu đã được ghi: {jsonData}");
            }
            else
            {
                Debug.LogError($"Lỗi khi ghi dữ liệu: {task.Exception}");
            }
        });
    }

    // Kiểm tra sự tồn tại của Id player trên Firebase
    private void CheckTestDataExistence(string idPlayer, string username, string avatarUrl, string diamond, string roll, string currencyLevelUser)
    {
        dbReference.Child("Users").Child(idPlayer).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    Debug.Log($"{idPlayer} đã tồn tại trong cơ sở dữ liệu. Đang vào trò chơi.");
                }
                else
                {
                    Debug.Log($"{idPlayer} chưa tồn tại trong cơ sở dữ liệu. Đang tạo user mới...");
                    CreateUserToFirebase(idPlayer, username, avatarUrl, diamond, roll, currencyLevelUser);
                }
            }
            else
            {
                Debug.LogError($"Lỗi khi kiểm tra dữ liệu: {task.Exception}");
            }
        });
    }
}

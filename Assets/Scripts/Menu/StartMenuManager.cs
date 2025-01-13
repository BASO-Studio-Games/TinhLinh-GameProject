using System.Collections;
using TMPro;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{   
    [Header("Screen:")]//----------
    [SerializeField] private GameObject checkingVersionScreen;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject loaderScreen;
    [SerializeField] private int indexHomeMenuScreen;

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

    void Start()
    {

        // Màn hình khởi tạo
        checkingVersionScreen.SetActive(true);
        loginScreen.SetActive(false);
        loaderScreen.SetActive(false);

        StartCoroutine(LoadingCheckTextAnimation());

        // Thêm sự kiện cho nút đăng nhập
        loginButton.onClick.AddListener(OnClickLoginButton);
        loginButton.interactable = false;
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

    private void SignIn()
    {
        informationLoginText.text = "Đang thực hiện đăng nhập.";
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    private void OnClickLoginButton()
    {
        // StartCoroutine(ReLogin());
        StartCoroutine(LoadGame());
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();

            informationLoginText.text = "Xin chào, " + name;

            // Lưu ID người chơi
            PlayerPrefs.SetString("ID_User", id);

            // Lưu DB người chơi
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
        loader.LoadScene(indexHomeMenuScreen);
    }

    private IEnumerator ReLogin()
    {
        informationLoginText.text = "Đang thực hiện đăng nhập.";
        loginButton.interactable = false;

        yield return new WaitForSeconds(intervalLogin);

        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }
}

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WInOrLoseMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string nextMapName;

    [SerializeField] private TMP_Text rollRewardText; // Hiển thị thưởng lượt quay
    [SerializeField] private TMP_Text diamondRewardText; // Hiển thị thưởng kim cương

    [SerializeField] private int WinRoll;
    [SerializeField] private int WinDiamond;

    [SerializeField] private int CloseRoll;
    [SerializeField] private int CloseDiamond;

    private DatabaseReference dbReference;
    private string userID;

    void OnEnable()
    {
        InitFirebase();
        Time.timeScale = 0;
    }

    private void GiveReward(bool isWin)
    {
        if (rollRewardText == null || diamondRewardText == null)
        {
            Debug.LogError("⚠️ rollRewardText hoặc diamondRewardText chưa được gán trong Inspector!");
            return;
        }

        if (dbReference == null)
        {
            Debug.LogError("⚠️ dbReference chưa được khởi tạo!");
            return;
        }

        userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogError("⚠️ userID chưa được lưu trong PlayerPrefs!");
            return;
        }

        int rollReward = isWin ? WinRoll : CloseRoll;
        int diamondReward = isWin ? WinDiamond : CloseDiamond;

        // Hiển thị phần thưởng trên UI
        rollRewardText.text = $"+{rollReward} lượt quay";
        diamondRewardText.text = $"+{diamondReward} kim cương";

        // Lấy dữ liệu từ Firebase, sau đó cập nhật
        dbReference.Child("Users").Child(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;

                int currentRoll = int.Parse(snapshot.Child("roll").Value.ToString());
                int currentDiamond = int.Parse(snapshot.Child("diamond").Value.ToString());

                currentRoll += rollReward;
                currentDiamond += diamondReward;

                // Lưu lên Firebase
                dbReference.Child("Users").Child(userID).Child("roll").SetValueAsync(currentRoll);
                dbReference.Child("Users").Child(userID).Child("diamond").SetValueAsync(currentDiamond);

                Debug.Log($"✅ Cập nhật thành công: {currentRoll} lượt quay, {currentDiamond} kim cương");
            }
            else
            {
                Debug.LogError("❌ Không thể lấy dữ liệu từ Firebase!");
            }
        });
    }

    public void Next()
    {
        if (nextMapName != null)
        {
            SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
            loader.LoadScene(nextMapName);

            // Cập nhật level hiện tại lên Firebase
            dbReference.Child("Users").Child(userID).Child("currentLevel").SetValueAsync("1" + nextMapName[nextMapName.Length - 1]);

            Time.timeScale = 1f;
        }
        else
        {
            Home();
        }
    }

    public void Home()
    {
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    // ----------Firebase----------
    private void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("✅ Firebase đã khởi tạo!");

                // Gọi GiveReward() sau khi Firebase sẵn sàng
                bool isWin = LevelManager.main.winner;
                GiveReward(isWin);
            }
            else
            {
                Debug.LogError("❌ Lỗi khởi tạo Firebase!");
            }
        });
    }
}

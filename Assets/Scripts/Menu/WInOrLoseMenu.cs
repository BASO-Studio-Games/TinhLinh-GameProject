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

    [SerializeField] private TMP_Text rollRewardText;
    [SerializeField] private TMP_Text diamondRewardText;

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

        int rollReward = isWin ? Random.Range(1, 6) : 0;
        int diamondReward = isWin ? Random.Range(5, 51) : 0;

        rollRewardText.text = rollReward > 0 ? $"+{rollReward} lượt quay" : "Không có thưởng";
        diamondRewardText.text = diamondReward > 0 ? $"+{diamondReward} kim cương" : "Không có thưởng";

        if (!isWin) return;

        dbReference.Child("Users").Child(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DataSnapshot snapshot = task.Result;

                int currentRoll = int.Parse(snapshot.Child("roll").Value.ToString());
                int currentDiamond = int.Parse(snapshot.Child("diamond").Value.ToString());

                currentRoll += rollReward;
                currentDiamond += diamondReward;

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
            Debug.Log(nextMapName);
            sceneLoader.LoadScene(nextMapName);
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
        sceneLoader.LoadScene("Home Menu");
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        sceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    private void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("✅ Firebase đã khởi tạo!");
                GiveReward(LevelManager.main.winner);
            }
            else
            {
                Debug.LogError("❌ Lỗi khởi tạo Firebase!");
            }
        });
    }
}

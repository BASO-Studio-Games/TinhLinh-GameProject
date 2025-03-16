using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WInOrLoseMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string nextMapName;

    private DatabaseReference dbReference;
    
    void OnEnable()
    {
        InitFirebase();
        Time.timeScale = 0;
    }

    public void Next()
    {
        if (nextMapName != null){
            SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
            loader.LoadScene(nextMapName);

            // Cập nhật dữ liệu trên Firebase
            string userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
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
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
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
}


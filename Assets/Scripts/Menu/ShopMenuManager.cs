using System.Net.Security;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private TMP_Text rollText;
    [SerializeField] private GameObject[] Items;
    private Button[] buttonItems;

    private User user;
    private DatabaseReference dbReference;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonItems = new Button[Items.Length];

        int count = 0;

        foreach (GameObject item in Items)
        {
            buttonItems[count] = item.GetComponentInChildren<Button>();
            if (buttonItems[count] != null)
            {
                }
                if (int.TryParse(buttonItems[count].GetComponentInChildren<TMP_Text>().text, out int price))
                {
                    switch (item.name){
                    case "Roll Item":
                        buttonItems[count].onClick.AddListener( () => BuyRollItem(price));
                        break;
                    }
                }
                else
                {
                    Debug.LogError("Không thể chuyển đổi tiền thành int.");
                }

            count++;
        }

        InitFirebase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyRollItem(int value)
    {
        if (user == null)
        {
            Debug.LogError("User data chưa được tải!");
            return;
        }

        int currentDiamond = int.Parse(user.diamond);
        int currentRoll = int.Parse(user.roll);

        if (currentDiamond >= value)
        {
            // Trừ vàng và tăng lượt roll
            currentDiamond -= value;
            currentRoll += 1;

            // Cập nhật dữ liệu trên Firebase
            string userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
            dbReference.Child("Users").Child(userID).Child("diamond").SetValueAsync(currentDiamond);
            dbReference.Child("Users").Child(userID).Child("roll").SetValueAsync(currentRoll);

            // Cập nhật UI
            diamondText.text = currentDiamond.ToString() + " Kim cương";
            rollText.text = currentRoll.ToString() + " Lượt quay";

            Debug.Log("Mua lượt roll thành công! Số vàng còn lại: " + currentDiamond);
        }
        else
        {
            Debug.Log("Không đủ vàng để mua lượt roll!");
        }
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

                    rollText.text = user.roll + " Lượt quay";
                    diamondText.text = user.diamond + " Kim cương";
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
}

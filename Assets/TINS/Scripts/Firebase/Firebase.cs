using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;

public class FirebaseTest : MonoBehaviour
{
    public TMP_Text debugText;
    public TMP_InputField keyInputField;
    public TMP_InputField valueInputField;


    private DatabaseReference dbReference;

    void Start()
    {
        debugText.text = "...";
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            // Kết nối tới Database
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Đọc dữ liệu từ Firebase
            ReadData();
        });
    }

    public void SaveData(){
        WriteData(keyInputField.text, valueInputField.text);
    }

    // Ghi dữ liệu lên Firebase
    public void WriteData(string key, string value)
    {
        dbReference.Child("TestData").Child(key).SetValueAsync(value).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Dữ liệu đã được ghi: {key} - {value}");
                debugText.text += $"\nDữ liệu đã được ghi: {key} - {value}";
            }
            else
            {
                Debug.LogError($"Lỗi khi ghi dữ liệu: {task.Exception}");
                debugText.text += $"\nLỗi khi ghi dữ liệu: {task.Exception}";
            }
        });
    }

    // Đọc dữ liệu từ Firebase
    public void ReadData()
    {
        dbReference.Child("TestData").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var child in snapshot.Children)
                {
                    debugText.text = "Read: ";
                    Debug.Log($"Key: {child.Key}, Value: {child.Value}");
                    debugText.text += $"\nKey: {child.Key}, Value: {child.Value}";
                }
            }
            else
            {
                Debug.LogError($"Lỗi khi đọc dữ liệu: {task.Exception}");
                debugText.text += $"\nLỗi khi đọc dữ liệu: {task.Exception}";
            }
        });
    }

    // // Ví dụ gọi từ Unity
    // public void TestWriteAndRead()
    // {
    //     WriteData("exampleKey", "Hello Firebase!");
    //     ReadData();
    // }
}

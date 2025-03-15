using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollShopUI : MonoBehaviour
{
    public RollManager rollManager;
    public Button rollButton;
    private TMP_Text rollCountText;
    public Button resetButton;
    public Button endButton;
    public Transform rollContainer;
    public GameObject rollPrefab;

    private List<RollItem> currentRolls = new List<RollItem>();
    private RollItem currentItem;

    private DatabaseReference dbReference;
    private int currentRollsCount; // Số roll hiện tại của người chơi
    private string userID;

    private void Start()
    {
        if (rollManager == null)
            rollManager = GetComponent<RollManager>();

        rollButton.onClick.AddListener(() => CheckAndRoll(true));
        // resetButton.onClick.AddListener(ResetRoll);
        endButton.onClick.AddListener(EndRoll);

        rollCountText = rollButton.GetComponentInChildren<TMP_Text>();

        ResetRoll();

        userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        LoadUserRolls();
    }

    private void Update() {
        if (rollManager.hasBuy){
            currentRolls.Remove(currentItem);
            UpdateRollUI(false);
            rollManager.hasBuy = false;
        }
    }

    private void LoadUserRolls()
    {
        dbReference.Child("Users").Child(userID).Child("roll").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Đã tải dữ liệu roll: " + snapshot);
                if (snapshot.Exists)
                {
                    currentRollsCount = int.Parse(snapshot.Value.ToString());
                }
                else
                {
                    currentRollsCount = 0;
                }

                rollCountText.text = currentRollsCount.ToString();
            }
            else
            {
                Debug.LogError("Lỗi tải dữ liệu roll: " + task.Exception);
                currentRollsCount = 0;
            }
        });
    }

    private void CheckAndRoll(bool isRoll)
    {
        if (currentRollsCount > 0)
        {
            UpdateRollUI(isRoll);
            currentRollsCount--;

            rollCountText.text = currentRollsCount.ToString();

            // Cập nhật lại số roll trên Firebase
            dbReference.Child("Users").Child(userID).Child("roll").SetValueAsync(currentRollsCount);
        }
        else
        {
            Debug.Log("Không đủ lượt roll!");
        }
    }

    private void UpdateRollUI(bool isRoll)
    {
        ClearRollUI();
        if (isRoll)
            currentRolls = rollManager.Roll();
        

        foreach (var item in currentRolls)
        {
            GameObject obj = Instantiate(rollPrefab, rollContainer);
            TMP_Text[] texts = obj.GetComponentsInChildren<TMP_Text>();

            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name.Contains("Name")) // Nếu là tên vật phẩm
                {
                    text.text = item.name;
                }
                else if (text.gameObject.name.Contains("Price"))
                {
                    text.text = item.GetCost().ToString();
                }
                else if (text.gameObject.name.Contains("Info"))
                {
                    text.text = item.GetInformation();
                }
            }

            Image[] images = obj.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                if (image.gameObject.name.Contains("Sprite")) // Nếu là tên vật phẩm
                {
                    image.sprite = item.GetSprite();
                }
                else if (image.gameObject.name.Contains("Level")) // Nếu là tên vật phẩm
                {
                    image.sprite = item.GetLevelSprite();
                }
                else if (image.gameObject.name.Contains("Class")) // Nếu là tên vật phẩm
                {
                    image.sprite = item.GetClassSprite();
                }
            }

            Button buyButton = obj.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    private void BuyItem(RollItem item)
    {
        if (LevelManager.main.CheckCurrency(item.GetCost())) // Kiểm tra đủ vàng không
        {
            if (item.GetIdTinhLinh() == "Tool00"){
                LevelManager.main.SpendCurrency(item.GetCost());
                ResetRoll();
                return;
            }

            rollManager.BuyItem(item);
            currentItem = item;
        }
        else
        {
            Debug.Log("Không đủ vàng!");
        }
    }

    public void ResetRoll()
    {
        rollManager.ResetRoll();
        UpdateRollUI(true);
    }

    private void ClearRollUI()
    {
        foreach (Transform child in rollContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void EndRoll()
    {
        rollManager.EndRoll();
        UpdateRollUI(true);
    }
}

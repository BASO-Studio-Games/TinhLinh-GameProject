using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollShopUI : MonoBehaviour
{
    public RollManager rollManager;
    public Button rollButton;
    public Button resetButton;
    public Button endButton;
    public Transform rollContainer;
    public GameObject rollPrefab;

    private List<RollItem> currentRolls = new List<RollItem>();
    private RollItem currentItem;

    private void Start()
    {
        if (rollManager == null)
            rollManager = GetComponent<RollManager>();

        rollButton.onClick.AddListener(() => UpdateRollUI(true));
        resetButton.onClick.AddListener(ResetRoll);
        endButton.onClick.AddListener(EndRoll);

        ResetRoll();
    }

    private void Update() {
        if (rollManager.hasBuy){
            currentRolls.Remove(currentItem);
            UpdateRollUI(false);
            rollManager.hasBuy = false;
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
                // else if (text.gameObject.name.Contains("Info"))
                // {
                //     text.text = item.information;
                // }
            }

            Button buyButton = obj.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    private void BuyItem(RollItem item)
    {
        if (LevelManager.main.CheckCurrency(item.GetCost())) // Kiểm tra đủ vàng không
        {
            rollManager.BuyItem(item);
            currentItem = item;
        }
        else
        {
            Debug.Log("Không đủ vàng!");
        }
    }

    private void ResetRoll()
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

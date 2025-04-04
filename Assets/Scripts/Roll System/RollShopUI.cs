using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollShopUI : MonoBehaviour
{
    public RollManager rollManager;
    public Button rollButton;
    private TMP_Text costOfRollText;
    public Button resetButton;
    public Button endButton;
    public Transform rollContainer;
    public GameObject rollPrefab;

    private List<RollItem> currentRolls = new List<RollItem>();
    private RollItem currentItem;

    private int costOfRoll;

    [SerializeField] private GameObject warningPanel;
    [SerializeField] private CanvasGroup warningCanvas;


    private void Start()
    {
        if (rollManager == null)
        {
            Debug.LogWarning("rollManager chưa được gán! Kiểm tra Inspector.");
        }
        else
        {
            rollManager.ResetRoll();
        }

        rollManager = GetComponent<RollManager>();

        rollButton.onClick.AddListener(() => CheckAndRoll(true));
        // resetButton.onClick.AddListener(ResetRoll);
        endButton.onClick.AddListener(EndRoll);

        costOfRollText = rollButton.GetComponentInChildren<TMP_Text>();
        costOfRoll = 10;
        costOfRollText.text = costOfRoll.ToString();

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

    private void CheckAndRoll(bool isRol)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance bị null!");
            return;
        }

        Debug.Log("Gọi PlaySFX(ButtonUI)");
        AudioManager.Instance.PlaySFX("ButtonUI");

        if (LevelManager.main.GetCurrency() >= costOfRoll)
        {
            UpdateRollUI(isRol);
            LevelManager.main.SpendCurrency(costOfRoll);

            costOfRoll *= 2;
            if (costOfRoll > 9999) costOfRoll = 9999;

            costOfRollText.text = costOfRoll.ToString();
        }
        else
        {
            Debug.Log("Không đủ tiền để roll!");
            ShowWarning();
        }
    }


    private void BuyItem(RollItem item)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance bị null!");
            return;
        }

        Debug.Log("Gọi PlaySFX(ButtonUI)");
        AudioManager.Instance.PlaySFX("ButtonUI");

        if (LevelManager.main.CheckCurrency(item.GetCost())) // Kiểm tra đủ vàng không
        {
            if (item.GetIdTinhLinh() == "Tool00")
            {
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
            ShowWarning();
        }
    }

    private void ShowWarning()
    {
        if (warningPanel != null && warningCanvas != null)
        {
            warningPanel.SetActive(true);
            warningCanvas.alpha = 1f;
            StartCoroutine(HideWarning());
        }
        else
        {
            Debug.LogError("⚠️ warningPanel hoặc warningCanvas chưa được gán trong Inspector!");
        }
    }

    private IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(1f); // Đợi 1 giây trước khi bắt đầu làm mờ

        float duration = 1f; // Thời gian mờ dần
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            warningCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }

        warningPanel.SetActive(false); // Tắt panel sau khi mờ dần
    }


    public void ResetRoll()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance bị null!");
            return;
        }

        Debug.Log("Gọi PlaySFX(ButtonUI)");
        AudioManager.Instance.PlaySFX("ButtonUI");

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

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance bị null!");
            return;
        }

        Debug.Log("Gọi PlaySFX(ButtonUI)");
        AudioManager.Instance.PlaySFX("ButtonUI");

        costOfRoll = 10;
        costOfRollText.text = costOfRoll.ToString();

        rollManager.EndRoll();
        UpdateRollUI(true);
    }

}

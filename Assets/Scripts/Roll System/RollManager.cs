using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollManager : MonoBehaviour
{
    public RollData rollData;
    public GameObject rollMenu;
    public int maxRolls = 4;
    private int remainingSlots;

    private EnemySpawner enemySpawner;
    private bool isUseItem;
    private bool isTool;
    private bool isUseTool;
    [SerializeField] private Transform defendersContainer;
    private int currentchildCount;
    [HideInInspector] public bool hasBuy;
    private bool isEndRollMenu;

    [SerializeField] private Button rollButton;
    private bool isRollButtonActive = false;

    private void Start()
    {
        isEndRollMenu = false;

        enemySpawner = GetComponentInParent<EnemySpawner>();
        enemySpawner.enabled = false;

        rollMenu.SetActive(true);
        rollButton.gameObject.SetActive(false);
        isUseItem = false;

        hasBuy = false;
    }

    private void Update()
    {
        if (isUseItem && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            if (isUseTool) return;

            if (defendersContainer.childCount > currentchildCount)
            {
                hasBuy = true;
                remainingSlots = Mathf.Max(remainingSlots - 1, 0);
            }
            else
            {
                BuildManager.main.SetSelectedTower(null);
                BuildManager.main.SetSelectedTool(null);
            }

            rollMenu.SetActive(true);
            isUseItem = false;
        }

        int remainingCards = 0;
        foreach (Transform child in rollMenu.transform)
        {
            if (child.gameObject.activeSelf)
            {
                remainingCards++;
            }
        }

        // Debug.Log($"[RollManager] Remaining Slots: {remainingSlots}, Remaining Cards: {remainingCards}");

        if (remainingSlots == 0)
        {
            EndRoll();
        }

        if (LevelManager.main.isMaxEnergy && isEndRollMenu)
        {
            rollButton.gameObject.SetActive(true);
            rollButton.interactable = true;
            isRollButtonActive = true;
        }
    }

    public void OpenRollPanel()
    {
        if (!isRollButtonActive) return;

        rollMenu.SetActive(true);
        rollButton.gameObject.SetActive(false);
        isUseItem = false;
        enemySpawner.enabled = false;
        isEndRollMenu = false;

        isRollButtonActive = false;
        rollButton.interactable = false;
    }

    public List<RollItem> Roll()
    {
        List<RollItem> result = new List<RollItem>();
        float totalProbability = 0f;

        foreach (var item in rollData.rollItems)
        {
            totalProbability += item.probability;
        }

        for (int i = 0; i < remainingSlots; i++)
        {
            float rand = Random.Range(0f, totalProbability);
            float cumulative = 0f;

            foreach (var item in rollData.rollItems)
            {
                cumulative += item.probability;
                if (rand <= cumulative)
                {
                    result.Add(item);
                    break;
                }
            }
        }

        return result;
    }

    public void BuyItem(RollItem item)
    {
        if (remainingSlots <= 0) return;
        currentchildCount = defendersContainer.childCount;

        if (item.GetIsTool())
        {
            if (item.GetIdTinhLinh() == "Tool00")
            {
                hasBuy = true;
                return;
            }

            Tool tool = item.prefab.GetComponent<Tool>();
            if (tool != null)
            {
                BuildManager.main.SetSelectedTool(tool);
                isUseItem = true;
                rollMenu.SetActive(false);
                rollButton.gameObject.SetActive(true);
            }
        }
        else
        {
            isUseItem = true;
            rollMenu.SetActive(false);
            BuildManager.main.SetSelectedTower(item);
            rollButton.gameObject.SetActive(true);
        }
    }

    public void ResetRoll()
    {
        remainingSlots = maxRolls;
    }

    public void EndRoll()
    {
        enemySpawner.enabled = true;

        ResetRoll();
        rollMenu.SetActive(false);
        rollButton.gameObject.SetActive(true);

        isEndRollMenu = true;

        if (LevelManager.main.isMaxEnergy)
            LevelManager.main.isMaxEnergy = false;
    }

    private void OnEnable()
    {
        Tool.OnToolActionCompleted += HandleToolActionCompleted;
    }

    private void OnDisable()
    {
        Tool.OnToolActionCompleted -= HandleToolActionCompleted;
    }

    private void HandleToolActionCompleted(Tool tool, Tile fromTile, Tile toTile, bool isDelete, bool isSwap)
    {
        if (isDelete)
        {
            if (fromTile != null)
            {
                hasBuy = true;
                remainingSlots = Mathf.Max(remainingSlots - 1, 0);
            }
            isUseTool = false;
            return;
        }

        if (toTile == null)
        {
            isUseTool = true;
            return;
        }

        hasBuy = true;
        remainingSlots = Mathf.Max(remainingSlots - 1, 0);
        isUseTool = false;
    }
}

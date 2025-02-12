using System.Collections.Generic;
using UnityEngine;

public class RollManager : MonoBehaviour
{
    public RollData rollData;
    public GameObject rollMenu;
    public int maxRolls = 4;
    private int remainingSlots;

    private EnemySpawner enemySpawner;
    private bool isUseItem;
    private bool isUseTool;
    [SerializeField] private Transform defendersContainer;
    private int currentchildCount;
    [HideInInspector] public bool hasBuy;
    private bool isEndRollMenu;


    private void Start()
    {
        isEndRollMenu = false;

        enemySpawner = GetComponentInParent<EnemySpawner>();
        enemySpawner.enabled = false;

        rollMenu.SetActive(true);
        isUseItem = false;
        
        hasBuy = false;
    }

    private void Update() {
        if (isUseItem && (Input.GetMouseButtonDown(0) || Input.touchCount > 0)){
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
    

        if (LevelManager.main.isMaxEnergy && isEndRollMenu){
            BuildManager.main.SetSelectedTower(null);

            rollMenu.SetActive(true);
            isUseItem = false;
            enemySpawner.enabled = false;

            isEndRollMenu = false;
        }
    }

    public List<RollItem> Roll()
    {
        List<RollItem> result = new List<RollItem>();
        float totalProbability = 0f;

        foreach (var item in rollData.rollItems)
        {
            totalProbability += item.probability; // Tính tổng probability
        }

        for (int i = 0; i < remainingSlots; i++)
        {
            float rand = Random.Range(0f, totalProbability); // Random từ 0 đến tổng probability
            float cumulative = 0f;

            foreach (var item in rollData.rollItems)
            {
                cumulative += item.probability; // Cộng dồn xác suất
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

        if (item.GetIsTool()) // Nếu là Tool
        {
            Tool tool = item.prefab.GetComponent<Tool>();
            if (tool != null)
            {
                BuildManager.main.SetSelectedTool(tool);
                // Debug.Log($"Đã chọn Tool: {tool.GetType().Name}, nhấn vào Tile để dùng.");

                isUseItem = true;
                rollMenu.SetActive(false);
            }
        }
        else // Nếu là Tinh Linh
        {
            isUseItem = true;
            rollMenu.SetActive(false);
            BuildManager.main.SetSelectedTower(item);
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

        isEndRollMenu = true;
        
        if (LevelManager.main.isMaxEnergy)
            LevelManager.main.isMaxEnergy = false;
    }


    // Nhận sự kiện từ tool
    private void OnEnable()
    {
        Tool.OnToolActionCompleted += HandleToolActionCompleted;
    }

    private void OnDisable()
    {
        Tool.OnToolActionCompleted -= HandleToolActionCompleted;
    }

    private void HandleToolActionCompleted(Tool tool, Tile fromTile, Tile toTile)
    {
        if (toTile == null)
        {
            // Debug.Log($"{tool.GetType().Name} đã được chọn để di chuyển từ {fromTile.name}");
            isUseTool = true;
            return;
        }

        // Debug.Log($"{tool.GetType().Name} đã hoàn thành hành động từ {fromTile.name} đến {toTile.name}");
        
        // if (tool is TinhLinhMover)
        // {
        //     Debug.Log("------------Tinh Linh đã được di chuyển!");
        // }

        hasBuy = true;
        remainingSlots = Mathf.Max(remainingSlots - 1, 0);
        isUseTool = false;
    }
}

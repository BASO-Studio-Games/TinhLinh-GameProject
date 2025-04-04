using System.Collections;
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
    private bool isTool;
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

    private void Update()
    {
        if (isUseItem && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            Debug.Log("Chamk");
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


        if (LevelManager.main.isMaxEnergy && isEndRollMenu)
        {
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

                StartCoroutine(DelayIsUseItem(0.25f));
                rollMenu.SetActive(false);
            }
        }
        else 
        {
            StartCoroutine(DelayIsUseItem(0.25f));
            rollMenu.SetActive(false);
            BuildManager.main.SetSelectedTower(item);
        }
    }

    private IEnumerator DelayIsUseItem(float delay)
    {
        yield return new WaitForSeconds(delay);
        isUseItem = true;
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

        // if (isSwap)
        // {
        //     isUseTool = false;
        //     return;
        // }

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
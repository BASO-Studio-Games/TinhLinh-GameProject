using UnityEngine;

public class ResetRollTool : Tool
{
    [SerializeField] private RollShopUI rollShopUI; 

    public override int GetMaxTaps()
    {
        return 0; // Chỉ cần 1 lần nhấn để reset roll
    }

    public override void Activate(Tile tile)
    {
        if (rollShopUI != null)
        {
            rollShopUI.ResetRoll();
            // NotifyToolActionCompleted(tile, null);
        }
        else
        {
            Debug.LogError("ResetRollTool: RollShopUI chưa được gán!");
        }
    }
}

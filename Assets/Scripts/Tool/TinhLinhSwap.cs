using UnityEngine;

public class TinhLinhSwap : Tool
{
    [SerializeField] private int maxTaps = 2;
    private static Tile selectedTile = null;
    private static Vector3 originalScale;

    public override int GetMaxTaps()
    {
        return maxTaps;
    }

    public override void Activate(Tile tile)
    {
        if (tile.HasTinhLinh() && selectedTile == null) 
        {
            selectedTile = tile;
            GameObject tinhLinh = selectedTile.GetTinhLinhObject();

            if (tinhLinh != null)
            {
                originalScale = tinhLinh.transform.localScale;
                tinhLinh.transform.localScale = originalScale * 1.5f;
            }

            NotifyToolActionCompleted(selectedTile, null);
            return;
        }

        if (selectedTile != null && tile.HasTinhLinh() && selectedTile != tile) 
        {
            ResetTinhLinhScale(selectedTile);
            SwapTinhLinh(selectedTile, tile);

            NotifyToolActionCompletedForSwap(selectedTile, tile);
            selectedTile = null;
            return;
        }

        if (selectedTile == tile) 
        {
            ResetTinhLinhScale(selectedTile);
            selectedTile = null;
        }
    }

    private static void SwapTinhLinh(Tile tileA, Tile tileB)
    {

        GameObject tinhLinhA = tileA.GetTinhLinhObject();
        GameObject tinhLinhB = tileB.GetTinhLinhObject();
        string idA = tileA.GetTinhLinhID();
        string idB = tileB.GetTinhLinhID();
        Turret turretA = tileA.GetTurret();
        Turret turretB = tileB.GetTurret();

        tileA.SetTinhLinh(tinhLinhB, idB, turretB);
        tileB.SetTinhLinh(tinhLinhA, idA, turretA);
    }

    private static void ResetTinhLinhScale(Tile tile)
    {
        GameObject tinhLinh = tile.GetTinhLinhObject();
        if (tinhLinh != null)
        {
            tinhLinh.transform.localScale = originalScale;
        }
    }
}

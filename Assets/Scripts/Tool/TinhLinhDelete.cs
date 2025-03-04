using UnityEngine;

public class TinhLinhDelete : Tool
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

        if (selectedTile == tile)
        {
            RemoveTinhLinh(selectedTile);
            selectedTile = null;
            return;
        }

        if (selectedTile != null && tile.HasTinhLinh() && selectedTile != tile)
        {
            ResetTinhLinhScale(selectedTile);

            NotifyToolActionCompleted(selectedTile, tile);
            selectedTile = null;
            return;
        }
    }


    private static void RemoveTinhLinh(Tile tile)
    {
        GameObject tinhLinh = tile.GetTinhLinhObject();
        if (tinhLinh != null)
        {
            GameObject.Destroy(tinhLinh); 
        }

        tile.ClearTile(); 
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

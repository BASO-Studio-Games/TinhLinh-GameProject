using UnityEngine;

public class TinhLinhMover : Tool
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

            Debug.Log("Đã chọn tinh linh. Hãy chọn ô trống để di chuyển.");

            // Gọi sự kiện khi chọn tinh linh
            NotifyToolActionCompleted(selectedTile, null);

            return;
        }

        if (selectedTile != null && !tile.HasTinhLinh())
        {
            ResetTinhLinhScale(selectedTile);
            MoveTinhLinh(selectedTile, tile);

            // Gọi sự kiện khi di chuyển tinh linh thành công
            NotifyToolActionCompleted(selectedTile, tile);

            selectedTile = null;
            return;
        }

        if (selectedTile == tile)
        {
            ResetTinhLinhScale(selectedTile);
            selectedTile = null;
            Debug.Log("Hủy chọn tinh linh.");
        }
    }

    private static void MoveTinhLinh(Tile fromTile, Tile toTile)
    {
        Debug.Log($"Di chuyển tinh linh từ {fromTile.name} đến {toTile.name}");

        GameObject tinhLinh = fromTile.GetTinhLinhObject();
        if (tinhLinh == null) return;

        toTile.SetTinhLinh(tinhLinh, fromTile.GetTinhLinhID(), fromTile.GetTurret());
        fromTile.ClearTile();
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

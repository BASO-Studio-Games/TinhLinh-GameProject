using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract int GetMaxTaps();
    public abstract void Activate(Tile selectedTile);

    // Sự kiện có thêm `isDelete` và `isSwap`
    public static event Action<Tool, Tile, Tile, bool, bool> OnToolActionCompleted;

    protected void NotifyToolActionCompleted(Tile fromTile, Tile toTile)
    {
        OnToolActionCompleted?.Invoke(this, fromTile, toTile, false, false);
    }

    protected void NotifyToolActionCompletedForDelete(Tile fromTile)
    {
        OnToolActionCompleted?.Invoke(this, fromTile, null, true, false);
    }

    protected void NotifyToolActionCompletedForSwap(Tile fromTile, Tile toTile)
    {
        OnToolActionCompleted?.Invoke(this, fromTile, toTile, false, true);
    }
}

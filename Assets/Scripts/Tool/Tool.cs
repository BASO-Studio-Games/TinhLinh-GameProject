using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract int GetMaxTaps();
    public abstract void Activate(Tile selectedTile);

    // Sự kiện chung cho tất cả các Tool
    public static event Action<Tool, Tile, Tile> OnToolActionCompleted;

    protected void NotifyToolActionCompleted(Tile fromTile, Tile toTile)
    {
        OnToolActionCompleted?.Invoke(this, fromTile, toTile);
    }
}

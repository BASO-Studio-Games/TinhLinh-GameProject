using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RollItem
{
    public string name;
    public float probability;
    public GameObject prefab;

    public RollItem(string name, float probability, GameObject prefab)
    {
        this.name = name;
        this.probability = probability;
        this.prefab = prefab;
    }

    public string GetIdTinhLinh(){
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.idTinhLinh : "";
    }

    public int GetCost()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.cost : 0; // Nếu không có ItemData, giá = 0
    }

    public bool GetIsTool()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.isTool : false;
    }
}

[System.Serializable]
public class RollData
{
    public List<RollItem> rollItems; // Danh sách tinh linh & tool kèm tỉ lệ
}
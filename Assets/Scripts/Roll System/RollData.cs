using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
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

    public string GetInformation(){
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.information : "";
    }


    public bool GetIsTool()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.isTool : false;
    }

    public Sprite GetSprite()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.sprite : null;
    }
    
    public Sprite GetLevelSprite()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.levelSprite : null;
    }

    public Sprite GetClassSprite()
    {
        ItemData data = prefab.GetComponent<ItemData>();
        return data != null ? data.classSprite : null;
    }
}

[System.Serializable]
public class RollData
{
    public List<RollItem> rollItems; // Danh sách tinh linh & tool kèm tỉ lệ
}
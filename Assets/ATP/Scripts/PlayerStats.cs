using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "UDEV/TDS/Create Player Stats")]
public class PlayerStats : ActorStats
{

    public float CurrentHp { get; internal set; }
    public float CurrentMana { get; internal set; }


    public override void Load()
    {
        if (!string.IsNullOrEmpty(Prefs.playerData))
        {
            JsonUtility.FromJsonOverwrite(Prefs.playerData, this);
        }
    }

    public override void Save()
    {
        Prefs.playerData = JsonUtility.ToJson(this);
    }


     
    //public void TakeDamage(float damage)
    //{
    //    // if (damage <= 0) return;
    //    // Debug.Log("dung roi ne");
    //    // CurHp -= damage;
    //    // CurHp = Mathf.Clamp(CurHp, 0, PlayerStats.hp);

    //    // OnTakeDamage?.Invoke();

    //    // if (CurHp <= 0)
    //    // {
    //    //     GameManager.Ins.GameoverChecking(OnLostLifeDelegate, OnDeadDelegate);
    //    // }
    //}
}

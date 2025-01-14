using System;
using UnityEngine;

public class ActorStats : Stats
{
    [Header("Base Stats:")]
    public float hp;
    public override bool IsMaxLevel()
    {
        return false;
    }

    public override void Load()
    {

    }

    public override void Save()
    {

    }

    public override void Upgrade(Action OnSuccess = null, Action OnFailed = null)
    {

    }

    
}

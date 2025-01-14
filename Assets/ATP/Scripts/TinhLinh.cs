using System;
using UnityEngine;

public class TinhLinh : Actor
{
    private PlayerStats m_playerStats;

    private bool isDestroyed = false;

    public PlayerStats PlayerStats { get => m_playerStats;private set => m_playerStats = value; }

    public override void Init()
    {
        LoadStats();
    }

    private void LoadStats()
    {
        if (statsData == null) return;

        m_playerStats = (PlayerStats)statsData;
        m_playerStats.Load();
        CurHp = m_playerStats.hp;
    }

    private void Start()
    {
        Init();
    }

    public override void TakeDamage(float damage)
    {
        CurHp -= damage;
        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();
        }


        OnTakeDamage?.Invoke(CurHp);
    }

    protected override void Die()
    {
        Debug.Log("die");
        m_rb.linearVelocity = Vector3.zero;

        OnDead?.Invoke();
        EnemySpawner.onEnemyDestroy.Invoke();
        isDestroyed = true;
        Destroy(gameObject, 1.5f);
    }

}

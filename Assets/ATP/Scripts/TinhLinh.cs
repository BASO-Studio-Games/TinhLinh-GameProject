using System;
using UnityEngine;
using UnityEngine.UI;

public class TinhLinh : Actor
{
    private PlayerStats m_playerStats;
    [SerializeField] protected float maxHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject hpBarObject;

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
        maxHp = m_playerStats.hp;
        CurHp = maxHp;
        UpdateHpBar();
        SetHpBarVisible(false);
    }

    private void Start()
    {
        Init();

        if (hpBarObject != null)
        {
            Canvas hpBarCanvas = hpBarObject.GetComponent<Canvas>();
            if (hpBarCanvas != null)
            {
                hpBarCanvas.overrideSorting = true;
                hpBarCanvas.sortingLayerName = "Default";
                hpBarCanvas.sortingOrder = 3;
            }
            else
            {
                Debug.LogError("hpBarCanvas is null");
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        CurHp -= damage;
        CurHp = Mathf.Max(CurHp, 0);
        UpdateHpBar();
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

        isDestroyed = true;
        Destroy(gameObject, 1.5f);
    }
    protected void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = CurHp / maxHp;
        }
    }

    public void SetHpBarVisible(bool isVisible)
    {
        if (hpBar != null)
        {
            hpBarObject.SetActive(isVisible);
        }
    }

}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TinhLinh : Actor
{
    private PlayerStats m_playerStats;
    [SerializeField] protected float maxHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject hpBarObject;
    private Animator hpBarAnimator;

    public bool IsFrozen { get; private set; } = false;
    public bool IsImmuneToFreeze { get; set; } = false;


    private Tile currentTile;

    private bool isDestroyed = false;

    public PlayerStats PlayerStats { get => m_playerStats;private set => m_playerStats = value; }


    public override void Init()
    {
        LoadStats();
    }

    private void Awake()
    {
        if (hpBarObject != null)
        {
            hpBarAnimator = hpBarObject.GetComponent<Animator>();
        }
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

    // protected override void Die()
    // {
    //     Debug.Log("die");
    //     m_rb.linearVelocity = Vector3.zero;

    //     if (currentTile != null)
    //     {
    //         currentTile.ClearTile();
    //     }

    //     isDestroyed = true;
    //     Destroy(gameObject, 1.5f);
    // }
    protected override void Die()
    {
        Debug.Log("Tinh linh chết");

        ClearTile(); // Xóa ID tinh linh khỏi ô

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
        if (hpBarAnimator != null)
        {
            hpBarAnimator.SetBool("isClose", !isVisible);
        }

        if (!isVisible)
        {
            StartCoroutine(HideHpBarAfterAnimation());
        }
        else
        {
            if (hpBarObject != null)
            {
                hpBarObject.SetActive(true);
            }
        }
    }

    private IEnumerator HideHpBarAfterAnimation()
    {
        float animationTime = hpBarAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationTime);

        if (hpBarObject != null)
        {
            hpBarObject.SetActive(false);
        }
    }

    public void SetTile(Tile tile)
    {
        currentTile = tile;
    }

    public void ClearTile()
    {
        if (currentTile != null)
        {
            currentTile.ClearTile();
        }
    }

    public void Freeze()
    {
        if (IsImmuneToFreeze) return; 

        IsFrozen = true;
        Debug.Log($"{gameObject.name} bị đóng băng!");
    }

    public void Unfreeze()
    {
        if (IsFrozen)
        {
            IsFrozen = false;
            Debug.Log($"{gameObject.name} được giải băng!");
        }
    }

}

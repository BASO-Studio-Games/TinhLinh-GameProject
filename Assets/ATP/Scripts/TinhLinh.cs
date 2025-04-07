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
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;


    public bool IsFrozen { get; private set; } = false;
    public bool IsImmuneToFreeze { get; set; } = false;

    private Tile currentTile;
    private bool isDestroyed = false;
    public Fridge freezingFridge; // Fridge đang đóng băng tinh linh này

    public PlayerStats PlayerStats { get => m_playerStats; private set => m_playerStats = value; }

    public override void Init()
    {
        LoadStats();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }

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

    public void Freeze(Fridge fridge)
    {
        if (IsImmuneToFreeze) return;

        // Nếu đã bị đóng băng bởi một Fridge khác, không đóng băng lại
        if (IsFrozen && freezingFridge != fridge)
        {
            Debug.Log($"{gameObject.name} đã bị đóng băng bởi {freezingFridge.name}, bỏ qua.");
            return;
        }

        if (spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
        else
        {
            Debug.LogError("erro");
        }

        // Sau đó mới đóng băng
        IsFrozen = true;
        freezingFridge = fridge;
        Debug.Log($"{gameObject.name} bị đóng băng bởi {fridge.name}!");
    }


    public void Unfreeze()
    {
        if (IsFrozen)
        {
            IsFrozen = false;
            freezingFridge = null; // Cho phép bị đóng băng lại
            Debug.Log($"{gameObject.name} được giải băng!");
        }
    }
}

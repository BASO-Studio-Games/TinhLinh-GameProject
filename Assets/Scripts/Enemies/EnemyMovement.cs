using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMovement : Actor
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform arrowIndicator;
    [SerializeField] private Animator animator;

    [Header("Tài nguyên nhận được")]
    [SerializeField] private int currencyTakeDamage;
    [SerializeField] private int energyTakeDamage;
    [SerializeField] private int currencyDie;
    [SerializeField] private int energyDie;

    [Header("Attributes")]
    [SerializeField] public float moveSpeed;
    [SerializeField] private int currencyWorth = 50;
    [SerializeField] protected float maxHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject hpBarObject;
    private bool hasTriggeredDestroy = false;
    [SerializeField] private bool isEvolved = false;


    [SerializeField] private bool isStunned = false;
    private Transform target;
    private int pathIndex = 0;
    public float baseSpeed;
    public bool isDestroyed = false;
    public bool isMoving = true;
    public bool isAttack = false;

    private EnemyStats m_enemyStats;
    public EnemyStats EnemyStats { get => m_enemyStats; private set => m_enemyStats = value; }

    public override void Init()
    {
        LoadStats();
    }

    private void LoadStats()
    {
        if (statsData == null) return;

        m_enemyStats = (EnemyStats)statsData;
        m_enemyStats.Load();
        maxHp = m_enemyStats.hp;
        CurHp = maxHp;
        UpdateHpBar();
        SetHpBarVisible(false);
        moveSpeed = m_enemyStats.speedEnemy;
    }

    private void Start()
    {
        Init();
        baseSpeed = moveSpeed;

        if (!isEvolved)
        {
            target = LevelManager.main.path[pathIndex];
        }

        if (arrowIndicator == null)
        {
            Debug.Log("Arrow Indicator is not assigned!");
        }
    }

    private void Update()
    {
        CheckPoint();
        UpdateArrowDirection();
    }

    private void FixedUpdate()
    {
        Move();
        Debug.Log(moveSpeed);
    }

    private void Move()
    {
        if (isMoving)
        {
            Vector2 direction = ((Vector2)transform.position - (Vector2)target.position).normalized;
            Vector2 newPosition = Vector2.MoveTowards(rb.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);


            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1); 
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); 
            }
        }
    }


    private void CheckPoint()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.01f)
        {
            pathIndex++;

            if (pathIndex == LevelManager.main.path.Length)
            {
                LevelManager.main.UpdateStatusLevel(false);
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private void UpdateArrowDirection()
    {
        if (arrowIndicator != null && target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            arrowIndicator.rotation = Quaternion.Euler(0, 0, angle);
        }
    }


    public void UpdateSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        isMoving = !stunned;
        GetComponent<EnemyMovement>().enabled = !stunned;
    }

    public override void TakeDamage(float damage)
    {
        CurHp -= damage;
        CurHp = MathF.Max(CurHp, 0);
        UpdateHpBar();
        LevelManager.main.IncreaseCurrency(currencyTakeDamage);
        LevelManager.main.IncreaseEnergy(energyTakeDamage);

        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();
        }

        OnTakeDamage?.Invoke(CurHp);
    }

    protected override void Die()
    {
        if (hasTriggeredDestroy) return;

        hasTriggeredDestroy = true;

        LevelManager.main.IncreaseCurrency(currencyDie);
        LevelManager.main.IncreaseEnergy(energyDie);

        isDestroyed = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("isDie", true);
        }

        StartCoroutine(EvolveEnemyAfterDelay(0.5f)); 

        GetComponent<EnemyMovement>().enabled = false;

        OnDead?.Invoke();
        EnemySpawner.onEnemyDestroy.Invoke();

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animationLength);
    }

    private IEnumerator EvolveEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        FanEnemy fanEnemyComponent = GetComponent<FanEnemy>();
        VacuumCleanerEnemy vacuumCleanerEnemy = GetComponent<VacuumCleanerEnemy>();
        if (fanEnemyComponent != null)
        {
            GameObject evolvedEnemy = Instantiate(EvolutionList.main.GetEvolvedFanPrefab(), transform.position, Quaternion.identity);

            // Đảm bảo đối tượng mới được gán vào cùng parent với đối tượng cũ
            if (transform.parent != null)
            {
                evolvedEnemy.transform.SetParent(transform.parent, false);
            }

            EnemyMovement evolvedMovement = evolvedEnemy.GetComponent<EnemyMovement>();
            if (evolvedMovement != null)
            {
                evolvedMovement.isEvolved = true;
                evolvedMovement.pathIndex = this.pathIndex;
                evolvedMovement.target = LevelManager.main.path[this.pathIndex];
            }
        }

        if (vacuumCleanerEnemy != null)
        {
            GameObject evolvedEnemy = Instantiate(EvolutionList.main.GetEvolvedFanPrefab(), transform.position, Quaternion.identity);
            evolvedEnemy.GetComponent<Collider2D>().enabled = false; 
            if (transform.parent != null)
            {
                evolvedEnemy.transform.SetParent(transform.parent, false);
            }
            StartCoroutine(EnableColliderAfterDelay(evolvedEnemy, 0.1f));

            EnemyMovement evolvedMovement = evolvedEnemy.GetComponent<EnemyMovement>();
            if (evolvedMovement != null)
            {
                evolvedMovement.isEvolved = true;
                evolvedMovement.pathIndex = this.pathIndex;
                evolvedMovement.target = LevelManager.main.path[this.pathIndex];
            }
        }
    }

    private IEnumerator EnableColliderAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.GetComponent<Collider2D>().enabled = true; 
    }

    public void TriggerDie()
    {
        isDestroyed = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("isDie", true);
        }

        StartCoroutine(EvolveEnemyAfterDelay(0.5f));

        GetComponent<EnemyMovement>().enabled = false;

        OnDead?.Invoke();
        EnemySpawner.onEnemyDestroy.Invoke();

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animationLength);
    }




    protected void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = CurHp / maxHp;
        }
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject, 0.5f);
    }

    public void SetHpBarVisible(bool isVisible)
    {
        if (hpBar != null)
        {
            hpBarObject.SetActive(isVisible);
        }
    }
}

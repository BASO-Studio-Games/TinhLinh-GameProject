using System;
using UnityEngine;

public class EnemyMovement : Actor
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform arrowIndicator; 

    [Header("Attributes")]
    [SerializeField] public float moveSpeed;
    [SerializeField] private int currencyWorth = 50;

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
        CurHp = m_enemyStats.hp;
        moveSpeed = m_enemyStats.speedEnemy;
    }

    private void Start()
    {
        Init();
        baseSpeed = moveSpeed;
        target = LevelManager.main.path[pathIndex];

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
    }

    private void Move()
    {
        if (isMoving)
        {
            Vector2 direction = Vector2.MoveTowards(rb.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(direction);
        }
    }

    private void CheckPoint()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.01f)
        {
            pathIndex++;

            if (pathIndex == LevelManager.main.path.Length)
            {
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

            transform.rotation = Quaternion.Euler(0, 0, angle);
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
        LevelManager.main.IncreaseCurrency(currencyWorth);
        isDestroyed = true;
        Destroy(gameObject, 0.5f);
    }
}

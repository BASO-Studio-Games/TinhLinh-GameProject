using System.Collections;
using UnityEngine;

public class AxAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Attributes")]
    [SerializeField] private Vector2[] squareOffsets = new Vector2[8];
    [SerializeField] private float squareSize = 0.7f;
    [SerializeField] private int damage = 50;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackDuration = 1f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private bool isRotating = false;
    private bool isAttacking = false;
    private float attackCooldown = 0f;

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector2(squareSize * 3, squareSize * 3), 0f, enemyMask);

        if (enemiesInRange.Length > 0)
        {
            Transform nearestEnemy = FindNearestEnemy(enemiesInRange);
            if (nearestEnemy != null)
            {
                RotateTowardsEnemy(nearestEnemy);
            }

            if (!isAttacking)
            {
                StartCoroutine(ContinuousAttack());
            }
        }
    }

    private Transform FindNearestEnemy(Collider2D[] enemies)
    {
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    private void RotateTowardsEnemy(Transform enemy)
    {
        Vector2 direction = (enemy.position - transform.position).normalized;

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator ContinuousAttack()
    {
        isAttacking = true;
        animator.SetBool("isAttack", true);

        while (true)
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector2(squareSize * 3, squareSize * 3), 0f, enemyMask);

            if (enemiesInRange.Length <= 0)
            {
                animator.SetBool("isAttack", false);
                break;
            }

            if (attackCooldown <= 0f && !isRotating)
            {
                yield return StartCoroutine(PerformAxAttack());
                attackCooldown = attackInterval;
            }

            yield return null;
        }

        isAttacking = false;
    }

    private IEnumerator PerformAxAttack()
    {
        isRotating = true;

        foreach (var offset in squareOffsets)
        {
            Vector2 targetPosition = (Vector2)transform.position + offset;

            DealDamageAtSquare(targetPosition);

            yield return new WaitForSeconds(attackDuration / 7);
        }

        isRotating = false;
    }

    private void DealDamageAtSquare(Vector2 squareCenter)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareSize, squareSize), 0f, enemyMask);

        bool hasHitEnemy = false;

        foreach (Collider2D hit in hits)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasHitEnemy = true;
            }
        }

        if (hasHitEnemy)
        {
            AudioManager.Instance.PlaySFX("TinhLinhHit");
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(squareCenter, new Vector3(squareSize, squareSize, 0f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 center = (Vector2)transform.position;
        Gizmos.DrawWireCube(center, new Vector3(squareSize * 3, squareSize * 3, 0f));
    }
}

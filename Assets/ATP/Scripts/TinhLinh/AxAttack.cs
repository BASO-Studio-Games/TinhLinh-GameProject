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
    private Vector2? currentTargetSquare = null;
    private float attackCooldown = 0f;
    private Collider2D currentEnemy = null;

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyMask) != 0)
        {
            currentEnemy = collision.collider;

            if (!isAttacking)
            {
                StartCoroutine(ContinuousAttack());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == currentEnemy)
        {
            currentEnemy = null;
            animator.SetBool("isAttack", false);
        }
    }

    private IEnumerator ContinuousAttack()
    {
        isAttacking = true;

        while (currentEnemy != null)
        {
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

        if (animator != null)
        {
            animator.SetBool("isAttack", true);
        }

        foreach (var offset in squareOffsets)
        {
            Vector2 targetPosition = (Vector2)transform.position + offset;
            currentTargetSquare = targetPosition;

            DealDamageAtSquare(targetPosition);

            yield return new WaitForSeconds(attackDuration / 7);
        }

        currentTargetSquare = null;
        isRotating = false;
    }

    private void DealDamageAtSquare(Vector2 squareCenter)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareSize, squareSize), 0f, enemyMask);

        foreach (Collider2D hit in hits)
        {
            EnemyMovement enemy = hit.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (enemy == null)
                {
                    currentEnemy = null;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;

            Gizmos.color = (currentTargetSquare.HasValue && currentTargetSquare.Value == squareCenter) ? Color.yellow : Color.red;
            Gizmos.DrawWireCube(squareCenter, new Vector3(squareSize, squareSize, 0f));
        }
    }
}

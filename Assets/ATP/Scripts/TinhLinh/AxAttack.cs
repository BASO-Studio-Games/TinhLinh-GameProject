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
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float attackDuration = 1f;
    [SerializeField] private float delayPerSquare = 0.2f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    public bool isRotating = false;
    private Vector2? currentTargetSquare = null;
    private float attackCooldown = 0f;

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (!isRotating && attackCooldown <= 0f && DetectEnemiesInArea())
        {
            StartCoroutine(PerformAxAttack());
        }
    }

    private bool DetectEnemiesInArea()
    {
        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;

            Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareSize, squareSize), 0f, enemyMask);

            if (hits.Length > 0)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator PerformAxAttack()
    {
        isRotating = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
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

        attackCooldown = attackInterval;
    }

    private void DealDamageAtSquare(Vector2 squareCenter)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareSize, squareSize), 0f, enemyMask);

        foreach (Collider2D hit in hits)
        {
            EnemyMovement health = hit.GetComponent<EnemyMovement>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;

            if (currentTargetSquare.HasValue && currentTargetSquare.Value == squareCenter)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireCube(squareCenter, new Vector3(squareSize, squareSize, 0f));
        }
    }
}

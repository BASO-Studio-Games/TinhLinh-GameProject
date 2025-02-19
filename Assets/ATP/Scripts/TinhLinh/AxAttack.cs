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
    private Collider2D[] enemiesInRange;

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Kiểm tra kẻ địch có nằm trong ô tấn công không
        enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector2(squareSize * 3, squareSize * 3), 0f, enemyMask);

        // Debug.Log(enemiesInRange.Length);
        if (enemiesInRange.Length > 0 && !isAttacking)
        {
            StartCoroutine(ContinuousAttack());
        }
    }

    private IEnumerator ContinuousAttack()
    {
        isAttacking = true;

        while (enemiesInRange.Length > 0)
        {
            if (attackCooldown <= 0f && !isRotating)
            {
                yield return StartCoroutine(PerformAxAttack());
                attackCooldown = attackInterval;
            }

            // Cập nhật danh sách kẻ địch trong vùng tấn công
            enemiesInRange = Physics2D.OverlapBoxAll(transform.position, new Vector2(squareSize * 2, squareSize * 2), 0f, enemyMask);
            
            if (enemiesInRange.Length <= 0)
            {
                animator.SetBool("isAttack", false);
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

            DealDamageAtSquare(targetPosition);

            yield return new WaitForSeconds(attackDuration / 7);
        }

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
            }
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

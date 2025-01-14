using System.Collections;
using UnityEngine;

public class AttackTinhLinh : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask playerMask;

    [Header("Attributes")]
    [SerializeField] private Vector2[] frostOffsets;
    [SerializeField] private float boxSize = 1f;
    [SerializeField] private float attackCooldown = 2f; 
    [SerializeField] private float attackDuration = 1f; 
    [SerializeField] private int damage = 20;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    private void Update()
    {
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            CheckForPlayersInGridAndAttack();
        }
    }

    private void CheckForPlayersInGridAndAttack()
    {
        foreach (var offset in frostOffsets)
        {
            Vector2 boxPosition = (Vector2)transform.position + offset;
            Collider2D hit = Physics2D.OverlapBox(boxPosition, new Vector2(boxSize, boxSize), 0f, playerMask);

            if (hit != null)
            {
                StartCoroutine(PerformAttack(hit, boxPosition));
                return;
            }
        }
    }

    private IEnumerator PerformAttack(Collider2D target, Vector2 boxPosition)
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Debug.Log("Tinh Linh Attack");

        yield return new WaitForSeconds(attackDuration); 

        if (target != null)
        {
            EnemyMovement playerHealth = target.GetComponent<EnemyMovement>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(attackCooldown - attackDuration);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        foreach (var offset in frostOffsets)
        {
            Vector2 boxCenter = (Vector2)transform.position + offset;
            Gizmos.DrawWireCube(boxCenter, new Vector3(boxSize, boxSize, 0f));
        }
    }
}

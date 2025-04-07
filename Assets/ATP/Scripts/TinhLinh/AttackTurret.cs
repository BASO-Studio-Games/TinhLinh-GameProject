using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject bloodPrefabs;
    private Vector2 lastHitPosition = Vector2.negativeInfinity;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private bool isAttacking = false;
    private List<Collider2D> enemiesInRange = new List<Collider2D>();

    private Collider2D hitTarget;

    private void Update()
    {
        if (enemiesInRange.Count > 0 && !isAttacking)
        {
            CheckForPlayersInGridAndAttack();
        }
    }

    private void CheckForPlayersInGridAndAttack()
    {
        bool hasTarget = false;

        foreach (var offset in frostOffsets)
        {
            Vector2 boxPosition = (Vector2)transform.position + offset;
            Collider2D hit = Physics2D.OverlapBox(boxPosition, new Vector2(boxSize, boxSize), 0f, playerMask);

            if (hit != null && enemiesInRange.Contains(hit))
            {
                Debug.Log("phat hien muc tieu");
                hitTarget = hit;
                hasTarget = true;

                Vector2 directionToTarget = (hit.transform.position - transform.position).normalized;

                RotateTowardsTarget(directionToTarget);

                break;
            }
            else
            {
                Debug.Log("khong co muc tieu");
            }
        }

        animator.SetBool("isAttack", hasTarget);
    }

    private void RotateTowardsTarget(Vector2 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }




    public void Attack()
    {
        isAttacking = true;

        if (hitTarget != null)
        {
            EnemyMovement playerHealth = hitTarget.GetComponent<EnemyMovement>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerMask) != 0)
        {
            Debug.Log("da va cham");
            enemiesInRange.Add(collision.collider);
            lastHitPosition = collision.transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerMask) != 0)
        {
            enemiesInRange.Remove(collision.collider);
        }

        if (enemiesInRange.Count == 0)
        {
            animator.SetBool("isAttack", false);
        }
    }

    public Vector2 GetLastHitPosition()
    {
        return lastHitPosition;
    }

    public void AttackEffects()
    {
        if (GetLastHitPosition() == Vector2.negativeInfinity) return;
        GameObject blood = Instantiate(bloodPrefabs, GetLastHitPosition(), Quaternion.identity);
        Destroy(blood, 1f);
    }

    public void AttackSound()
    {
        AudioManager.Instance.PlaySFX("TinhLinhHit");
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

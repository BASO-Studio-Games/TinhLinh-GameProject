using System.Collections;
using UnityEngine;
using UnityEditor;

public class BommerTurret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Attributes")]
    [SerializeField] private float squareRange = 5f;
    [SerializeField] private int damage = 50;
    [SerializeField] private float delayBeforeExplosion = 0.8f;

    [Header("Visuals")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Animator animator;

    [Header("Offsets for Squares")]
    [SerializeField] private Vector2[] squareOffsets = new Vector2[4];

    public bool isPreparingToExplode = false;

    private void Update()
    {
        if (!isPreparingToExplode)
        {
            DetectEnemiesAndPrepareToExplode();
        }
    }

    private void DetectEnemiesAndPrepareToExplode()
    {
        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareRange, squareRange), 0f, enemyMask);

            if (hits.Length > 0)
            {
                isPreparingToExplode = true;
                StartCoroutine(PrepareToExplode());
                break;
            }
        }
    }

    private IEnumerator PrepareToExplode()
    {
        if (animator != null)
        {
            animator.SetTrigger("isAttack");
        }

        float elapsedTime = 0f;

        while (elapsedTime < delayBeforeExplosion)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(squareCenter, new Vector2(squareRange, squareRange), 0f, enemyMask);

            foreach (Collider2D hit in hits)
            {
                EnemyMovement health = hit.GetComponent<EnemyMovement>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (var offset in squareOffsets)
        {
            Vector2 squareCenter = (Vector2)transform.position + offset;
            Gizmos.DrawWireCube(squareCenter, new Vector3(squareRange, squareRange, 0f));
        }
    }
}

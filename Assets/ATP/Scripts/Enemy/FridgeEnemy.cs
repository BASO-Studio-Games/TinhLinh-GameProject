using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FridgeEnemy : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int health = 20;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int damage = 5;

    [Header("Frost Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector2[] frostOffsets;
    [SerializeField] private float boxSize = 3f; 
    [SerializeField] private float freezeDuration = 3f; 
    [SerializeField] private float detectionInterval = 0.1f; 

    private EnemyMovement enemyMovement;
    private TinhLinh targetTinhLinh;
    private Dictionary<Collider2D, float> frostTimers = new Dictionary<Collider2D, float>();
    private HashSet<Collider2D> frozenTargets = new HashSet<Collider2D>(); 

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();

        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement script not found on this GameObject.");
        }

        StartCoroutine(DetectEnemiesInFrostBoxesRoutine());
    }

    private IEnumerator DetectEnemiesInFrostBoxesRoutine()
    {
        while (true)
        {
            DetectEnemiesInFrostBoxes();
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    private void DetectEnemiesInFrostBoxes()
    {
        HashSet<Collider2D> currentFrameTargets = new HashSet<Collider2D>();

        foreach (var offset in frostOffsets)
        {
            Vector2 boxCenter = (Vector2)transform.position + offset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, new Vector2(boxSize, boxSize), 0f, enemyLayer);

            foreach (Collider2D hit in hits)
            {
                currentFrameTargets.Add(hit);

                if (!frostTimers.ContainsKey(hit))
                {
                    frostTimers[hit] = 0f;
                    Debug.Log($"Enemy {hit.name} entered frost box at {Time.time} seconds.");
                }
                else
                {
                    frostTimers[hit] += detectionInterval;

                    if (frostTimers[hit] >= freezeDuration && !frozenTargets.Contains(hit))
                    {
                        FreezeTarget(hit); 
                        frozenTargets.Add(hit);
                    }
                }
            }
        }

        List<Collider2D> toRemove = new List<Collider2D>();

        foreach (var target in frostTimers.Keys)
        {
            if (!currentFrameTargets.Contains(target))
            {
                toRemove.Add(target);
            }
        }

        foreach (var target in toRemove)
        {
            frostTimers.Remove(target);
            Debug.Log($"Enemy {target.name} exited frost box at {Time.time} seconds.");
            if (frozenTargets.Contains(target))
            {
                UnfreezeTarget(target); 
                frozenTargets.Remove(target);
            }
        }
    }

    private void FreezeTarget(Collider2D target)
    {
        Debug.Log($"Target {target.name} is frozen!");
        EnemyMovement targetMovement = target.GetComponent<EnemyMovement>();
        if (targetMovement != null)
        {
            targetMovement.SetStunned(true);
        }
    }

    private void UnfreezeTarget(Collider2D target)
    {
        Debug.Log($"Target {target.name} is unfrozen!");
        EnemyMovement targetMovement = target.GetComponent<EnemyMovement>();
        if (targetMovement != null)
        {
            targetMovement.SetStunned(false);
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TinhLinh tinhlinh = collision.gameObject.GetComponent<TinhLinh>();
        if (tinhlinh != null)
        {
            enemyMovement.isAttack = true;
            enemyMovement.isMoving = false;
            targetTinhLinh = tinhlinh;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TinhLinh tinhlinh = collision.gameObject.GetComponent<TinhLinh>();
        if (tinhlinh != null && tinhlinh == targetTinhLinh)
        {
            enemyMovement.isAttack = false;
            enemyMovement.isMoving = true;
            targetTinhLinh = null;
        }
    }
}

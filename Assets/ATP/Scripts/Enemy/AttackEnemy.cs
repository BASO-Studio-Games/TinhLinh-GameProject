using UnityEngine;
using System.Collections;

public class AttackEnemy : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float attackInterval = 2f; 
    [SerializeField] private float attackDuration = 1f; 
    [SerializeField] private int enemyDamage = 1;

    private EnemyMovement enemyMovement;
    private TinhLinh targetTinhLinh;
    private float attackCooldown; 
    private bool isPerformingAttack = false; 

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();

        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement script not found on this GameObject.");
        }

        attackCooldown = 0f; 
    }

    private void Update()
    {
        if (enemyMovement.isAttack && targetTinhLinh != null && !isPerformingAttack)
        {
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0f)
            {
                StartCoroutine(PerformAttack(targetTinhLinh));
                attackCooldown = attackInterval; 
            }
        }
    }

    private IEnumerator PerformAttack(TinhLinh tinhlinh)
    {
        isPerformingAttack = true;

        Debug.Log("Enemy begins attack animation!");

        yield return new WaitForSeconds(attackDuration);

        if (tinhlinh == null)
        {
            Debug.LogWarning("Target is destroyed or no longer exists.");
            isPerformingAttack = false;
            yield break; 
        }

        Debug.Log("Enemy Attack!");
        tinhlinh.TakeDamage(enemyDamage);

        isPerformingAttack = false;
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

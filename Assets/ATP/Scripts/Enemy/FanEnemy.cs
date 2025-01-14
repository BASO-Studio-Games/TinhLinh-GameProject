using UnityEngine;

public class FanEnemy : MonoBehaviour
{
    [SerializeField] private int baseHealth = 10;
    [SerializeField] private int propellerHealth = 5;

    [SerializeField] private float speedBoost = 2f;
    [SerializeField] private int bonusDamage = 2; 
    [SerializeField] private int baseDamage = 1; 

    [SerializeField] private float attackInterval = 2f;

    private TinhLinh targetTinhLinh;
    private EnemyMovement enemyMovement;
    private int currentHealth;
    private bool isPropellerActive = true; 
    private float attackCooldown;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement script not found on this GameObject.");
        }

        currentHealth = baseHealth + propellerHealth;
        EnablePropeller();
    }

    private void Update()
    {
        if (isPropellerActive && currentHealth <= baseHealth)
        {
            RemovePropeller();
        }

        if (enemyMovement.isAttack && targetTinhLinh != null)
        {
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0f)
            {
                Attack(targetTinhLinh);
                attackCooldown = attackInterval; 
            }
        }
    }

    private void Attack(TinhLinh tinhlinh)
    {
        int currentDamage = isPropellerActive ? baseDamage + bonusDamage : baseDamage; 
        Debug.Log($"Attack with {currentDamage} damage!");
        tinhlinh.TakeDamage(currentDamage);
    }

    private void EnablePropeller()
    {
        isPropellerActive = true;

        if (enemyMovement != null)
        {
            enemyMovement.moveSpeed *= speedBoost;
        }
        Debug.Log("Propeller enabled! Increased speed and damage.");
    }

    private void RemovePropeller()
    {
        isPropellerActive = false;

        if (enemyMovement != null)
        {
            enemyMovement.moveSpeed /= speedBoost;
        }
        Debug.Log("Propeller destroyed! Enemy reverted to basic state.");
    }

    //private void Die()
    //{
    //    Debug.Log("Enemy died!");
    //    Destroy(gameObject);
    //}

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

    //public void TakeDamage(int damage)
    //{
    //    currentHealth -= damage; 
    //    Debug.Log($"Enemy took {damage} damage. Current health: {currentHealth}");

    //    if (currentHealth <= 0)
    //    {
    //        Die();
    //    }
    //}
}

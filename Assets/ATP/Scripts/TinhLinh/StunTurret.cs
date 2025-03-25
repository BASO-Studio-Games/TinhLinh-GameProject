using UnityEngine;

public class StunTurret : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int damage = 1000;
    [SerializeField] private GameObject explosionEffectPrefab;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private Collider2D turretCollider;

    private Collider2D currentEnemyCollider;

    private void Awake()
    {
        turretCollider = GetComponent<Collider2D>();
        if (turretCollider == null)
        {
            Debug.LogError("StunTurret: Collider2D not found!");
        }
        else
        {
            turretCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Collider2D enemyCollider = collision.GetComponent<Collider2D>();

            if (enemyCollider != null && enemyCollider.isTrigger)
            {
                return;
            }

            animator.SetBool("isAttack", true);
            currentEnemyCollider = collision;
            CreateExplosionEffect(collision.transform.position);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentEnemyCollider)
        {
            currentEnemyCollider = null;
        }
    }

    public void Attack()
    {
        if (currentEnemyCollider != null)
        {
            EnemyMovement enemy = currentEnemyCollider.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            TinhLinh tinhLinh = gameObject.GetComponent<TinhLinh>();
            if (tinhLinh != null)
            {
                tinhLinh.ClearTile();
            }
            Destroy(gameObject, 1f);
        }
    }

    public void CreateExplosionEffect(Vector2 position)
    {
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 1.5f);
        }
    }
}

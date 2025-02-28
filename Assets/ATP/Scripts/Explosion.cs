using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 500f;
    [SerializeField] private GameObject explosionEffectPrefab; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyMovement health = collision.GetComponent<EnemyMovement>();
            if (health != null)
            {
                health.TakeDamage(damage);

                CreateExplosionEffect(collision.transform.position);
            }
        }
    }

    private void CreateExplosionEffect(Vector2 position)
    {
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 1.5f); 
        }
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}

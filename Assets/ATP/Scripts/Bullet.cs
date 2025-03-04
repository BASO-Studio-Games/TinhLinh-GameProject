using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject explosionEffectPrefab;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Animator animator;

    private bool isEmpowered = false; 

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
                CreateExplosionEffect(collision.transform.position);
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 4.5f);
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

    public void ChangeAnimatorState()
    {
        if (!isEmpowered) 
        {
            isEmpowered = true;

            if (animator != null)
            {
                animator.SetBool("isChange", true);
                Debug.Log("Bullet has been empowered!");
            }
            else
            {
                Debug.LogWarning("Bullet: Animator is missing!");
            }

            bulletDamage *= 2;  
            bulletSpeed *= 1.5f; 
        }
    }
}

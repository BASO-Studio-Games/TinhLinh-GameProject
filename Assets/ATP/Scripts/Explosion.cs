using GooglePlayGames.BasicApi;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 500f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyMovement health = collision.GetComponent<EnemyMovement>();
        if (collision.CompareTag("Enemy"))
        {
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class BulletAutoTarGet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;
    //[SerializeField] private Vector2 direction;

    private Transform targer;

    public void SetTarget(Transform _target)
    {
        targer = _target;
    }

    private void FixedUpdate()
    {
        if (!targer) return;
        Vector2 direction = (targer.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }

    //public void SetDirection(Vector2 newDirection)
    //{
    //    direction = newDirection.normalized;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<EnemyMovement>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}


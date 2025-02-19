using System.Collections;
using UnityEngine;

public class BommerTurret : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject explosionPrefabs;

    public void CreateExplosion()
    {
        if (explosionPrefabs != null)
        {
            Destroy(gameObject);
            Instantiate(explosionPrefabs, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            animator.SetBool("isAttack", true);
        }
    }
}

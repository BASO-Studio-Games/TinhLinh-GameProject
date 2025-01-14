using System.Collections;
using UnityEngine;

public class StunTurret : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int damage = 1000;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float damageDelay = 0.5f;
    [SerializeField] private float destroyDelay = 0.5f;

    [Header("Activation Settings")]
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float inactiveDuration = 3f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private float timer = 0f;
    private bool isActive = true;
    private Collider2D turretCollider;

    private void Awake()
    {
        turretCollider = GetComponent<Collider2D>();
        if (turretCollider == null)
        {
            Debug.LogError("StunTurret: Collider2D not found!");
        }
        SetActiveState(true); // Start as inactive
    }

    private void Update()
    {
        Debug.Log(isActive);
        timer += Time.deltaTime;

        if (isActive && timer >= activeDuration)
        {
            SetActiveState(false);
            timer = 0f; // Reset timer
        }
        else if (!isActive && timer >= inactiveDuration)
        {
            SetActiveState(true);
            timer = 0f; // Reset timer
        }
    }

    private void SetActiveState(bool active)
    {
        isActive = active;

        if (turretCollider != null)
        {
            turretCollider.enabled = active;
        }

        if (animator != null)
        {
            animator.SetBool("IsActive", active);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && ((1 << collision.gameObject.layer) & enemyMask) != 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Explode");
            }
            StartCoroutine(DelayDamage(collision.gameObject));
        }
    }

    private IEnumerator DelayDamage(GameObject enemy)
    {
        yield return new WaitForSeconds(damageDelay);

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            Debug.Log($"StunTurret: Dealing {damage} damage to {enemyMovement.name}");
            enemyMovement.TakeDamage(damage);
        }

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}

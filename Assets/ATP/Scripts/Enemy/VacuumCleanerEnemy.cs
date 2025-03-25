using UnityEngine;

public class VacuumCleanerEnemy : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private bool isPhaseThrough = false;

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Speed Settings")]
    [SerializeField] private float phaseThroughSpeedMultiplier = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Collision Detection")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask tinhLinhLayer; 

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyMovement == null) Debug.LogError("EnemyMovement script not found on this GameObject.");
        if (col == null) Debug.LogError("Collider2D not found on this GameObject.");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer not found on this GameObject.");
    }

    private void Update()
    {
        Collider2D tinhLinh = Physics2D.OverlapCircle(transform.position, detectionRadius, tinhLinhLayer);

        if (tinhLinh != null && !isPhaseThrough)
        {
            isPhaseThrough = true;
            col.isTrigger = true;
            enemyMovement.UpdateSpeed(phaseThroughSpeedMultiplier);
            Debug.Log("Entering phase through state!");

            StopAllCoroutines();
            StartCoroutine(FadeSprite(0.3f));
        }
        else if (tinhLinh == null && isPhaseThrough)
        {
            isPhaseThrough = false;
            col.isTrigger = false;
            enemyMovement.ResetSpeed();
            Debug.Log("Exiting phase through state!");

            StopAllCoroutines();
            StartCoroutine(FadeSprite(1f));
            enemyMovement.TriggerDie();
        }
    }

    private System.Collections.IEnumerator FadeSprite(float targetAlpha)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(1f, 1f, 1f, newAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, targetAlpha);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

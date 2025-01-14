using UnityEngine;

public class VacuumCleanerEnemy : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int health = 20;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int damage = 5;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask trapLayer;
    [SerializeField] private Vector2[] detectionOffsets; 
    [SerializeField] private float boxSize = 1f; 

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;

    private bool isJumping = false;

    private void Update()
    {
        if (!isJumping)
        {
            DetectAndJumpOverTrap();
        }
    }

    private void DetectAndJumpOverTrap()
    {
        foreach (var offset in detectionOffsets)
        {
            Vector2 boxCenter = (Vector2)transform.position + offset;

            Collider2D hit = Physics2D.OverlapBox(boxCenter, new Vector2(boxSize, boxSize), 0f, trapLayer);

            if (hit != null)
            {
                Debug.Log("Trap detected at " + boxCenter + "! Jumping over...");
                StartCoroutine(JumpOverTrap());
                return; 
            }
        }
    }

    private System.Collections.IEnumerator JumpOverTrap()
    {
        isJumping = true;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y + jumpHeight);

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition; 
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (var offset in detectionOffsets)
        {
            Vector2 boxCenter = (Vector2)transform.position + offset;
            Gizmos.DrawWireCube(boxCenter, new Vector3(boxSize, boxSize, 0f));
        }
    }
}

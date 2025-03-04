using System.Collections;
using UnityEngine;
using UnityEditor;

public class Torch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask bulletMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;

    private void Update()
    {
        ChangeBulletAnimation();
    }

    private void ChangeBulletAnimation()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, bulletMask);

        foreach (RaycastHit2D hit in hits)
        {
            Bullet bullet = hit.transform.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.ChangeAnimatorState();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
#endif
    }
}

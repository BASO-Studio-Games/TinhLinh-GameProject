using System.Collections;
using UnityEngine;
using UnityEditor;

public class Torch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask bulletMask;
    [SerializeField] private LayerMask enemyMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float unfreezeDelay = 2f;

    private void Update()
    {
        ChangeBulletAnimation();
        UnfreezeEnemies();
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

    private void UnfreezeEnemies()
    {
        RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);

        foreach (RaycastHit2D hit in enemies)
        {
            TinhLinh tinhLinh = hit.transform.GetComponent<TinhLinh>();
            if (tinhLinh != null)
            {
                if (tinhLinh.IsFrozen)
                {
                    StartCoroutine(UnfreezeAfterDelay(tinhLinh));
                }
                else
                {
                    tinhLinh.IsImmuneToFreeze = true; 
                }
            }
        }
    }

    private IEnumerator UnfreezeAfterDelay(TinhLinh tinhLinh)
    {
        yield return new WaitForSeconds(unfreezeDelay);
        tinhLinh.Unfreeze();
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
#endif
    }
}

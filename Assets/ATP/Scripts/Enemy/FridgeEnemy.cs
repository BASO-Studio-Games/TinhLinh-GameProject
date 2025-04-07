using UnityEngine;
using System.Collections;

public class Fridge : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackInterval = 0.5f;
    [SerializeField] private float freezeDelay = 2f;
    [SerializeField] private GameObject freezeEffectPrefab;

    private TinhLinh targetTinhLinh;
    private EnemyMovement enemyMovement;
    private float attackCooldown;
    public bool IsAttack = false;

    [Header("References")]
    [SerializeField] private Animator animator;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement script not found on this GameObject.");
        }
    }

    private void Update()
    {
        if (IsAttack)
        {
            if (animator != null)
            {
                animator.SetBool("isAttack", true);
            }

            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TinhLinh tinhlinh = collision.gameObject.GetComponent<TinhLinh>();
        if (tinhlinh != null)
        {
            enemyMovement.isAttack = true;
            enemyMovement.isMoving = false;
            targetTinhLinh = tinhlinh;

            if (animator != null)
            {
                animator.SetBool("isBerore", true);
            }

            StartCoroutine(FreezeTargetAfterDelay());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsAttack && attackCooldown <= 0)
        {
            Attack();
            attackCooldown = attackInterval;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TinhLinh tinhlinh = collision.gameObject.GetComponent<TinhLinh>();
        if (tinhlinh != null && tinhlinh == targetTinhLinh)
        {
            enemyMovement.isAttack = false;
            enemyMovement.isMoving = true;
            targetTinhLinh = null;
            IsAttack = false;

            ResetAnimator();
            Debug.Log("Exit Collision and Reset Animator");
        }
    }

    public void Attack()
    {
        if (targetTinhLinh != null)
        {
            Debug.Log("Enemy Attack!");
            targetTinhLinh.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Target is null, stopping attack.");
            IsAttack = false;
            ResetAnimator();
        }
    }

    private void ResetAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isBerore", false);
            animator.Play("FanEnemyRun");
        }
    }

    public void ReadyAttack()
    {
        if (targetTinhLinh != null)
        {
            IsAttack = true;
        }
        else
        {
            IsAttack = false;
            ResetAnimator();
        }
    }

    private IEnumerator FreezeTargetAfterDelay()
    {
        yield return new WaitForSeconds(freezeDelay);

        if (targetTinhLinh != null)
        {
            ItemData itemData = targetTinhLinh.GetComponent<ItemData>();
            if (itemData != null && itemData.idTinhLinh == "M1TN07")
            {
                Debug.Log("Target M1TN07 is immune to freeze!");
                yield break;
            }

            if (targetTinhLinh.IsFrozen && targetTinhLinh.freezingFridge != this)
            {
                yield break;
            }

            targetTinhLinh.Freeze(this);

            AttackTinhLinh attackScript = targetTinhLinh.GetComponent<AttackTinhLinh>();
            Animator targetAnimator = targetTinhLinh.GetComponent<Animator>();

            if (attackScript != null) attackScript.enabled = false;
            if (targetAnimator != null) targetAnimator.enabled = false;

            DisableComponent<CrossbowTinhLinh>(targetTinhLinh.gameObject);
            DisableComponent<AxAttack>(targetTinhLinh.gameObject);
            DisableComponent<BommerTurret>(targetTinhLinh.gameObject);
            DisableComponent<StunTurret>(targetTinhLinh.gameObject);

            if (freezeEffectPrefab != null)
            {
                GameObject freezeEffect = Instantiate(freezeEffectPrefab, targetTinhLinh.transform.position, Quaternion.identity);
                FreezeEffect freezeScript = freezeEffect.GetComponent<FreezeEffect>();
                if (freezeScript != null) freezeScript.SetTarget(targetTinhLinh.transform);
            }
        }
    }




    private void DisableComponent<T>(GameObject target) where T : MonoBehaviour
    {
        T component = target.GetComponent<T>();
        if (component != null)
        {
            component.enabled = false;
            Debug.Log($"{typeof(T).Name} has been disabled!");
        }
    }

}

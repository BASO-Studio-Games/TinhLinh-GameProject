using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using Unity.Mathematics;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeBtn;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float bps = 1f;
    [SerializeField] private int baseUpgradeCost = 100;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float timeUntilFire;

    private int level = 1;

    private void Start()
    {
        bpsBase = bps;
        targetingRangeBase = targetingRange;

        upgradeBtn.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if(!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot()
    {
        Debug.Log("shoot");
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        BulletAutoTarGet bulletScript = bulletObj.GetComponent<BulletAutoTarGet>();
        bulletScript.SetTarget(target);
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);
        
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI?.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    public void Upgrade ()
    {
        if (CalculateCost() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendCurrency(CalculateCost());

        level++;

        bps = CalculateBPS();
        targetingRange = CalculateRange();

        CloseUpgradeUI();
    }

    private float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }

    private int CalculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.6f);
    }

    private void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
        #endif
    }
}

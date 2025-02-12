using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Thiết lập đường đi.")]
    public Transform startPoint;
    public Transform[] path;
    
    [Header("Thông số tài nguyên.")]
    [SerializeField] private int currency;
    public int maxEnergy;
    [SerializeField] private float maxEnergyGrowthFactor = 1.2f;
    private int currentEnergy;
    [HideInInspector] public bool isMaxEnergy;

    private EnemySpawner enemySpawner;

    private void Awake()
    {
        main = this;
        enemySpawner = GetComponent<EnemySpawner>();

        isMaxEnergy = false;
    }

    private void Start()
    {
        // currency = 100;
    }
    
    // Tiền tệ-----
    public void IncreaseCurrency(int amount)
    {
        if (!isMaxEnergy)
            currency += amount;
    }

    public bool CheckCurrency(int amount)
    {
        if (amount <= currency)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SpendCurrency(int amount)
    {
        if (CheckCurrency(amount))
        {
            currency -= amount;
        }
    }

    public int GetCurrency()
    {
       return currency;
    }

    // Năng lượng-----
    public void IncreaseEnergy(int amount)
    {
        if (isMaxEnergy) return;
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        MaxEnergyGrowth();
    }

    public int GetEnergy()
    {
       return currentEnergy;
    }

    public int GetMaxEnergy()
    {
       return maxEnergy;
    }

    private void MaxEnergyGrowth()
    {
        if (currentEnergy >= maxEnergy)
        {
            maxEnergy = Mathf.RoundToInt(maxEnergy * maxEnergyGrowthFactor);
            currentEnergy = 0;

            isMaxEnergy = true;
        }
    }
    

    // Lượt tấn công của quái-----
    public int GetCurrentWave(){
        return enemySpawner.currentWave;
    }

    public int GetTotalWave(){
        return enemySpawner.totalWaves;
    }

    public float GetWaveProgress(){
        return enemySpawner.GetWaveProgress();
    }

    public int GetTotalEnemies(int wave)
    {
        if (wave < 0 || wave >= enemySpawner.enemiesUpToWave.Length)
        {
            return enemySpawner.totalEnemies;
        }
        return enemySpawner.enemiesUpToWave[wave];
    }

    // Đường đi-------
    public void AddNewTransformTilePoint(Transform newTransform)
    {
        // Tăng kích thước mảng
        Array.Resize(ref path, path.Length + 1);
        
        // Thêm phần tử mới vào cuối mảng
        path[path.Length - 1] = newTransform;
    }
}

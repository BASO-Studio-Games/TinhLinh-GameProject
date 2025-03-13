using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject parentObject;

    [Header("Attributes")]
    // Tăng baseEnemies → Tăng số lượng kẻ địch mỗi wave.
    // Tăng enemiesPerSecond → Kẻ địch xuất hiện nhanh hơn.
    // Giảm timeBetweenWaves → Rút ngắn thời gian nghỉ giữa các wave.
    // Tăng difficultyScalingFactor → Game khó hơn nhanh hơn.
        //Khi difficultyScalingFactor lớn, số lượng kẻ địch và tốc độ spawn tăng nhanh hơn theo wave.
        //Khi difficultyScalingFactor nhỏ, game khó lên từ từ.
    // Tăng enemiesPerSecondCap → Giới hạn tốc độ spawn
    public int totalWaves; // Tổng số wave của level
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecondCap = 15f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    [HideInInspector] public int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps;
    public bool isSpawning = false;

    [HideInInspector] public int totalEnemies = 0;   // Tổng số quái trong tất cả các wave
    [HideInInspector] public  int[] enemiesUpToWave; // Số quái tích lũy từng wave
    private int enemiesSpawned = 0; // Số quái đã spawn

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
        CalculateTotalEnemies(); 
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        CheckWaveInLevel();
        if (!isSpawning) return;
        if (currentWave > totalWaves) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive <= 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }

    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        StartCoroutine(StartWave());
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private void SpawnEnemy()
    {
        int index = UnityEngine.Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[index];
        Vector3 spawnPosition = new Vector3(LevelManager.main.startPoint.position.x, LevelManager.main.startPoint.position.y, 0);
        GameObject spawnedEnemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedEnemy.transform.SetParent(parentObject.transform, false);

        enemiesSpawned++;
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSecond();
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }

    private void CalculateTotalEnemies()
    {
        enemiesUpToWave = new int[totalWaves];
        totalEnemies = 0;
        for (int i = 1; i <= totalWaves; i++)
        {
            totalEnemies += Mathf.RoundToInt(baseEnemies * Mathf.Pow(i, difficultyScalingFactor));
            enemiesUpToWave[i - 1] = totalEnemies;
        }
    }

    public float GetWaveProgress()
    {
        if (totalEnemies == 0) return 0f;
        return (float)enemiesSpawned / totalEnemies;
    }

    private void CheckWaveInLevel(){
        if (currentWave < totalWaves) return;

        // Debug.LogWarning("Làn sóng cuối cùng " + currentWave);
        if (enemiesSpawned == totalEnemies){
            if (parentObject.transform.childCount == 0){
                Debug.LogWarning("Thắng rồi ..........................");
            }
        }
    }
}

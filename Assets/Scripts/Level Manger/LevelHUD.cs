using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelHUD : MonoBehaviour
{
    [Header("Vàng và năng lượng")]
    [SerializeField] private TMP_Text currencyText; 
    [SerializeField] private Image energyCircle;
    [SerializeField] private TMP_Text energyText;

    [Header("Tiến trình quái")]
    [SerializeField] private Slider waveProgressSlider; // Thanh tiến trình wave
    [SerializeField] private Image waveIconPrefab;   // Prefab của icon wave

    private bool finalWaveAnnouncement;
    private Image[] waveIcons;
    
    void Start()
    {
        SpawnWaveIcons();
    }

    // Update is called once per frame
    void Update()
    {
        currencyText.text = LevelManager.main.GetCurrency().ToString();
    
        energyCircle.fillAmount = (float)LevelManager.main.GetEnergy() / (float)LevelManager.main.GetMaxEnergy();
        energyText.text = $"{LevelManager.main.GetEnergy()}\n[{LevelManager.main.GetMaxEnergy()}]";

        WaveProgressHUD();
    }

    private void WaveProgressHUD(){
        waveProgressSlider.value = LevelManager.main.GetWaveProgress();

        if (LevelManager.main.GetCurrentWave() >= LevelManager.main.GetTotalWave()){
            Transform fillTransform = waveProgressSlider.transform.Find("Fill Area");
            if (fillTransform != null)
            {
                Image fillImage = fillTransform.GetComponentInChildren<Image>();
                if (fillImage != null)
                {
                    if (!finalWaveAnnouncement){
                        CameraShake.Instance.StartShake();
                        NotificationMenu.Instance.ShowNotificationPanel("Kẻ thù đến rồi!", Color.red);
                        finalWaveAnnouncement = true;
                    }
                    fillImage.color = Color.red;
                }
            }
        }
    }

    private void SpawnWaveIcons()
    {
        int totalWaves = LevelManager.main.GetTotalWave();
        waveIcons = new Image[totalWaves];

        int totalEnemies = LevelManager.main.GetTotalEnemies(-1);

        RectTransform sliderRect = waveProgressSlider.GetComponent<RectTransform>();
        float sliderWidth = sliderRect.sizeDelta.x; // Lấy kích thước thanh tiến trình
        float sliderStartX = -sliderWidth / 2; // Điểm bắt đầu của thanh

        int enemiesUpToWave = 0; // Reset lại biến này trước khi loop

        for (int i = 0; i < totalWaves; i++)
        {
            // Tạo icon từ prefab
            Image icon = Instantiate(waveIconPrefab, waveProgressSlider.transform);
            waveIcons[i] = icon;

            // Lấy tổng số quái tính đến wave hiện tại
            enemiesUpToWave = LevelManager.main.GetTotalEnemies(i);

            // Debug.Log($"Wave {i + 1}: Tổng số quái đến hiện tại = {enemiesUpToWave}, tổng tất cả quái = {totalEnemies}");

            float waveProgress = (float)enemiesUpToWave / totalEnemies;
            // Debug.Log($"Wave {i + 1} end at {waveProgress * 100}%");

            // Tính vị trí icon từ trái sang phải
            float positionX = sliderStartX + sliderWidth * (1 - waveProgress);

            // Cập nhật vị trí icon
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchoredPosition = new Vector2(positionX, 0);
        }
    }
}

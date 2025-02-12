using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton

    public float shakeDuration = 0.5f;  // Thời gian rung
    public float shakeMagnitude = 0.3f; // Độ mạnh của rung
    public float dampingSpeed = 1.0f;   // Tốc độ giảm rung

    private Vector3 initialPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        initialPosition = transform.position;
    }

    /// <summary>
    /// Gọi hàm này để làm rung camera
    /// </summary>
    public void StartShake()
    {
        StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(initialPosition.x + xOffset, initialPosition.y + yOffset, initialPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Trả camera về vị trí ban đầu
        transform.position = initialPosition;
    }
}

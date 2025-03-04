using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject clickEffectPrefab;

    void Awake()
    {
        if (FindObjectsOfType<MouseEffectManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            CreateClickEffect();
        }
    }

    void CreateClickEffect()
    {
        GameObject effect = Instantiate(clickEffectPrefab, transform);
        effect.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Debug.Log(effect);
        Destroy(effect, 0.5f);
    }
}

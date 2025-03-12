using UnityEngine;
using System;

public class GuidePoint : MonoBehaviour
{
    public event Action OnGuidePointClicked;

    private void OnMouseDown()
    {
        OnGuidePointClicked?.Invoke();
        Destroy(gameObject);
    }
}

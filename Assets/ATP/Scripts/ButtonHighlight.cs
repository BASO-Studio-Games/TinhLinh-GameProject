using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.5f; 

    private void Start()
    {
        originalScale = transform.localScale; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; 
    }
}

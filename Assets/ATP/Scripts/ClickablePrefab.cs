using UnityEngine;
using UnityEngine.UI;

public class ClickablePrefab : MonoBehaviour
{
    public bool isClicked = false;
    public bool isCollided = false;

    public bool IsClicked => isClicked;
    public bool IsCollided => isCollided;

    private Button button;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
    }

    private void OnMouseDown()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsCurrentTarget(gameObject))
        {
            isClicked = true;
            Debug.Log($"Prefab {gameObject.name} has been clicked.");
            DialogueManager.Instance.OnPrefabClicked(gameObject);

            UpdateButtonState();
        }
    }

    public void SetCollisionState(bool state)
    {
        isCollided = state;
    }

    public void UpdateButtonState()
    {
        if (button != null)
        {
            button.enabled = isClicked;
        }
    }

    public void ResetCollisionState()
    {
        if (button != null)
        {
            button.enabled = true;
        }
    }
}

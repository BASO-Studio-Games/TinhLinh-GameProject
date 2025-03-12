using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (!enabled) return;

        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.IsCurrentTarget(gameObject))
            {
                Debug.Log(gameObject.name + " - YES ");
                DialogueManager.Instance.OnPrefabClicked(gameObject);
            }
            else
            {
                Debug.Log(gameObject.name + " - NO ");
            }
        }
    }
}

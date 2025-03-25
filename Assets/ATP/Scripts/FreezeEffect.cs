using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    private Transform target; 

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); 
        }
        else
        {
            transform.position = target.position; 
        }
    }
}

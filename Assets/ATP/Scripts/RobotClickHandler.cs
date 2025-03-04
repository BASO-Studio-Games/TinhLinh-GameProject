using UnityEngine;

public class RobotClickHandler : MonoBehaviour
{
    private static EnemyMovement selectedEnemy = null;

    private void OnMouseDown()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                EnemyMovement enemy = hit.collider.GetComponent<EnemyMovement>();
                if (enemy != null)
                {
                    SelectTinhLinh(enemy);
                }
            }
        }
    }

    private void SelectTinhLinh(EnemyMovement enemy)
    {
        if (selectedEnemy == enemy)
        {
            selectedEnemy.SetHpBarVisible(false);
            selectedEnemy = null;
        }
        else
        {
            if (selectedEnemy != null)
            {
                selectedEnemy.SetHpBarVisible(false);
            }

            selectedEnemy = enemy;
            selectedEnemy.SetHpBarVisible(true);
        }
    }
}

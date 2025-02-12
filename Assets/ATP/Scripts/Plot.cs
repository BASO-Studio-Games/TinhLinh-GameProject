using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject towerObj;
    public Turret turret;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        // if (UIManager.main.IsHoveringUI()) return;

        // if (towerObj != null)
        // {
        //     turret.OpenUpgradeUI();
        //     return;
        // }

        RollItem tinhlinhItem = BuildManager.main.GetSelectedTower();

        if (tinhlinhItem.GetCost() > LevelManager.main.GetCurrency())
        {
            return;
        }

        LevelManager.main.SpendCurrency(tinhlinhItem.GetCost());

        towerObj = Instantiate(tinhlinhItem.prefab, transform.position, Quaternion.identity);
        turret = towerObj.GetComponent<Turret>();
    }
}

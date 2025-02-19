using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameObject towerObj; // Đối tượng tinh linh
    [SerializeField] private Transform defendersContainer; // Container chứa tinh linh
    [HideInInspector] public Turret turret; // Script của tinh linh

    private string idTinhLinh; // ID của tinh linh

    private void Start()
    {
        idTinhLinh = "";
    }

    public void OnMouseDown()
    {
        // Kiểm tra nếu có Tool đang được chọn
        Tool selectedTool = BuildManager.main.GetSelectedTool();
        if (selectedTool != null)
        {
            selectedTool.Activate(this);
            return;
        }

        // Nếu không có Tool, kiểm tra xem người chơi có đang chọn tinh linh để đặt không
        RollItem tinhlinhItem = BuildManager.main.GetSelectedTower();
        if (tinhlinhItem != null)
        {
            PlaceTinhLinh(tinhlinhItem);
        }
    }

    private void PlaceTinhLinh(RollItem tinhlinhItem)
    {
        if (HasTinhLinh()) return;
        if (tinhlinhItem.GetCost() > LevelManager.main.GetCurrency()) return;

        LevelManager.main.SpendCurrency(tinhlinhItem.GetCost());

        towerObj = Instantiate(tinhlinhItem.prefab, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity, defendersContainer);
        idTinhLinh = tinhlinhItem.GetIdTinhLinh();
        turret = towerObj.GetComponent<Turret>();

        BuildManager.main.SetSelectedTower(null);
    }

    public bool HasTinhLinh()
    {
        return !string.IsNullOrEmpty(idTinhLinh);
    }

    public GameObject GetTinhLinhObject()
    {
        return towerObj;
    }

    public string GetTinhLinhID()
    {
        return idTinhLinh;
    }

    public Turret GetTurret()
    {
        return turret;
    }

    public void SetTinhLinh(GameObject tinhLinhObj, string tinhLinhID, Turret tinhLinhTurret)
    {
        towerObj = tinhLinhObj;
        towerObj.transform.position = transform.position + new Vector3(0, 0.3f, 0);
        towerObj.transform.SetParent(defendersContainer);

        idTinhLinh = tinhLinhID;
        turret = tinhLinhTurret;
    }

    public void ClearTile()
    {
        towerObj = null;
        idTinhLinh = "";
        turret = null;
    }
}

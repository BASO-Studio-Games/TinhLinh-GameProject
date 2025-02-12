using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    private RollItem selectedTinhLinh;
    private Tool selectedTool;

    private void Awake()
    {
        main = this;
    }

    public RollItem GetSelectedTower()
    {
        return selectedTinhLinh;
    }

    public void SetSelectedTower(RollItem tinhLinhItem)
    {
        selectedTinhLinh = tinhLinhItem;
        selectedTool = null; // Hủy Tool nếu chọn Tinh Linh
    }

    public Tool GetSelectedTool()
    {
        return selectedTool;
    }

    public void SetSelectedTool(Tool tool)
    {
        selectedTool = tool;
        selectedTinhLinh = null; // Hủy Tinh Linh nếu chọn Tool
    }

    public bool HasSelectedTool()
    {
        return selectedTool != null;
    }

    public bool HasSelectedTinhLinh()
    {
        return selectedTinhLinh != null;
    }
}

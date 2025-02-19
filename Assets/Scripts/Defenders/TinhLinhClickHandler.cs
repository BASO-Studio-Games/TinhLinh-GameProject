using UnityEngine;

public class TinhLinhClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    // Debug.Log("Tinh Linh bị nhấn -> Chuyển sự kiện xuống Tile: " + tile.name);
                tile.OnMouseDown();
                }
            }
        }
    }
}

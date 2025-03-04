using UnityEngine;

public class TinhLinhClickHandler : MonoBehaviour
{
    private static TinhLinh selectedTinhLinh = null; 

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
                    tile.OnMouseDown();
                }

                TinhLinh tinhLinh = hit.collider.GetComponent<TinhLinh>();
                if (tinhLinh != null)
                {
                    SelectTinhLinh(tinhLinh);
                }
            }
        }
    }

    private void SelectTinhLinh(TinhLinh tinhLinh)
    {
        if (selectedTinhLinh == tinhLinh)
        {
            selectedTinhLinh.SetHpBarVisible(false);
            selectedTinhLinh = null; 
        }
        else
        {
            if (selectedTinhLinh != null)
            {
                selectedTinhLinh.SetHpBarVisible(false);
            }

            selectedTinhLinh = tinhLinh;
            selectedTinhLinh.SetHpBarVisible(true);
        }
    }
}


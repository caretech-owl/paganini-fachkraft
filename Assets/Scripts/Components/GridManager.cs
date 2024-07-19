using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform gridRectTransform;
    public GameObject[] cardPrefabs; // Array to hold card prefabs

    private int columns;
    private int rows;
    private bool[,] gridSlots;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        columns = Mathf.FloorToInt(gridRectTransform.rect.width / gridLayoutGroup.cellSize.x);
        rows = Mathf.FloorToInt(gridRectTransform.rect.height / gridLayoutGroup.cellSize.y);

        gridSlots = new bool[columns, rows];
    }

    public void AddCard(GameObject cardPrefab, int width, int height)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (CanPlaceCard(x, y, width, height))
                {
                    PlaceCard(x, y, cardPrefab, width, height);
                    return;
                }
            }
        }
    }

    bool CanPlaceCard(int x, int y, int width, int height)
    {
        if (x + width > columns || y + height > rows)
            return false;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridSlots[x + i, y + j])
                    return false;
            }
        }

        return true;
    }

    void PlaceCard(int x, int y, GameObject cardPrefab, int width, int height)
    {
        GameObject card = Instantiate(cardPrefab, gridLayoutGroup.transform);
        RectTransform rt = card.GetComponent<RectTransform>();
        // Disable the GridLayoutGroup component temporarily
        gridLayoutGroup.enabled = false;
        // Manually set the position and size
        rt.anchoredPosition = new Vector2(x * gridLayoutGroup.cellSize.x, -y * gridLayoutGroup.cellSize.y);
        rt.sizeDelta = new Vector2(width * gridLayoutGroup.cellSize.x, height * gridLayoutGroup.cellSize.y);
        // Re-enable the GridLayoutGroup component
        gridLayoutGroup.enabled = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridSlots[x + i, y + j] = true;
            }
        }
    }
}

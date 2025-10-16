using UnityEngine;
using UnityEngine.UI;

public class FlexGridLayout : LayoutGroup
{
    [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);
    [SerializeField] private int columnsCount = 1;
    [SerializeField] private float maxCellWidth = 0f;
    [SerializeField] private float maxCellHeight = 0f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        SetCellsAlongAxis();
    }

    public override void CalculateLayoutInputVertical()
    {
        SetCellsAlongAxis();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    private void SetCellsAlongAxis()
    {
        int childCount = rectChildren.Count;
        if (childCount == 0)
            return;

        int columns = columnsCount > 0 ? columnsCount : Mathf.CeilToInt(Mathf.Sqrt(childCount));
        int rows = Mathf.CeilToInt((float)childCount / columns);

        float totalHorizontalSpacing = spacing.x * (columns - 1);
        float totalVerticalSpacing = spacing.y * (rows - 1);

        float availableWidth = rectTransform.rect.width - padding.horizontal - totalHorizontalSpacing;
        float availableHeight = rectTransform.rect.height - padding.vertical - totalVerticalSpacing;

        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;

        // Apply width and height caps if specified
        if (maxCellWidth > 0 && cellWidth > maxCellWidth)
            cellWidth = maxCellWidth;

        if (maxCellHeight > 0 && cellHeight > maxCellHeight)
            cellHeight = maxCellHeight;

        float contentWidth = cellWidth * columns + totalHorizontalSpacing;
        float contentHeight = cellHeight * rows + totalVerticalSpacing;

        float startX = GetStartOffset(0, contentWidth);
        float startY = GetStartOffset(1, contentHeight);

        int column = 0, row = 0;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform item = rectChildren[i];

            float xPos = startX + (cellWidth + spacing.x) * column;
            float yPos = startY + (cellHeight + spacing.y) * row;

            SetChildAlongAxis(item, 0, xPos, cellWidth);
            SetChildAlongAxis(item, 1, yPos, cellHeight);

            column++;
            if (column >= columns)
            {
                column = 0;
                row++;
            }
        }
    }
}

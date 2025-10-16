using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputCell : MonoBehaviour
{
    [HideInInspector] public int RowIndex;
    [HideInInspector] public int ColumnIndex;
    [HideInInspector] public GridGenerator2D ParentGrid;

    private TextMeshProUGUI _text;
    private Image _background;
    private bool _isActive = false;

    public bool IsFilled => !string.IsNullOrEmpty(_text.text);

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _background = GetComponent<Image>();
        UpdateVisual();
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        UpdateVisual();
    }

    /// <summary>
    /// Sets a character into this cell.
    /// Only allowed if cell is active AND empty (1 letter max).
    /// Automatically notifies the parent grid and moves focus to next cell.
    /// </summary>
    public void SetCharacter(char c)
    {
        if (!_isActive || IsFilled) return; // BLOCK multiple letters

        _text.text = c.ToString().ToUpper();
        UpdateVisual();

        // Notify parent grid that this cell has been filled
        ParentGrid.OnCellFilled(this); // Grid will move to next cell automatically
    }

    /// <summary>
    /// Clears the character and updates visuals.
    /// </summary>
    public void ClearCharacter()
    {
        _text.text = "";
        UpdateVisual();
    }

    /// <summary>
    /// Updates the cell's background color based on state.
    /// </summary>
    public void UpdateVisual()
    {
        if (_isActive)
            _background.color = ParentGrid.GetColorActive();
        else if (IsFilled)
            _background.color = ParentGrid.GetColorFilled();
        else
            _background.color = ParentGrid.GetColorEmpty();
    }

    /// <summary>
    /// Forces the cell's background to a specific color.
    /// </summary>
    public void SetColor(Color color)
    {
        if (_background != null)
            _background.color = color;
    }

    /// <summary>
    /// Returns the current character in the cell.
    /// </summary>
    public string GetCharacter()
    {
        return _text.text;
    }
}

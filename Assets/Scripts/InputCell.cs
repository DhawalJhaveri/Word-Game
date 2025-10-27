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

    public bool IsFilled => !string.IsNullOrEmpty(_text?.text);

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>(true); // true = include inactive children
        if (_text == null)
            Debug.LogError($"[InputCell] No TextMeshProUGUI found in {gameObject.name}");
        UpdateVisual();
    }


    // Manual initialization instead of Awake or Start
    public void Initialize(GridGenerator2D grid, int row, int column)
    {
        ParentGrid = grid;
        RowIndex = row;
        ColumnIndex = column;

        _text = GetComponentInChildren<TextMeshProUGUI>();
        _background = GetComponent<Image>();

        UpdateVisual();
    }

    public void SetActive(bool active)
    {
        _isActive = active;
        UpdateVisual();
    }

    public void SetCharacter(char c)
    {
        if (_text.text == c.ToString().ToUpper())
        {
            Debug.LogWarning($"[InputCell] Ignoring duplicate SetCharacter for '{c}' at Row {RowIndex}, Col {ColumnIndex}");
            return;
        }

        Debug.Log($"[InputCell] SetCharacter called on Row {RowIndex}, Col {ColumnIndex} | isActive={_isActive} | textRefNull={_text == null}");

        Debug.Log($"[InputCell] {gameObject.name} (ActiveInHierarchy={gameObject.activeInHierarchy}) | Transform path: {transform.GetHierarchyPath()} | Setting text '{c}'");

        // If text component missing, show more info and return
        if (_text == null)
        {
            Debug.LogError($"[InputCell] TEXT IS NULL on Row {RowIndex}, Col {ColumnIndex}. GameObject: {gameObject.name}. Components: {string.Join(", ", System.Array.ConvertAll(gameObject.GetComponents<Component>(), x => x.GetType().Name))}");
            return;
        }

        // Check active state
        if (!_isActive)
        {
            Debug.LogWarning($"[InputCell] Cell not active, refusing input. Row {RowIndex}, Col {ColumnIndex}");
            return;
        }

        // If already filled
        if (!string.IsNullOrEmpty(_text.text))
        {
            Debug.LogWarning($"[InputCell] Cell already has text '{_text.text}'. Row {RowIndex}, Col {ColumnIndex}");
            return;
        }

        // Set text
        _text.text = c.ToString().ToUpper();
        Debug.Log($"[InputCell] Text set to '{_text.text}' on Row {RowIndex}, Col {ColumnIndex}");

        // Update visuals and notify grid
        UpdateVisual();
        if (ParentGrid == null)
            Debug.LogError($"[InputCell] ParentGrid is NULL on Row {RowIndex}, Col {ColumnIndex}");
        else
            ParentGrid.OnCellFilled(this);

        Debug.Log($"[InputCell] Setting '{c}' on {_text.gameObject.name} (was '{_text.text}')");
        _text.text = c.ToString().ToUpper();
    }

    public void ClearCharacter()
    {
        if (_text == null)
            return;

        _text.text = "";
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (_background == null || ParentGrid == null)
            return;

        if (_isActive)
            _background.color = ParentGrid.GetColorActive();
        else if (IsFilled)
            _background.color = ParentGrid.GetColorFilled();
        else
            _background.color = ParentGrid.GetColorEmpty();
    }

    public void SetColor(Color color)
    {
        if (_background != null)
            _background.color = color;
    }

    public string GetCharacter()
    {
        return _text?.text ?? "";
    }
}


public static class TransformExtensions
{
    public static string GetHierarchyPath(this Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}

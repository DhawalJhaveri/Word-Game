using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GridGenerator2D : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cellPrefab;        // Character cells
    [SerializeField] private GameObject inputCellsPrefab;  // Input cells
    [SerializeField] private TextMeshProUGUI currentWordDisplay;

    [Header("Cell Settings")]
    [SerializeField] private Vector2 cellSize = new Vector2(100, 100);
    [SerializeField] private Vector2 spacing = new Vector2(10, 10);

    [Header("Word Settings")]
    [SerializeField] private string oxfordWordsPath = "Assets/grid words/oxford_words.json";
    [SerializeField] private int currentWordIndex = 0;

    [Header("Input Cell Colors")]
    [SerializeField] private Color colorEmpty = Color.white;
    [SerializeField] private Color colorFilled = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color colorActive = new Color(1f, 0.95f, 0.6f);

    // Data
    private List<string> oxfordWords = new();
    private bool wordsLoaded = false;

    // Grid input management
    public List<List<InputCell>> inputRows = new();
    private int activeRowIndex = 0;
    private int activeCellIndex = 0;

    private List<string> completedWords = new(); // Store completed words

    private void Awake()
    {
        LoadOxfordWords();

        // Subscribe to keyboard events
        Keyboard.OnKeyPressedEvent += HandleKeyboardInput;
        Keyboard.OnBackspaceEvent += HandleBackspaceInput;
    }

    private void OnDestroy()
    {
        Keyboard.OnKeyPressedEvent -= HandleKeyboardInput;
        Keyboard.OnBackspaceEvent -= HandleBackspaceInput;
    }

    private void Start()
    {
        GenerateProceduralGrid();
    }

    #region JSON Loading
    [Serializable]
    private class OxfordWordsData { public string[] words; }

    private void LoadOxfordWords()
    {
        try
        {
            string jsonPath = Path.Combine(Application.dataPath, oxfordWordsPath.Replace("Assets/", ""));
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"Oxford words file not found: {jsonPath}");
                return;
            }

            string jsonContent = File.ReadAllText(jsonPath);
            OxfordWordsData wordData = JsonUtility.FromJson<OxfordWordsData>(jsonContent);
            if (wordData?.words != null)
            {
                oxfordWords = new List<string>(wordData.words);
                wordsLoaded = true;
                Debug.Log($"Loaded {oxfordWords.Count} words.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading Oxford words: {e.Message}");
        }
    }
    #endregion

    #region Grid Generation
    public void GenerateProceduralGrid()
    {
        if (!wordsLoaded || oxfordWords.Count == 0)
        {
            Debug.LogWarning("Oxford words not loaded! Cannot generate grid.");
            return;
        }

        string word = oxfordWords[currentWordIndex];
        Debug.Log($"Generating grid for word {currentWordIndex + 1}/{oxfordWords.Count}: '{word}'");

        currentWordIndex = (currentWordIndex + 1) % oxfordWords.Count;

        char[] inputCharacters = word.ToCharArray();

        // Clear previous grid
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        inputRows.Clear();
        completedWords.Clear();
        activeRowIndex = 0;
        activeCellIndex = 0;

        CreateCrosswordStyleGrid(inputCharacters);
        UpdateWordDisplay();
    }

    private void CreateCrosswordStyleGrid(char[] inputCharacters)
    {
        int totalRows = inputCharacters.Length;

        int[] characterColumnLengths = new int[totalRows];
        List<int> availableLengths = new();
        for (int length = 4; length <= 8; length++) availableLengths.Add(length);

        // Assign random column lengths
        for (int i = 0; i < totalRows; i++)
        {
            if (availableLengths.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, availableLengths.Count);
                characterColumnLengths[i] = availableLengths[index];
                availableLengths.RemoveAt(index);
            }
            else
            {
                characterColumnLengths[i] = UnityEngine.Random.Range(4, 9);
            }

            Debug.Log($"[Row {i}] Column Length = {characterColumnLengths[i]}");
        }

        int maxColumns = Mathf.Max(characterColumnLengths);
        float startX = -((maxColumns * (cellSize.x + spacing.x)) / 2f) + cellSize.x / 2f;
        float startY = (totalRows * (cellSize.y + spacing.y)) / 2f - cellSize.y / 2f;

        for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
        {
            List<InputCell> rowCells = new();
            int colLength = characterColumnLengths[rowIndex];
            int middleColumn = maxColumns / 2;
            int startColumn = middleColumn - (colLength / 2);
            int charColumn = startColumn + (colLength / 2);

            Debug.Log($"-- Row {rowIndex}: startCol={startColumn}, charCol={charColumn}, colLength={colLength}");

            for (int col = startColumn; col < startColumn + colLength; col++)
            {
                GameObject prefabToUse = (col == charColumn) ? cellPrefab : inputCellsPrefab;
                GameObject cellGO = Instantiate(prefabToUse, transform);
                RectTransform rect = cellGO.GetComponent<RectTransform>();
                rect.sizeDelta = cellSize;
                rect.anchoredPosition = new Vector2(startX + col * (cellSize.x + spacing.x), startY - rowIndex * (cellSize.y + spacing.y));

                if (prefabToUse == cellPrefab)
                {
                    TextMeshProUGUI textComp = cellGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComp != null)
                    {
                        textComp.text = inputCharacters[rowIndex].ToString().ToUpper();
                        textComp.color = Color.black;
                        textComp.fontSize = 24;
                    }
                }
                else
                {
                    InputCell inputCell = cellGO.GetComponent<InputCell>();
                    if (inputCell != null)
                    {
                        inputCell.RowIndex = rowIndex;
                        inputCell.ColumnIndex = col;
                        inputCell.ParentGrid = this;
                        rowCells.Add(inputCell);
                        inputCell.SetColor(colorEmpty);
                    }
                }
            }

            if (rowCells.Count > 0)
                inputRows.Add(rowCells);
        }

        if (inputRows.Count > 0 && inputRows[0].Count > 0)
            SetActiveCell(0, 0);
    }
    #endregion

    #region Keyboard Handling
    private void HandleKeyboardInput(char inputChar)
    {
        if (inputRows.Count == 0) return;

        var currentRow = inputRows[activeRowIndex];
        var currentCell = currentRow[activeCellIndex];

        currentCell.SetCharacter(inputChar);
    }

    private void HandleBackspaceInput()
    {
        if (inputRows.Count == 0) return;

        var currentRow = inputRows[activeRowIndex];
        var currentCell = currentRow[activeCellIndex];

        if (!currentCell.IsFilled && activeCellIndex > 0)
        {
            activeCellIndex--;
            SetActiveCell(activeRowIndex, activeCellIndex);
            inputRows[activeRowIndex][activeCellIndex].ClearCharacter();
        }
        else
        {
            currentCell.ClearCharacter();
            SetActiveCell(activeRowIndex, activeCellIndex);
        }

        if (activeCellIndex == 0 && activeRowIndex > 0 && !currentRow[0].IsFilled)
        {
            activeRowIndex--;
            activeCellIndex = inputRows[activeRowIndex].Count - 1;
            SetActiveCell(activeRowIndex, activeCellIndex);
        }
    }
    #endregion

    #region Cell Activation & Row Completion
    public void OnCellFilled(InputCell cell)
    {
        // Get the current row
        var row = inputRows[cell.RowIndex];

        // If the row is not yet complete, move to the next input cell
        if (activeCellIndex + 1 < row.Count)
        {
            activeCellIndex++;
            SetActiveCell(cell.RowIndex, activeCellIndex);
        }
        else
        {
            // Row is complete, collect letters
            string rowWord = "";

            // Include letters from inputCellsPrefab
            foreach (var c in row)
            {
                rowWord += c.GetCharacter();
            }

            // Include the fixed character from the character cell
            rowWord += GetRowCharacter(cell.RowIndex);

            completedWords.Add(rowWord);
            Debug.Log($"Row {cell.RowIndex + 1} completed: {rowWord}");

            // Move to next row if any
            if (cell.RowIndex + 1 < inputRows.Count)
            {
                activeRowIndex++;
                activeCellIndex = 0;
                SetActiveCell(activeRowIndex, activeCellIndex);
            }
            else
            {
                // All rows finished
                Debug.Log("All rows completed!");
                DisplayCollectedWords();
            }
        }
    }

    /// <summary>
    /// Returns the letter from the fixed character cell for a given row
    /// </summary>
    private string GetRowCharacter(int rowIndex)
    {
        foreach (Transform child in transform)
        {
            InputCell inputCell = child.GetComponent<InputCell>();
            if (inputCell == null)
            {
                // This is the fixed cellPrefab
                TextMeshProUGUI textComp = child.GetComponentInChildren<TextMeshProUGUI>();
                if (textComp != null)
                {
                    // Check approximate Y position to match the row
                    float yPos = child.transform.localPosition.y;
                    float expectedY = (cellSize.y + spacing.y) * rowIndex * -1;
                    if (Mathf.Abs(yPos - expectedY) < 0.1f)
                        return textComp.text;
                }
            }
        }
        return "";
    }

    private char GetCharacterPrefab(int rowIndex)
    {
        foreach (Transform child in transform)
        {
            TextMeshProUGUI textComp = child.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null && !child.TryGetComponent(out InputCell _))
            {
                float yPos = -rowIndex * (cellSize.y + spacing.y);
                if (Mathf.Abs(child.localPosition.y - yPos) < 0.1f)
                    return textComp.text[0];
            }
        }
        return '?';
    }

    private void SetActiveCell(int rowIndex, int colIndex)
    {
        for (int r = 0; r < inputRows.Count; r++)
            foreach (var c in inputRows[r])
                c.SetColor(colorFilled);

        var activeCell = inputRows[rowIndex][colIndex];
        activeCell.SetColor(colorActive);

        activeRowIndex = rowIndex;
        activeCellIndex = colIndex;
    }
    #endregion

    #region Display Helpers
    private void UpdateWordDisplay()
    {
        if (currentWordDisplay != null && wordsLoaded && oxfordWords.Count > 0)
        {
            int nextIndex = currentWordIndex;
            currentWordDisplay.text = $"Next Word {nextIndex + 1}/{oxfordWords.Count}: {oxfordWords[nextIndex].ToUpper()}";
        }
    }

    private void DisplayCollectedWords()
    {
        string allWords = string.Join(", ", completedWords);
        Debug.Log("Collected words: " + allWords);

        if (currentWordDisplay != null)
            currentWordDisplay.text = "Words: " + allWords;
    }
    #endregion

    public Color GetColorActive() => colorActive;
    public Color GetColorFilled() => colorFilled;
    public Color GetColorEmpty() => colorEmpty;
}

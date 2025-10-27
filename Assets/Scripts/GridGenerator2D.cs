using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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

    [SerializeField] private GameObject hide_Keyboard;

    // Data
    private List<string> oxfordWords = new();
    private bool wordsLoaded = false;

    // Grid input management
    public List<List<InputCell>> inputRows = new();
    private int activeRowIndex = 0;
    private int activeCellIndex = 0;

    private List<int> fixedCharColumnIndices = new(); // Store fixed cell column index for each row
    private List<string> fixedCharLetters = new List<string>(); // fixed letter for each row
    private List<string> completedWords = new(); // Store completed words
    private HashSet<int> completedRowIndices = new(); // <— NEW

    private void Awake()
    {
        LoadOxfordWords();

        Keyboard.OnKeyPressedEvent += HandleKeyboardInput;
        Keyboard.OnBackspaceEvent += HandleBackspaceInput;
        Keyboard.OnSubmitEvent += HandleSubmitPressed; // add this
    }

    private void OnDestroy()
    {
        Keyboard.OnKeyPressedEvent -= HandleKeyboardInput;
        Keyboard.OnBackspaceEvent -= HandleBackspaceInput;
        Keyboard.OnSubmitEvent -= HandleSubmitPressed; // remove this
    }

    private void HandleSubmitPressed()
    {
        Debug.Log("[Grid] Submit pressed -> loading next word");
        GenerateProceduralGrid();
    }

    private void Start()
    {
        hide_Keyboard.SetActive(false);
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
        fixedCharColumnIndices.Clear();
        completedWords.Clear();
        activeRowIndex = 0;
        activeCellIndex = 0;

        CreateCrosswordStyleGrid(inputCharacters);
        UpdateWordDisplay();
    }

    private void CreateCrosswordStyleGrid(char[] inputCharacters)
    {
        int totalRows = inputCharacters.Length;

        int[] characterPositions = new int[totalRows];
        int[] characterColumnLengths = new int[totalRows];

        // Unique column lengths list (4-8)
        List<int> availableLengths = new List<int>();
        for (int length = 4; length <= 8; length++)
            availableLengths.Add(length);

        // Assign random positions and column lengths
        for (int i = 0; i < totalRows; i++)
        {
            characterPositions[i] = UnityEngine.Random.Range(1, 9);

            if (availableLengths.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableLengths.Count);
                characterColumnLengths[i] = availableLengths[randomIndex];
                availableLengths.RemoveAt(randomIndex);
            }
            else
            {
                characterColumnLengths[i] = UnityEngine.Random.Range(4, 9);
            }

            Debug.Log($"[Row {i}] Character: '{inputCharacters[i]}' | Position: {characterPositions[i]} | Column Length: {characterColumnLengths[i]}");
        }

        int maxColumns = Mathf.Max(characterColumnLengths);
        Debug.Log($"Max Columns across all rows: {maxColumns}");

        float startX = -((maxColumns * (cellSize.x + spacing.x)) / 2f) + cellSize.x / 2f;
        float startY = (totalRows * (cellSize.y + spacing.y)) / 2f - cellSize.y / 2f;

        // Reset tracking lists
        inputRows.Clear();
        fixedCharColumnIndices.Clear();
        fixedCharLetters.Clear();
        completedWords.Clear();

        for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
        {
            List<InputCell> rowCells = new List<InputCell>();
            int colLength = characterColumnLengths[rowIndex];

            int middleColumn = maxColumns / 2;
            int startColumn = middleColumn - (colLength / 2);
            int charColumn = startColumn + (colLength / 2);

            Debug.Log($"-- Generating Row {rowIndex}: StartCol={startColumn}, CharCol={charColumn}, ColLength={colLength}");

            // instantiate cells for this row
            for (int col = startColumn; col < startColumn + colLength; col++)
            {
                bool isFixedLetter = (col == charColumn);
                GameObject prefabToUse = isFixedLetter ? cellPrefab : inputCellsPrefab;
                GameObject cellGO = Instantiate(prefabToUse, transform);

                // ensure active
                cellGO.SetActive(true);

                RectTransform rect = cellGO.GetComponent<RectTransform>();
                rect.sizeDelta = cellSize;
                rect.anchoredPosition = new Vector2(
                    startX + col * (cellSize.x + spacing.x),
                    startY - rowIndex * (cellSize.y + spacing.y)
                );

                if (isFixedLetter)
                {
                    // Fixed character cell: set its letter
                    TextMeshProUGUI textComp = cellGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComp != null)
                    {
                        string letter = inputCharacters[rowIndex].ToString().ToUpper();
                        textComp.text = letter;
                        textComp.color = Color.black;
                        textComp.fontSize = 24;
                    }
                }
                else
                {
                    // Input cell: initialize and add to rowCells
                    InputCell inputCell = cellGO.GetComponent<InputCell>();
                    if (inputCell != null)
                    {
                        inputCell.RowIndex = rowIndex;
                        inputCell.ColumnIndex = col - startColumn; // column relative to this row
                        inputCell.ParentGrid = this;
                        rowCells.Add(inputCell);
                        // force initial visual update (optional)
                        inputCell.UpdateVisual();
                    }
                    else
                    {
                        Debug.LogWarning($"InputCell component missing on prefab at Row {rowIndex}, Col {col}");
                    }
                }

                Debug.Log($"---- Instantiated {(isFixedLetter ? "CHAR" : "INPUT")} Cell at Row {rowIndex}, Column {col}");
            }

            // store row if it has input cells
            if (rowCells.Count > 0)
            {
                inputRows.Add(rowCells);

                // compute and store relative fixed char index inside this row
                int relativeCharIndex = Mathf.Clamp(charColumn - startColumn, 0, rowCells.Count);
                fixedCharColumnIndices.Add(relativeCharIndex);

                // store fixed letter directly (safe and exact)
                string fixedLetter = inputCharacters[rowIndex].ToString().ToUpper();
                fixedCharLetters.Add(fixedLetter);

                Debug.Log($"[Row {rowIndex}] Stored fixed char index = {relativeCharIndex}, letter = {fixedLetter}");
            }
            else
            {
                // still push empty row to keep indexing consistent (optional)
                inputRows.Add(new List<InputCell>());
                fixedCharColumnIndices.Add(0);
                fixedCharLetters.Add(inputCharacters[rowIndex].ToString().ToUpper());
            }
        }

        // Activate first available input cell
        for (int r = 0; r < inputRows.Count; r++)
        {
            if (inputRows[r].Count > 0)
            {
                activeRowIndex = r;
                activeCellIndex = 0;
                SetActiveCell(activeRowIndex, activeCellIndex);
                Debug.Log($"[Init] Active cell set to Row {r}, Col 0");
                break;
            }
        }
    }
    #endregion

    #region Keyboard Handling
    private void HandleKeyboardInput(char inputChar)
    {
        Debug.Log($"[Grid] HandleKeyboardInput received '{inputChar}'. activeRow={activeRowIndex}, activeCol={activeCellIndex}, rows={inputRows.Count}");

        if (inputRows.Count == 0) return;
        if (activeRowIndex < 0 || activeRowIndex >= inputRows.Count) return;

        var currentRow = inputRows[activeRowIndex];
        if (currentRow == null || currentRow.Count == 0) { Debug.LogWarning("[Grid] currentRow empty"); return; }
        if (activeCellIndex < 0 || activeCellIndex >= currentRow.Count) { Debug.LogWarning("[Grid] activeCellIndex out of range"); return; }

        var currentCell = currentRow[activeCellIndex];
        Debug.Log($"[Grid] Calling SetCharacter on cell GameObject '{currentCell.gameObject.name}' (Row {activeRowIndex}, Col {activeCellIndex})");
        currentCell.SetCharacter(inputChar);
    }

    private void HandleBackspaceInput()
    {
        if (inputRows.Count == 0) return;

        var currentRow = inputRows[activeRowIndex];
        var currentCell = currentRow[activeCellIndex];

        // Case 1: Current cell has a letter -> clear it directly
        if (currentCell.IsFilled)
        {
            currentCell.ClearCharacter();
        }
        // Case 2: Current cell is empty -> move back and clear previous one
        else if (activeCellIndex > 0)
        {
            activeCellIndex--;
            SetActiveCell(activeRowIndex, activeCellIndex);
            currentRow[activeCellIndex].ClearCharacter();
        }
        // Case 3: We're at the first cell but not in the first row -> move to previous row
        else if (activeRowIndex > 0)
        {
            activeRowIndex--;
            var prevRow = inputRows[activeRowIndex];
            activeCellIndex = prevRow.Count - 1;
            SetActiveCell(activeRowIndex, activeCellIndex);
            prevRow[activeCellIndex].ClearCharacter();
        }

        // If this row was previously marked completed, remove its word
        if (completedRowIndices.Contains(activeRowIndex))
        {
            completedRowIndices.Remove(activeRowIndex);

            // Safety check in case word list and rows mismatch
            if (activeRowIndex < completedWords.Count)
                completedWords[activeRowIndex] = "";

            Debug.Log($"[Grid] Row {activeRowIndex + 1} edited -> old word removed");
        }
    }
    #endregion

    #region Cell Activation & Row Completion
    public void OnCellFilled(InputCell cell)
    {
        if (cell == null || cell.RowIndex < 0 || cell.RowIndex >= inputRows.Count)
        {
            Debug.LogWarning("[Grid] Invalid cell reference in OnCellFilled.");
            return;
        }

        var row = inputRows[cell.RowIndex];
        if (row == null || row.Count == 0)
        {
            Debug.LogWarning($"[Grid] Row {cell.RowIndex} is empty or null.");
            return;
        }

        // Check if row is now fully filled
        bool rowComplete = row.TrueForAll(c => c.IsFilled);

        if (rowComplete)
        {
            // --- Build the completed word including the fixed letter at correct position ---
            string newWord = "";
            int fixedColIndex = fixedCharColumnIndices[cell.RowIndex];
            string fixedLetter = fixedCharLetters[cell.RowIndex];

            for (int i = 0; i <= row.Count; i++)
            {
                if (i == fixedColIndex)
                    newWord += fixedLetter;

                if (i < row.Count)
                    newWord += row[i].GetCharacter();
            }

            // --- Ensure visuals update properly (disable highlight + filled color) ---
            foreach (var c in row)
            {
                c.SetActive(false);
                c.UpdateVisual();
            }

            // --- Check if this row word already existed (correction case) ---
            if (completedRowIndices.Contains(cell.RowIndex))
            {
                string oldWord = completedWords[cell.RowIndex];
                completedWords[cell.RowIndex] = newWord;
                Debug.Log($"[Grid]  Corrected word in Row {cell.RowIndex + 1}: '{oldWord}' -> '{newWord}'");
            }
            else
            {
                completedRowIndices.Add(cell.RowIndex);

                // Ensure completedWords list has proper capacity
                while (completedWords.Count <= cell.RowIndex)
                    completedWords.Add("");

                completedWords[cell.RowIndex] = newWord;
                Debug.Log($"[Grid] Row {cell.RowIndex + 1} completed: {newWord}");
            }

            // --- Move to next row or finish ---
            if (cell.RowIndex + 1 < inputRows.Count)
            {
                activeRowIndex++;
                activeCellIndex = 0;
                SetActiveCell(activeRowIndex, activeCellIndex);
            }
            else
            {
                Debug.Log("[Grid] All rows completed. Displaying collected words...");
                DisplayCollectedWords();

                // Instead of dimming or disabling manually, show the hide_Keyboard UI
                if (hide_Keyboard != null)
                {
                    hide_Keyboard.SetActive(true);
                    Debug.Log("[Grid] hide_Keyboard activated — keyboard interaction stopped.");
                }
                else
                {
                    Debug.LogWarning("[Grid] hide_Keyboard reference not assigned!");
                }

                // Optionally stop receiving keyboard events (still safe)
                Keyboard.OnKeyPressedEvent -= HandleKeyboardInput;
                Keyboard.OnBackspaceEvent -= HandleBackspaceInput;
            }
        }
        else
        {
            // --- Row was previously completed but now edited ---
            if (completedRowIndices.Contains(cell.RowIndex))
            {
                string oldWord = completedWords[cell.RowIndex];
                completedRowIndices.Remove(cell.RowIndex);
                completedWords[cell.RowIndex] = "";
                Debug.Log($"[Grid] Row {cell.RowIndex + 1} uncompleted — old word removed: '{oldWord}'");
            }
            else
            {
                // Move focus to next input cell if available
                if (activeCellIndex + 1 < row.Count)
                {
                    activeCellIndex++;
                    SetActiveCell(cell.RowIndex, activeCellIndex);
                }
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
            InputCell ic = child.GetComponent<InputCell>();
            if (ic == null)
            {
                TextMeshProUGUI textComp = child.GetComponentInChildren<TextMeshProUGUI>();
                if (textComp != null &&
                    Mathf.RoundToInt(child.localPosition.y) ==
                    Mathf.RoundToInt(-rowIndex * (cellSize.y + spacing.y)))
                {
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
        Debug.Log($"[Grid] SetActiveCell called -> row {rowIndex}, col {colIndex}");

        // Deactivate all cells and set filled color for previously filled ones
        for (int r = 0; r < inputRows.Count; r++)
        {
            for (int c = 0; c < inputRows[r].Count; c++)
            {
                var ic = inputRows[r][c];
                ic.SetActive(false);
            }
        }

        // safety bounds
        if (rowIndex < 0 || rowIndex >= inputRows.Count)
        {
            Debug.LogError($"[Grid] SetActiveCell: rowIndex {rowIndex} out of range");
            return;
        }
        if (colIndex < 0 || colIndex >= inputRows[rowIndex].Count)
        {
            Debug.LogError($"[Grid] SetActiveCell: colIndex {colIndex} out of range for row {rowIndex} (count={inputRows[rowIndex].Count})");
            return;
        }

        var activeCell = inputRows[rowIndex][colIndex];
        Debug.Log($"[Grid] Activating cell GameObject '{activeCell.gameObject.name}' (Row {rowIndex}, Col {colIndex})");

        activeCell.SetActive(true); // this sets the cell's _isActive and calls UpdateVisual
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

    public void DisplayCollectedWords()
    {
        Debug.Log("====================================");
        Debug.Log("Final Collected Words:");

        for (int i = 0; i < completedWords.Count; i++)
        {
            string word = completedWords[i];

            // Skip rows that are empty or unfilled
            if (string.IsNullOrEmpty(word))
                continue;

            Debug.Log($"{i + 1}. {word}");
        }

        Debug.Log("====================================");
    }
    #endregion

    public Color GetColorActive() => colorActive;
    public Color GetColorFilled() => colorFilled;
    public Color GetColorEmpty() => colorEmpty;
}

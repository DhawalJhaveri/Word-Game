using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GridGenerator2D : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cellPrefab;        // Prefab for each cell (UI Image)
    [SerializeField] private GameObject inputCellsPrefab;
    [SerializeField] private TextMeshProUGUI currentWordDisplay;  // Display current word from JSON

    [Header("Cell Settings")]
    private Vector2 cellSize = new Vector2(100, 100);
    private Vector2 spacing = new Vector2(10, 10);

    [Header("Word Settings")]
    [SerializeField] private string oxfordWordsPath = "Assets/grid words/oxford_words.json";
    [SerializeField] private int currentWordIndex = 0;

    private GameObject[,] gridArray;
    private RectTransform panelRect;
    private char[] inputCharacters;      // Array to store individual characters
    private int[] characterPositions;    // Array to store position numbers (1-9) for each character
    private int[] characterColumnLengths; // Array to store column length (4-8) for each character
    
    // Oxford words data
    private List<string> oxfordWords = new List<string>();
    private bool wordsLoaded = false;

    void Awake()
    {
        panelRect = GetComponent<RectTransform>();
        LoadOxfordWords();
    }

    void Start()
    {
        //yield return new WaitForSeconds(0.25f);

        GenerateProceduralGrid();
    }

    private void LoadOxfordWords()
    {
        try
        {
            string jsonPath = Path.Combine(Application.dataPath, oxfordWordsPath.Replace("Assets/", ""));
            
            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                OxfordWordsData wordData = JsonUtility.FromJson<OxfordWordsData>(jsonContent);
                
                if (wordData != null && wordData.words != null)
                {
                    oxfordWords = new List<string>(wordData.words);
                    wordsLoaded = true;
                    
                    Debug.Log($"Loaded {oxfordWords.Count} words from Oxford dictionary");

                    Debug.Log("Word List = " + JsonUtility.ToJson(oxfordWords));
                                        
                    Debug.Log($"First few words: {string.Join(", ", oxfordWords.GetRange(0, Mathf.Min(10, oxfordWords.Count)))}");
                }
                else
                {
                    Debug.LogError("Failed to parse Oxford words JSON data");
                }
            }
            else
            {
                Debug.LogError($"Oxford words file not found at: {jsonPath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading Oxford words: {e.Message}");
        }
    }

    public void GenerateProceduralGrid()
    {
        // Get the current word from Oxford dictionary
        if (!wordsLoaded || oxfordWords.Count == 0)
        {
            Debug.LogWarning("Oxford words not loaded! Cannot generate grid.");
            return;
        }

        // Get word in ascending order and automatically move to next word
        string word = oxfordWords[currentWordIndex];
        
        Debug.Log($"Generating grid for word {currentWordIndex + 1}/{oxfordWords.Count}: '{word}'");
        
        // Automatically move to next word for next call
        currentWordIndex = (currentWordIndex + 1) % oxfordWords.Count;

        // Break down the input word into individual characters
        inputCharacters = word.ToCharArray();
        
        // Initialize the arrays
        characterPositions = new int[inputCharacters.Length];
        characterColumnLengths = new int[inputCharacters.Length];
        
        // Assign random numbers (4-8) for both positions and column lengths
        Debug.Log($"Processing word: '{word}' with {inputCharacters.Length} characters:");
        
        // Ensure unique column lengths for each character
        List<int> availableLengths = new List<int>();
        for (int length = 4; length <= 8; length++)
        {
            availableLengths.Add(length);
        }
        
        for (int i = 0; i < inputCharacters.Length; i++)
        {
            characterPositions[i] = Random.Range(1, 9);     // Random position between 1-9 (inclusive)
            
            // Assign unique column length
            if (availableLengths.Count > 0)
            {
                int randomIndex = Random.Range(0, availableLengths.Count);
                characterColumnLengths[i] = availableLengths[randomIndex];
                availableLengths.RemoveAt(randomIndex); // Remove to ensure uniqueness
            }
            else
            {
                // Fallback if we run out of unique lengths (shouldn't happen with 5 characters max)
                characterColumnLengths[i] = Random.Range(4, 9);
            }
            
            Debug.Log($"Character '{inputCharacters[i]}' at index {i}: Position = {characterPositions[i]}, Column Length = {characterColumnLengths[i]}");
        }

        // Clear any existing cells
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Create crossword puzzle style grid with irregular shapes
        CreateCrosswordStyleGrid();

        Debug.Log($"Generated crossword-style grid for word: '{word}'");
        Debug.Log($"Character positions: [{string.Join(", ", characterPositions)}]");
        Debug.Log($"Column lengths: [{string.Join(", ", characterColumnLengths)}]");
        
        // Update UI display
        UpdateWordDisplay();
    }

    private void UpdateWordDisplay()
    {
        if (currentWordDisplay != null && wordsLoaded && oxfordWords.Count > 0)
        {
            // Show the next word that will be used (since index advances after each call)
            int nextIndex = currentWordIndex; // This is now the next word to be used
            string nextWord = oxfordWords[nextIndex];
            currentWordDisplay.text = $"Next Word {nextIndex + 1}/{oxfordWords.Count}: {nextWord.ToUpper()}";
        }
    }

    public string GetCurrentWord()
    {
        if (wordsLoaded && oxfordWords.Count > 0)
        {
            // Get the word that was just used (previous index)
            int previousIndex = (currentWordIndex - 1 + oxfordWords.Count) % oxfordWords.Count;
            return oxfordWords[previousIndex];
        }
        return "";
    }

    public string GetNextWord()
    {
        if (wordsLoaded && oxfordWords.Count > 0)
        {
            return oxfordWords[currentWordIndex];
        }
        return "";
    }

    [ContextMenu("Next Word")]
    public void NextWord()
    {
        if (wordsLoaded && oxfordWords.Count > 0)
        {
            currentWordIndex = (currentWordIndex + 1) % oxfordWords.Count;
            Debug.Log($"Moved to word {currentWordIndex + 1}: '{oxfordWords[currentWordIndex]}'");
            UpdateWordDisplay();
        }
    }

    [ContextMenu("Previous Word")]
    public void PreviousWord()
    {
        if (wordsLoaded && oxfordWords.Count > 0)
        {
            currentWordIndex = (currentWordIndex - 1 + oxfordWords.Count) % oxfordWords.Count;
            Debug.Log($"Moved to word {currentWordIndex + 1}: '{oxfordWords[currentWordIndex]}'");
            UpdateWordDisplay();
        }
    }

    [ContextMenu("Random Word")]
    public void RandomWord()
    {
        if (wordsLoaded && oxfordWords.Count > 0)
        {
            currentWordIndex = Random.Range(0, oxfordWords.Count);
            Debug.Log($"Random word {currentWordIndex + 1}: '{oxfordWords[currentWordIndex]}'");
            UpdateWordDisplay();
        }
    }

    private void CreateCrosswordStyleGrid()
    {
        // Calculate grid bounds - find the maximum column length and total rows
        int maxColumns = Mathf.Max(characterColumnLengths);
        int totalRows = inputCharacters.Length;
        
        // Create a 2D array to track which cells should be created
        bool[,] shouldCreateCell = new bool[maxColumns, totalRows];
        
        // Fill the grid based on character positions and column lengths
        for (int charIndex = 0; charIndex < inputCharacters.Length; charIndex++)
        {
            int columnLength = characterColumnLengths[charIndex];
            int characterPosition = characterPositions[charIndex]; // Position within the row (1-9)
            
            // Calculate starting position for this character's row
            // Center the row around the middle of the grid
            int middleColumn = maxColumns / 2;
            int startColumn = middleColumn - (columnLength / 2);
            
            // Ensure we don't go out of bounds
            startColumn = Mathf.Clamp(startColumn, 0, maxColumns - columnLength);
            
            // Mark cells that should be created for this character's row
            for (int col = startColumn; col < startColumn + columnLength && col < maxColumns; col++)
            {
                shouldCreateCell[col, charIndex] = true;
            }
        }
        
        // Calculate grid size for centering
        float estimatedWidth = maxColumns * (cellSize.x + spacing.x);
        float estimatedHeight = totalRows * (cellSize.y + spacing.y);
        
        float startX = -estimatedWidth / 2f + cellSize.x / 2f;
        float startY = estimatedHeight / 2f - cellSize.y / 2f;
        
        // Create cells only where needed (crossword style)
        int cellCount = 0;
        for (int x = 0; x < maxColumns; x++)
        {
            for (int y = 0; y < totalRows; y++)
            {
                if (shouldCreateCell[x, y])
                {
                    // Check if this is a character cell or empty cell
                    int charIndex = y;
                    int columnLength = characterColumnLengths[charIndex];
                    int middleColumn = maxColumns / 2;
                    int startColumn = middleColumn - (columnLength / 2);
                    int characterColumn = startColumn + (columnLength / 2); // Center of the row
                    
                    // Use different prefab for character cells vs empty cells
                    GameObject prefabToUse = (x == characterColumn) ? cellPrefab : inputCellsPrefab;
                    GameObject cell = Instantiate(prefabToUse, transform);
                    cell.name = $"CrosswordCell_{x}_{y}_{((x == characterColumn) ? "CHAR" : "EMPTY")}";
                    
                    RectTransform rect = cell.GetComponent<RectTransform>();
                    rect.sizeDelta = cellSize;
                    
                    // Position cell
                    float posX = startX + x * (cellSize.x + spacing.x);
                    float posY = startY - y * (cellSize.y + spacing.y);
                    rect.anchoredPosition = new Vector2(posX, posY);
                    
                    // Add character text to character cells only
                    if (x == characterColumn)
                    {
                        TextMeshProUGUI textComponent = cell.GetComponentInChildren<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            textComponent.text = inputCharacters[charIndex].ToString().ToUpper();
                            textComponent.color = Color.black;
                            textComponent.fontSize = 24;
                        }
                    }
                    
                    cellCount++;
                }
            }
        }
        
        Debug.Log($"Created {cellCount} crossword-style cells across {maxColumns}x{totalRows} potential grid");
        Debug.Log($"Word layout: Vertical word with each character centered in its row, surrounded by horizontal extensions");
    }

    [System.Serializable]
    private class OxfordWordsData
    {
        public string[] words;
    }
}

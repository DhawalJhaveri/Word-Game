using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordFilter : MonoBehaviour
{
    [Header("File Paths")]
    [SerializeField] private string inputFilePath = "Assets/grid words/oxford_words.json";
    [SerializeField] private string outputFilePath = "Assets/grid words/oxford_words_filtered.json";
    
    [Header("Filter Settings")]
    [SerializeField] private int minWordLength = 4;
    [SerializeField] private int maxWordLength = 6;

    /*private void Start()
    {
        FilterWords();
    }*/

    [ContextMenu("Filter Words")]
    public void FilterWords()
    {
        try
        {
            // Read the original JSON file
            string jsonContent = File.ReadAllText(inputFilePath);

            // Parse the JSON to get the words array
            WordData wordData = JsonUtility.FromJson<WordData>(jsonContent);

            // Filter words based on length and remove duplicates while preserving order
            List<string> filteredWords = new List<string>();
            HashSet<string> seenWords = new HashSet<string>();

            foreach (string word in wordData.words)
            {
                if (word.Length >= minWordLength && word.Length <= maxWordLength && !seenWords.Contains(word))
                {
                    filteredWords.Add(word);
                    seenWords.Add(word);
                }
            }

            // Create new WordData with filtered words
            WordData filteredData = new WordData();
            filteredData.words = filteredWords.ToArray();

            // Convert back to JSON
            string filteredJson = JsonUtility.ToJson(filteredData, true);

            // Write to new file
            File.WriteAllText(outputFilePath, filteredJson);

            Debug.Log($"Filtered words: {filteredWords.Count} unique words kept out of {wordData.words.Length} total words");
            Debug.Log($"Filtered file saved to: {outputFilePath}");

            // Log some examples of filtered words
            Debug.Log("Sample filtered words:");
            for (int i = 0; i < Mathf.Min(10, filteredWords.Count); i++)
            {
                Debug.Log($"  {filteredWords[i]} ({filteredWords[i].Length} letters)");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error filtering words: {e.Message}");
        }
    }

    [System.Serializable]
    private class WordData
    {
        public string[] words;
    }
}

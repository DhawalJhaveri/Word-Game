using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    // Events for GridGenerator2D to subscribe
    public static event Action<char> OnKeyPressedEvent;
    public static event Action OnBackspaceEvent;

    [Header("UI References")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject backspacePrefab;
    [SerializeField] private GameObject submitPrefab;
    [SerializeField] private GameObject rowPrefab;

    [Header("Visual Settings")]
    [SerializeField] private Sprite backspaceSprite;
    [SerializeField] private int spacing = 10;

    private string currentWord = string.Empty;

    // Keyboard layout (QWERTY)
    private readonly List<string> keyboardRows = new()
    {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
    };

    private void Start()
    {
        BuildKeyboard();
    }

    private void BuildKeyboard()
    {
        float totalWidth = GetComponent<RectTransform>().rect.width;
        float keyWidth = (totalWidth - (keyboardRows[0].Length - 1) * spacing) / keyboardRows[0].Length;

        // Letter rows
        foreach (string row in keyboardRows)
        {
            var rowTransform = Instantiate(rowPrefab, transform).transform;

            foreach (char letter in row)
            {
                var keyGO = Instantiate(keyPrefab, rowTransform);
                keyGO.name = $"{letter}_BTN";
                keyGO.GetComponentInChildren<TMP_Text>().text = letter.ToString();
                keyGO.GetComponent<RectTransform>().sizeDelta = new Vector2(keyWidth, keyWidth);

                keyGO.GetComponent<Button>().onClick.AddListener(() => HandleKeyPress(letter));
            }
        }

        // Backspace key
        var backspaceGO = Instantiate(backspacePrefab, transform);
        backspaceGO.name = "Backspace_BTN";
        backspaceGO.GetComponent<RectTransform>().sizeDelta = new Vector2(keyWidth * 1.5f, keyWidth);
        if (backspaceSprite != null)
            backspaceGO.GetComponent<Image>().sprite = backspaceSprite;
        backspaceGO.GetComponent<Button>().onClick.AddListener(HandleBackspacePress);
    }

    #region Input Handlers
    private void HandleKeyPress(char c)
    {
        // Add char to currentWord
        currentWord += c;

        // Notify grid
        OnKeyPressedEvent?.Invoke(c);
    }

    private void HandleBackspacePress()
    {
        if (string.IsNullOrEmpty(currentWord)) return;

        // Remove last char
        currentWord = currentWord[..^1];

        // Notify grid
        OnBackspaceEvent?.Invoke();
    }
    #endregion
}

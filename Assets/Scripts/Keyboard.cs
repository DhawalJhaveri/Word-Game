using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Keyboard : MonoBehaviour
{
    // Events for GridGenerator2D to subscribe
    public static event Action<char> OnKeyPressedEvent;
    public static event Action OnBackspaceEvent;
    public static event Action OnSubmitEvent;

    [Header("UI References")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject backspacePrefab;
    [SerializeField] private GameObject submitPrefab;
    [SerializeField] private GameObject rowPrefab;

    [Header("Visual Settings")]
    [SerializeField] private Sprite backspaceSprite;
    [SerializeField] private Sprite submitSprite;
    [SerializeField] private Vector2 keySize = new(100, 100);
    [SerializeField] private int spacing = 10;

    [Header("Layout Settings")]
    [SerializeField] private int paddingLeft = 40;
    [SerializeField] private int paddingRight = 40;
    [SerializeField] private int paddingTop = 40;
    [SerializeField] private int paddingBottom = 80;
    [SerializeField] private int rowSpacing = 10;

    private string currentWord = string.Empty;

    private readonly List<string> keyboardRows = new()
    {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
    };

    private RectTransform container; // Parent for all rows and submit button

    private void Start()
    {
        BuildKeyboard();
    }

    private void BuildKeyboard()
    {
        // Clear old keyboard if exists
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Create container for proper padding and safe area handling
        container = new GameObject("KeyboardContainer", typeof(RectTransform), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
        container.SetParent(transform, false);
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 1);
        container.pivot = new Vector2(0.5f, 0);
        container.offsetMin = Vector2.zero;
        container.offsetMax = Vector2.zero;

        var vLayout = container.GetComponent<VerticalLayoutGroup>();
        vLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        vLayout.spacing = rowSpacing;
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.childForceExpandWidth = false;
        vLayout.childForceExpandHeight = false;

        // Build all letter rows
        for (int rowIndex = 0; rowIndex < keyboardRows.Count; rowIndex++)
        {
            string row = keyboardRows[rowIndex];

            GameObject rowGO = new GameObject($"Row_{rowIndex}", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            rowGO.transform.SetParent(container, false);

            var hLayout = rowGO.GetComponent<HorizontalLayoutGroup>();
            hLayout.spacing = spacing;
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            hLayout.childControlWidth = false;
            hLayout.childControlHeight = false;
            hLayout.childForceExpandWidth = false;
            hLayout.childForceExpandHeight = false;

            foreach (char letter in row)
            {
                var keyGO = Instantiate(keyPrefab, rowGO.transform);
                keyGO.name = $"{letter}_BTN";
                keyGO.GetComponentInChildren<TMP_Text>().text = letter.ToString();
                keyGO.GetComponent<RectTransform>().sizeDelta = keySize;
                keyGO.GetComponent<Button>().onClick.AddListener(() => HandleKeyPress(letter));
            }

            // Add Backspace only to the last row
            if (rowIndex == keyboardRows.Count - 1)
            {
                var backspaceGO = Instantiate(backspacePrefab, rowGO.transform);
                backspaceGO.name = "Backspace_BTN";
                backspaceGO.GetComponent<RectTransform>().sizeDelta = new Vector2(keySize.x * 1.5f, keySize.y);
                if (backspaceSprite != null)
                    backspaceGO.GetComponent<Image>().sprite = backspaceSprite;

                backspaceGO.GetComponent<Button>().onClick.AddListener(HandleBackspacePress);
            }
        }

        // Add Submit Button after all rows
        CreateSubmitButton();
    }

    private void CreateSubmitButton()
    {
        var submitGO = Instantiate(submitPrefab, container);
        submitGO.name = "Submit_BTN";

        var rt = submitGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(keySize.x * 2f, keySize.y * 1.1f);

        if (submitSprite != null)
            submitGO.GetComponent<Image>().sprite = submitSprite;

        var btn = submitGO.GetComponent<Button>();
        btn.onClick.AddListener(HandleSubmitPress);

        Debug.Log("[Keyboard] Submit button instantiated below keyboard with safe padding.");
    }

    #region Input Handlers
    private void HandleKeyPress(char c)
    {
        Debug.Log($"[Keyboard] Virtual key pressed: {c}");
        currentWord += c;
        OnKeyPressedEvent?.Invoke(c);
    }

    private void HandleBackspacePress()
    {
        Debug.Log("[Keyboard] Backspace pressed");

        if (!string.IsNullOrEmpty(currentWord))
            currentWord = currentWord[..^1];

        OnBackspaceEvent?.Invoke();
    }

    private void HandleSubmitPress()
    {
        Debug.Log("[Keyboard] Submit pressed -> requesting next word");
        currentWord = string.Empty;
        OnSubmitEvent?.Invoke();
    }
    #endregion
}

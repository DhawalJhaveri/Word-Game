using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public static Action<char> add_word;
    [SerializeField] TMP_InputField InputField_CurrentWord;
    [SerializeField] GameObject keyPrefab;
    [SerializeField] GameObject keyPrefab_BackSpace;
    [SerializeField] GameObject keyPrefab_Submit;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] Sprite BackSpaceSprite;
    [SerializeField] int Spacing = 10;
    private Button SubmitKey;
    readonly List<string> keysStrings = new() {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
    };
    Transform RowGO = null;
    string _Word = string.Empty;

    string CurrentWord
    {
        get => _Word;
        set 
        {
            //InputField_CurrentWord.GetComponent<ChatInputTaker>().AddTextToInput(_Word = value);

            //this.Log("input length = " + value.Length);
            if(SubmitKey == null)
                return;
            if (value.Length < 2)
            {
                //SubmitKey.interactable = false;
                //this.Log("check");
            }
            else
            {
                //SubmitKey.interactable = true;
                //this.Log("checked");
            }
        }
    }

    private void OnEnable()
    {
        add_word += OnKeyPressed;


    }
    public void OnDisable()
    {
        add_word -= OnKeyPressed;
        CurrentWord = string.Empty;
    }

    void Start() 
    {
        //yield return new WaitUntil(() => UIManager.Instance.firstWordDone == true);
        GenerateKeyboard(); 
    } 

    void GenerateKeyboard()
    {
        var TotalWidth = transform.GetComponent<RectTransform>().rect.size.x;
        var FirstStringLength = keysStrings[0].Length;
        var KeyWidth = (TotalWidth - (FirstStringLength - 1) * Spacing) / FirstStringLength;
        foreach (var row in keysStrings)
        {
            RowGO = Instantiate(rowPrefab, transform).transform;
            foreach (var key in row)
            {
                var KeyGO = Instantiate(keyPrefab, RowGO);
                KeyGO.name = key + "_BTN";
                KeyGO.GetComponentInChildren<TMP_Text>().text = key.ToString();
                KeyGO.GetComponent<RectTransform>().sizeDelta = new Vector2(KeyWidth, KeyWidth);
                KeyGO.GetComponent<Button>().onClick.AddListener(() => OnKeyPressed(key));
            }
        }
        //back space
        var Backspace = Instantiate(keyPrefab_BackSpace, RowGO);
        Backspace.name = "BackSpace_BTN";
        Backspace.GetComponent<RectTransform>().sizeDelta = new Vector2(KeyWidth, KeyWidth);
        Backspace.GetComponent<Button>().onClick.AddListener(() => BackSpace());
        //submit button
        RowGO = Instantiate(rowPrefab, transform).transform;
        GameObject SKey = Instantiate(keyPrefab_Submit, RowGO);
        SKey.name = "Submit_BTN";
        SKey.GetComponent<RectTransform>().sizeDelta = new Vector2(KeyWidth * 7, KeyWidth * 2);
        SubmitKey = SKey.GetComponent<Button>();
            SubmitKey.onClick.AddListener(() => ReturnOrSubmit());
        //this.Log("Keyboard Generation completed");
    }

    void OnKeyPressed(char s) 
    {
        //SoundManager.Instance.PlaySFX();
        //SoundManager.Instance.PlayHapticFeedback();

        CurrentWord += s;
    }

    void BackSpace()
    {
        //SoundManager.Instance.PlaySFX();
        //SoundManager.Instance.PlayHapticFeedback();

        if (string.IsNullOrEmpty(CurrentWord) || !(CurrentWord.Length>1)) return;
        CurrentWord = CurrentWord[..^1];
    }

    void ReturnOrSubmit()
    {
        //SoundManager.Instance.PlaySFX();
        //SoundManager.Instance.PlayHapticFeedback();

        //this.Log(CurrentWord);
        //UIManager.Instance.PlayGame(CurrentWord);
        CurrentWord = string.Empty;
    }
}
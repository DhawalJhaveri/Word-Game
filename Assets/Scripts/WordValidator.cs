using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class WordValidator : MonoBehaviour
{
    public static WordValidator Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text streak_Timer_Text;

    [Header("Visual Settings")]
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color loseColor = Color.red;
    [SerializeField] private Color streakColor = Color.blue;

    private const string DICTIONARY_API = "https://api.dictionaryapi.dev/api/v2/entries/en/";
    private const string STREAK_KEY = "WinStreak";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    /// <summary>
    /// Validates a list of words using the free dictionary API.
    /// If even one word is invalid, the player loses immediately.
    /// </summary>
    public void ValidateWords(List<string> words)
    {
        StartCoroutine(ValidateWordsCoroutine(words));
    }

    private IEnumerator ValidateWordsCoroutine(List<string> words)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        resultText.text = "Checking words...";
        resultText.color = Color.white;

        for (int i = 0; i < words.Count; i++)
        {
            string word = words[i];

            if (string.IsNullOrWhiteSpace(word))
                continue;

            Debug.Log($"[Validator] Checking word: {word}");

            using (UnityWebRequest request = UnityWebRequest.Get(DICTIONARY_API + word.ToLower()))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[Validator] Error checking '{word}': {request.error}");
                    DisplayLose(word, "Connection error");
                    yield break;
                }

                if (!request.downloadHandler.text.Contains("\"word\""))
                {
                    Debug.Log($"[Validator] Word '{word}' is NOT valid.");
                    DisplayLose(word, "Invalid word");
                    yield break;
                }
                else
                {
                    Debug.Log($"[Validator] Word '{word}' is valid.");
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        DisplayWin();
    }

    private void DisplayWin()
    {
        int currentStreak = PlayerPrefs.GetInt(STREAK_KEY, 0);
        currentStreak++;
        PlayerPrefs.SetInt(STREAK_KEY, currentStreak);
        PlayerPrefs.Save();

        //resultText.text = $"All words valid.\nYou WIN!\nCurrent Streak: {currentStreak}";
        resultText.color = winColor;

        Debug.Log($"[Validator] Player wins! All words valid. Current streak: {currentStreak}");

        float bestTime = GameTimer.Instance.GetBestTime();
        float lastTime = GameTimer.Instance.GetAllTimes().Count > 0 ?
                         GameTimer.Instance.GetAllTimes()[^1] : 0f;

        resultText.text = $"All words valid.\nYou WIN!\n" +
                          $"Current Streak: {currentStreak}\n" +
                          $"Your Time: {GameTimer.Instance.FormatTime(lastTime)}\n" +
                          $"Best Time: {GameTimer.Instance.FormatTime(bestTime)}";
        streak_Timer_Text.color = streakColor;

    }

    private void DisplayLose(string invalidWord, string reason)
    {
        PlayerPrefs.SetInt(STREAK_KEY, 0);
        PlayerPrefs.Save();

        resultText.text = $"'{invalidWord.ToUpper()}' is not a valid word.\nYou LOSE!\nStreak reset to 0.";
        //resultText.transform.position = Vector2.zero;
        resultText.color = loseColor;
        streak_Timer_Text.text = "";
        Debug.Log($"[Validator] Player loses — Invalid word: {invalidWord} ({reason}). Streak reset.");
    }
}

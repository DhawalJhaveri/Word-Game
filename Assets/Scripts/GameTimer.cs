using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private List<float> completionTimes = new List<float>();
    private const string TIMES_KEY = "CompletionTimes";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadTimes();
        UpdateTimerDisplay(0f);
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay(elapsedTime);
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay(0f);
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timerText == null) return;

        string formatted = FormatTime(time);
        timerText.text = formatted;
    }

    /// <summary>
    /// Formats the time with hours shown only when needed.
    /// </summary>
    public string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        if (hours > 0)
            return $"{hours:0}:{minutes:00}:{seconds:00}.{milliseconds / 10:00}";
        else
            return $"{minutes:00}:{seconds:00}.{milliseconds / 10:00}";
    }

    /// <summary>
    /// Called when the player completes all words successfully.
    /// </summary>
    public void SaveCompletionTime()
    {
        completionTimes.Add(elapsedTime);
        SaveTimes();
    }

    private void SaveTimes()
    {
        string serialized = string.Join(",", completionTimes);
        PlayerPrefs.SetString(TIMES_KEY, serialized);
        PlayerPrefs.Save();
    }

    private void LoadTimes()
    {
        completionTimes.Clear();
        if (PlayerPrefs.HasKey(TIMES_KEY))
        {
            string serialized = PlayerPrefs.GetString(TIMES_KEY);
            string[] times = serialized.Split(',');
            foreach (string t in times)
            {
                if (float.TryParse(t, out float val))
                    completionTimes.Add(val);
            }
        }
    }

    public float GetBestTime()
    {
        if (completionTimes.Count == 0)
            return 0f;
        return Mathf.Min(completionTimes.ToArray());
    }

    public List<float> GetAllTimes()
    {
        return new List<float>(completionTimes);
    }
}

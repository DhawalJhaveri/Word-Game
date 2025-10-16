using UnityEngine;
using UnityEngine.Events;
using distriqt.plugins.vibration;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource win;
    [SerializeField] private AudioSource lose;
    [SerializeField] private AudioSource tick;
    [SerializeField] private AudioClip tickClip;

    [HideInInspector] public bool isMusicOn = true;
    [HideInInspector] public bool isSFXOn = true;
    [HideInInspector] public bool isHapticsOn = true;

    //  Events to notify UI toggles
    [HideInInspector] public UnityEvent<bool> OnMusicToggleChanged = new UnityEvent<bool>();
    [HideInInspector] public UnityEvent<bool> OnSFXToggleChanged = new UnityEvent<bool>();
    [HideInInspector] public UnityEvent<bool> OnHapticsToggleChanged = new UnityEvent<bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load preferences
        isMusicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        isHapticsOn = PlayerPrefs.GetInt("Haptics", 1) == 1;

        UpdateAudioStates();
    }

    public void SetMusic(bool state)
    {
        isMusicOn = state;
        PlayerPrefs.SetInt("Music", state ? 1 : 0);
        UpdateAudioStates();

        // Notify all UI panels
        OnMusicToggleChanged.Invoke(state);
    }

    public void SetSFX(bool state)
    {
        isSFXOn = state;
        PlayerPrefs.SetInt("SFX", state ? 1 : 0);
        UpdateAudioStates();

        //  Notify all UI panels
        OnSFXToggleChanged.Invoke(state);
    }

    public void SetHaptics(bool state)
    {
        isHapticsOn = state;
        PlayerPrefs.SetInt("Haptics", state ? 1 : 0);
        OnHapticsToggleChanged.Invoke(state);
    }

    private void UpdateAudioStates()
    {
        if (musicSource != null)
        {
            musicSource.mute = !isMusicOn;
        }

        if (sfxSource != null)
        {
            sfxSource.mute = !isSFXOn;
        }
    }

    public void PlaySFX()
    {
        if (isSFXOn && sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayHapticFeedback()
    {
        if (isHapticsOn)
        {
            Vibration.Instance.Vibrate(75);
        }
    }

    public void PlayWINLOSE(bool res)
    {
        if (res == true)
            win.Play();
        else
            lose.Play();
    }

    public void PlayTickSound()
    {
        // If you have an AudioSource assigned
        if (tick != null && tickClip != null)
        {
            tick.PlayOneShot(tickClip);
        }
    }

    public void PlayForcedTurnChangeHaptic()
    {
        Vibration.Instance.Vibrate(150);
    }
}

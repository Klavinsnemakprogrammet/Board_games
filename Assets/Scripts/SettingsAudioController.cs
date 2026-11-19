using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider cloudSpeedSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button skipSongButton;
    [SerializeField] private Button previousSongButton;
    [SerializeField] private MusicScript musicPlayer;

    private List<DisplayResolution> availableResolutions;

    private void Start()
    {
        if (musicSlider == null)
        {
            Debug.LogError("The music volume slider has not been assigned.", this);
        }
        else
        {
            musicSlider.SetValueWithoutNotify(GameAudioSettings.MusicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider == null)
        {
            Debug.LogError("The SFX volume slider has not been assigned.", this);
        }
        else
        {
            sfxSlider.SetValueWithoutNotify(GameAudioSettings.SfxVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        if (cloudSpeedSlider == null)
        {
            Debug.LogError("The cloud speed slider has not been assigned.", this);
        }
        else
        {
            cloudSpeedSlider.SetValueWithoutNotify(GameCloudSettings.Speed);
            cloudSpeedSlider.onValueChanged.AddListener(SetCloudSpeed);
        }

        InitializeResolutionDropdown();
        InitializeQualityDropdown();

        if (musicPlayer == null)
        {
            Debug.LogError("The background music player has not been assigned.", this);
        }
        else
        {
            musicPlayer.SetVolume(GameAudioSettings.MusicVolume);
        }

        if (skipSongButton == null)
        {
            Debug.LogError("The skip song button has not been assigned.", this);
        }
        else
        {
            skipSongButton.onClick.AddListener(SkipSong);
        }

        if (previousSongButton == null)
        {
            Debug.LogError("The previous song button has not been assigned.", this);
        }
        else
        {
            previousSongButton.onClick.AddListener(PlayPreviousSong);
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }

        if (cloudSpeedSlider != null)
        {
            cloudSpeedSlider.onValueChanged.RemoveListener(SetCloudSpeed);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(SetDisplayResolution);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.RemoveListener(SetGraphicsQuality);
        }

        if (skipSongButton != null)
        {
            skipSongButton.onClick.RemoveListener(SkipSong);
        }

        if (previousSongButton != null)
        {
            previousSongButton.onClick.RemoveListener(PlayPreviousSong);
        }

        GameAudioSettings.Save();
    }

    private void SetMusicVolume(float volume)
    {
        GameAudioSettings.SetMusicVolume(volume);
        if (musicPlayer != null)
        {
            musicPlayer.SetVolume(volume);
        }
    }

    private void SetSfxVolume(float volume)
    {
        GameAudioSettings.SetSfxVolume(volume);
        Soundeffects[] soundEffects = FindObjectsByType<Soundeffects>(FindObjectsSortMode.None);
        foreach (Soundeffects soundEffect in soundEffects)
        {
            soundEffect.SetVolume(volume);
        }
    }

    private void SetCloudSpeed(float speed)
    {
        GameCloudSettings.SetSpeed(speed);
    }

    private void InitializeQualityDropdown()
    {
        if (qualityDropdown == null)
        {
            Transform qualityGroup = transform.Find("Quality");
            if (qualityGroup != null)
            {
                qualityDropdown = qualityGroup.GetComponentInChildren<TMP_Dropdown>(true);
            }
        }

        if (qualityDropdown == null)
        {
            Debug.LogError(
                "No TextMeshPro quality dropdown is assigned or found under the 'Quality' group.",
                this);
            return;
        }

        List<string> qualityOptions = GameGraphicsSettings.GetQualityOptions();
        if (qualityOptions.Count == 0)
        {
            Debug.LogError("No Unity quality levels are configured for this platform.", this);
            return;
        }

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.SetValueWithoutNotify(GameGraphicsSettings.QualityIndex);
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetGraphicsQuality);
    }

    private void SetGraphicsQuality(int selectedIndex)
    {
        GameGraphicsSettings.SetQualityIndex(selectedIndex);
    }

    private void InitializeResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            Transform resolutionGroup = transform.Find("Resolution");
            if (resolutionGroup != null)
            {
                resolutionDropdown = resolutionGroup.GetComponentInChildren<TMP_Dropdown>(true);
            }
        }

        if (resolutionDropdown == null)
        {
            Debug.LogError(
                "No TextMeshPro resolution dropdown is assigned or found under the 'Resolution' group.",
                this);
            return;
        }

        availableResolutions = GetAvailableResolutions();
        if (availableResolutions.Count == 0)
        {
            Debug.LogError("No valid screen resolutions are available.", this);
            return;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(availableResolutions
            .Select(resolution => $"{resolution.Width} x {resolution.Height}")
            .ToList());

        int selectedWidth = Screen.width;
        int selectedHeight = Screen.height;
        if (GameResolutionSettings.TryGetSavedResolution(out int savedWidth, out int savedHeight))
        {
            selectedWidth = savedWidth;
            selectedHeight = savedHeight;
        }

        int selectedIndex = FindResolutionIndex(selectedWidth, selectedHeight);
        if (selectedIndex < 0)
        {
            selectedIndex = FindResolutionIndex(Screen.width, Screen.height);
        }

        resolutionDropdown.SetValueWithoutNotify(Mathf.Max(selectedIndex, 0));
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetDisplayResolution);
    }

    private static List<DisplayResolution> GetAvailableResolutions()
    {
        var resolutions = new List<DisplayResolution>();
        foreach (Resolution resolution in Screen.resolutions)
        {
            AddResolution(resolutions, resolution.width, resolution.height);
        }

        if (GameResolutionSettings.TryGetSavedResolution(out int savedWidth, out int savedHeight))
        {
            AddResolution(resolutions, savedWidth, savedHeight);
        }

        AddResolution(resolutions, Screen.width, Screen.height);
        resolutions.Sort((left, right) =>
        {
            int widthOrder = left.Width.CompareTo(right.Width);
            return widthOrder != 0 ? widthOrder : left.Height.CompareTo(right.Height);
        });
        return resolutions;
    }

    private static void AddResolution(List<DisplayResolution> resolutions, int width, int height)
    {
        if (width <= 0 || height <= 0 ||
            resolutions.Any(resolution => resolution.Width == width && resolution.Height == height))
        {
            return;
        }

        resolutions.Add(new DisplayResolution(width, height));
    }

    private int FindResolutionIndex(int width, int height)
    {
        return availableResolutions.FindIndex(resolution =>
            resolution.Width == width && resolution.Height == height);
    }

    private void SetDisplayResolution(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= availableResolutions.Count)
        {
            Debug.LogError("The selected resolution is not in the available resolution list.", this);
            return;
        }

        DisplayResolution resolution = availableResolutions[selectedIndex];
        GameResolutionSettings.SetResolution(resolution.Width, resolution.Height);
    }

    private void SkipSong()
    {
        if (musicPlayer != null)
        {
            musicPlayer.SkipSong();
        }
    }

    private void PlayPreviousSong()
    {
        if (musicPlayer != null)
        {
            musicPlayer.PlayPreviousSong();
        }
    }

    private readonly struct DisplayResolution
    {
        public readonly int Width;
        public readonly int Height;

        public DisplayResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}

public static class GameGraphicsSettings
{
    private const string QualityPreferenceKey = "GraphicsQuality";

    public static int QualityIndex
    {
        get
        {
            int qualityCount = QualitySettings.names.Length;
            if (qualityCount == 0)
            {
                return -1;
            }

            return Mathf.Clamp(
                PlayerPrefs.GetInt(QualityPreferenceKey, QualitySettings.GetQualityLevel()),
                0,
                qualityCount - 1);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedQuality()
    {
        int qualityIndex = QualityIndex;
        if (qualityIndex >= 0)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }
        else
        {
            Debug.LogError("No Unity quality levels are configured.");
        }
    }

    public static List<string> GetQualityOptions()
    {
        return new List<string>(QualitySettings.names);
    }

    public static void SetQualityIndex(int qualityIndex)
    {
        string[] qualityNames = QualitySettings.names;
        if (qualityIndex < 0 || qualityIndex >= qualityNames.Length)
        {
            Debug.LogError(
                "The selected graphics quality is not in the Unity quality levels available for this platform.");
            return;
        }

        PlayerPrefs.SetInt(QualityPreferenceKey, qualityIndex);
        PlayerPrefs.Save();
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}

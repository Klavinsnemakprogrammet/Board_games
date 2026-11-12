using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider cloudSpeedSlider;
    [SerializeField] private Dropdown resolutionDropdown;
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

    private void InitializeResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            resolutionDropdown = FindObjectsByType<Dropdown>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(dropdown =>
                    dropdown.transform.parent != null &&
                    dropdown.transform.parent.name == "Resolution");
        }

        if (resolutionDropdown == null)
        {
            Debug.LogError(
                "No Unity UI Dropdown is assigned or found under the 'Resolution' group.",
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

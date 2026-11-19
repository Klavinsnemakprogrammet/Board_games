using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(GameGraphicsSettings.GetQualityOptions());
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
    private const int QualityOptionCount = 3;
    private static UniversalRenderPipelineAsset runtimePipelineAsset;

    public static int QualityIndex =>
        Mathf.Clamp(PlayerPrefs.GetInt(QualityPreferenceKey, 0), 0, QualityOptionCount - 1);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedQuality()
    {
        ApplyQuality(QualityIndex);
    }

    public static List<string> GetQualityOptions()
    {
        return new List<string> { "Great", "OK", "Potato" };
    }

    public static void SetQualityIndex(int qualityIndex)
    {
        if (qualityIndex < 0 || qualityIndex >= QualityOptionCount)
        {
            Debug.LogError("The selected graphics quality is not in the available quality list.");
            return;
        }

        PlayerPrefs.SetInt(QualityPreferenceKey, qualityIndex);
        PlayerPrefs.Save();
        ApplyQuality(qualityIndex);
    }

    private static void ApplyQuality(int qualityIndex)
    {
        switch (qualityIndex)
        {
            case 0:
                QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                QualitySettings.shadowDistance = 80f;
                QualitySettings.antiAliasing = 4;
                QualitySettings.lodBias = 2f;
                QualitySettings.maximumLODLevel = 0;
                QualitySettings.vSyncCount = 1;
                ApplyPipelineQuality(1.25f, 4, 100f, 4096, 4);
                break;
            case 1:
                QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
                QualitySettings.shadowDistance = 40f;
                QualitySettings.antiAliasing = 2;
                QualitySettings.lodBias = 1f;
                QualitySettings.maximumLODLevel = 0;
                QualitySettings.vSyncCount = 1;
                ApplyPipelineQuality(0.8f, 2, 35f, 1024, 2);
                break;
            case 2:
                QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
                QualitySettings.shadowDistance = 0f;
                QualitySettings.antiAliasing = 0;
                QualitySettings.lodBias = 0.5f;
                QualitySettings.maximumLODLevel = 1;
                QualitySettings.vSyncCount = 0;
                ApplyPipelineQuality(0.5f, 1, 0f, 256, 1);
                break;
        }
    }

    private static void ApplyPipelineQuality(
        float renderScale,
        int msaaSamples,
        float shadowDistance,
        int shadowMapResolution,
        int shadowCascadeCount)
    {
        if (runtimePipelineAsset == null)
        {
            UniversalRenderPipelineAsset activePipeline =
                GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (activePipeline == null)
            {
                Debug.LogError(
                    "Graphics quality profiles require an active Universal Render Pipeline asset.");
                return;
            }

            runtimePipelineAsset = Object.Instantiate(activePipeline);
            runtimePipelineAsset.hideFlags = HideFlags.HideAndDontSave;
            QualitySettings.renderPipeline = runtimePipelineAsset;
        }

        runtimePipelineAsset.renderScale = renderScale;
        runtimePipelineAsset.msaaSampleCount = msaaSamples;
        runtimePipelineAsset.shadowDistance = shadowDistance;
        runtimePipelineAsset.mainLightShadowmapResolution = shadowMapResolution;
        runtimePipelineAsset.shadowCascadeCount = shadowCascadeCount;
    }
}

using UnityEngine;

public static class GameResolutionSettings
{
    private const string WidthKey = "ScreenResolutionWidth";
    private const string HeightKey = "ScreenResolutionHeight";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedResolution()
    {
        if (!TryGetSavedResolution(out int width, out int height))
        {
            return;
        }

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Ignoring an invalid saved screen resolution.");
            return;
        }

        Screen.SetResolution(width, height, GetResolutionMode());
    }

    public static bool TryGetSavedResolution(out int width, out int height)
    {
        width = PlayerPrefs.GetInt(WidthKey, 0);
        height = PlayerPrefs.GetInt(HeightKey, 0);
        return PlayerPrefs.HasKey(WidthKey) && PlayerPrefs.HasKey(HeightKey);
    }

    public static void SetResolution(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Screen resolution dimensions must be positive.");
            return;
        }

        PlayerPrefs.SetInt(WidthKey, width);
        PlayerPrefs.SetInt(HeightKey, height);
        PlayerPrefs.Save();
        Screen.SetResolution(width, height, GetResolutionMode());
    }

    private static FullScreenMode GetResolutionMode()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow ||
            Screen.fullScreenMode == FullScreenMode.MaximizedWindow)
        {
            return FullScreenMode.Windowed;
        }

        return Screen.fullScreenMode;
    }
}

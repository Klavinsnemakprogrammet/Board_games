using UnityEngine;

public static class GameAudioSettings
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    public static float MusicVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
    public static float SfxVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));

    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(volume));
    }

    public static void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(volume));
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}

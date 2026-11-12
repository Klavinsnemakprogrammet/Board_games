using UnityEngine;

public static class GameCloudSettings
{
    private const string SpeedKey = "CloudSpeed";

    public static float Speed => Mathf.Clamp01(PlayerPrefs.GetFloat(SpeedKey, 0.4f));

    public static void SetSpeed(float speed)
    {
        PlayerPrefs.SetFloat(SpeedKey, Mathf.Clamp01(speed));
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button skipSongButton;
    [SerializeField] private Button previousSongButton;
    [SerializeField] private MusicScript musicPlayer;

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
}

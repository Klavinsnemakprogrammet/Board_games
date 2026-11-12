using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;

    private AudioSource musicSource;
    private int currentSongIndex = -1;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.loop = false;
        SetVolume(GameAudioSettings.MusicVolume);
    }

    private void Start()
    {
        PlayNextSong();
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayNextSong();
        }
    }

    public void SetVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SkipSong()
    {
        PlayNextSong();
    }

    public void PlayPreviousSong()
    {
        PlaySong(-1);
    }

    private void PlayNextSong()
    {
        PlaySong(1);
    }

    private void PlaySong(int direction)
    {
        if (songs == null || songs.Length == 0)
        {
            Debug.LogWarning("No background songs have been assigned.", this);
            enabled = false;
            return;
        }

        if (direction < 0 && currentSongIndex < 0)
        {
            currentSongIndex = 0;
        }

        for (int i = 0; i < songs.Length; i++)
        {
            currentSongIndex = (currentSongIndex + direction + songs.Length) % songs.Length;
            if (songs[currentSongIndex] == null)
            {
                continue;
            }

            musicSource.clip = songs[currentSongIndex];
            musicSource.Play();
            return;
        }

        Debug.LogWarning("The background song list contains no valid audio clips.", this);
        enabled = false;
    }
}

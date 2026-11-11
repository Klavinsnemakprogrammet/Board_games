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
    }

    void Start()
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

    private void PlayNextSong()
    {
        if (songs == null || songs.Length == 0)
        {
            Debug.LogWarning("No background songs have been assigned.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < songs.Length; i++)
        {
            currentSongIndex = (currentSongIndex + 1) % songs.Length;
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

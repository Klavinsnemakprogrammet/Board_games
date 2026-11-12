using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Soundeffects : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource effectsSource;

    private void Awake()
    {
        if (effectsSource == null)
        {
            effectsSource = GetComponent<AudioSource>();
            if (effectsSource == null)
            {
                effectsSource = gameObject.AddComponent<AudioSource>();
            }

            effectsSource.playOnAwake = false;
            effectsSource.spatialBlend = 0f;
        }

        SetVolume(GameAudioSettings.SfxVolume);
        AttachButtonSoundEffects();
    }

    public void Hover()
    {
        PlayEffect(0);
    }

    public void Click()
    {
        PlayEffect(1);
    }

    public void OnDice()
    {
        PlayEffect(2);
    }

    public void SetVolume(float volume)
    {
        if (effectsSource != null)
        {
            effectsSource.volume = Mathf.Clamp01(volume);
        }
    }

    private void PlayEffect(int clipIndex)
    {
        if (effectsSource == null)
        {
            Debug.LogError("An effects AudioSource has not been assigned.", this);
            return;
        }

        if (audioClips == null || clipIndex >= audioClips.Length || audioClips[clipIndex] == null)
        {
            Debug.LogWarning($"No sound effect is assigned for index {clipIndex}.", this);
            return;
        }

        effectsSource.PlayOneShot(audioClips[clipIndex]);
    }

    private void AttachButtonSoundEffects()
    {
        foreach (Button button in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            if (eventTrigger.triggers == null)
            {
                eventTrigger.triggers = new List<EventTrigger.Entry>();
            }

            if (!HasSoundEvent(eventTrigger, EventTriggerType.PointerEnter, nameof(Hover)))
            {
                AddButtonEvent(eventTrigger, EventTriggerType.PointerEnter, _ => Hover());
            }

            if (!HasSoundEvent(eventTrigger, EventTriggerType.PointerDown, nameof(Click)))
            {
                AddButtonEvent(eventTrigger, EventTriggerType.PointerDown, _ => Click());
            }
        }
    }

    private bool HasSoundEvent(EventTrigger eventTrigger, EventTriggerType eventType, string methodName)
    {
        foreach (EventTrigger.Entry entry in eventTrigger.triggers)
        {
            if (entry.eventID != eventType)
            {
                continue;
            }

            for (int i = 0; i < entry.callback.GetPersistentEventCount(); i++)
            {
                if (entry.callback.GetPersistentTarget(i) == this &&
                    entry.callback.GetPersistentMethodName(i) == methodName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static void AddButtonEvent(EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener(callback);
        eventTrigger.triggers.Add(entry);
    }
}

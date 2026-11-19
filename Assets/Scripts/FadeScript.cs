using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class FadeScript : MonoBehaviour
{
    Image img;

    void Start()
    {
        img = GetComponent<Image>();
        SetAlpha(1f);
        StartCoroutine(FadeIn(0.2f));
    }

    public IEnumerator FadeIn(float fadeTime)
    {
        yield return FadeTo(0f, fadeTime);
    }

    public IEnumerator FadeOut(float fadeTime)
    {
        yield return FadeTo(1f, fadeTime);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        img.raycastTarget = true;
        float startAlpha = img.color.a;

        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = t * t * (3f - 2f * t);
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, easedT));
            yield return null;
        }

        SetAlpha(targetAlpha);
        img.raycastTarget = false;
    }

    private void SetAlpha(float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}

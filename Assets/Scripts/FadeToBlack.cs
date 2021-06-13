using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeToBlack : MonoBehaviour
{

    public float FadeDuration { set => fadeDuration = value; }
    public static FadeToBlack instance => FindObjectOfType<FadeToBlack>();

    [SerializeField] float fadeDuration;
    [SerializeField] bool startBlack;

    public float Alpha => CanvasGroup.alpha;

    CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

    Coroutine fade;

    private void Start()
    {
        if (startBlack)
            FadeUp(true);
        else
            FadeDown(true);
    }

    public void FadeUp (bool instant = false)
    {
        if (fade != null)
            StopCoroutine(fade);

        if (instant)
            CanvasGroup.alpha = 1f;
        else
            StartCoroutine(Fade(CanvasGroup.alpha, 1f, fadeDuration));
    }

    public void FadeDown(bool instant = false)
    {
        if (fade != null)
            StopCoroutine(fade);
        if (instant)
            CanvasGroup.alpha = 0f;
        else
            StartCoroutine(Fade(CanvasGroup.alpha, 0f, fadeDuration));
    }

    IEnumerator Fade (float start, float end, float duration)
    {
        for(float i = 0; i < 1; i += 1f/duration * Time.deltaTime)
        {
            CanvasGroup.alpha = Mathf.Lerp(start, end, i);
            yield return new WaitForEndOfFrame();
        }
    }
}

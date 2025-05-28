using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;

    public IEnumerator FadeOut(float duration)
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0f;
        while (timer <= duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            timer += Time.unscaledDeltaTime; // 시간 멈춰도 작동
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeIn(float duration)
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0f;
        while (timer <= duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance
    {
        get;
        private set;
    }

    [Header("Referencias")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Configuración")]
    [SerializeField] private float defaultDuration = 2f;

    [Header("Animación")]
    [SerializeField] private float popDuration = 0.15f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float overshootScale = 1.1f;

    private Coroutine activeNotification;

    private Vector3 originalScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (notificationPanel != null)
        {
            originalScale =notificationPanel.transform.localScale;

            notificationPanel.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message,defaultDuration);
    }

    public void ShowMessage(string message,float duration)
    {
        if (notificationPanel == null ||notificationText == null)
        {
            Debug.LogError("Faltan referencias en NotificationUI.");

            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (activeNotification != null)
        {
            StopCoroutine(activeNotification);
        }

        activeNotification =StartCoroutine(ShowMessageRoutine(message,duration));
    }

    private IEnumerator ShowMessageRoutine(string message,float duration)
    {
        notificationText.text = message;

        notificationPanel.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        Transform panelTransform =notificationPanel.transform;

        panelTransform.localScale =originalScale * startScale;

        yield return StartCoroutine(AnimateScale(panelTransform,originalScale * startScale,originalScale * overshootScale,popDuration));

        yield return StartCoroutine(AnimateScale(panelTransform,originalScale * overshootScale,originalScale,popDuration));

        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f,duration));

        if (canvasGroup != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        panelTransform.localScale =originalScale;

        notificationPanel.SetActive(false);

        activeNotification = null;
    }

    private IEnumerator AnimateScale(Transform target,Vector3 from,Vector3 to,float duration)
    {
        if (duration <= 0f)
        {
            target.localScale = to;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer +=Time.unscaledDeltaTime;

            float t =Mathf.Clamp01(timer / duration);

            target.localScale =Vector3.Lerp(from,to,t);

            yield return null;
        }

        target.localScale = to;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer +=Time.unscaledDeltaTime;

            float t =Mathf.Clamp01(timer / fadeDuration);

            canvasGroup.alpha =Mathf.Lerp(1f,0f,t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
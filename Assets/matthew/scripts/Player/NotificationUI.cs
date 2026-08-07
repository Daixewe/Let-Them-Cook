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

    [Header("Configuración")]
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine activeNotification;

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
            notificationPanel.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDuration);
    }

    public void ShowMessage(string message, float duration)
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

        activeNotification = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private IEnumerator ShowMessageRoutine(string message,float duration)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, duration));

        notificationPanel.SetActive(false);
        activeNotification = null;
    }
}
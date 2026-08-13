using TMPro;
using UnityEngine;
using System.Collections;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private TMP_Text moneyText;

    [Header("Animación")]
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private float scaleMultiplier = 1.25f;

    private Vector3 originalScale;
    private Coroutine moneyAnimation;

    private void Awake()
    {
        if (moneyText != null)
        {
            originalScale =moneyText.transform.localScale;
        }
    }

    private void OnEnable()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (moneyManager == null ||moneyText == null)
        {
            return;
        }

        moneyText.text =$"₡{moneyManager.CurrentMoney}";

        if (moneyAnimation != null)
        {
            StopCoroutine(moneyAnimation);
        }

        moneyAnimation =StartCoroutine(AnimateMoney());
    }

    private IEnumerator AnimateMoney()
    {
        Transform target =moneyText.transform;

        Vector3 enlargedScale =originalScale *scaleMultiplier;

        float timer = 0f;

        // Aumenta de tamaño.
        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =timer / animationDuration;

            target.localScale =Vector3.Lerp(originalScale,enlargedScale,t);

            yield return null;
        }

        timer = 0f;

        // Regresa al tamaño original.
        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =timer / animationDuration;

            target.localScale =Vector3.Lerp(enlargedScale,originalScale,t);

            yield return null;
        }

        target.localScale =originalScale;

        moneyAnimation = null;
    }
}
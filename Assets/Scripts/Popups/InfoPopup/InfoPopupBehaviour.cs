using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupBehaviour : MonoBehaviour
{
    public static event Action OnInfoPopupClosed;
    public static string infoText;
    [SerializeField] private Image infoImage;
    [SerializeField] private TextMeshProUGUI text;
    private void OnEnable()
    {
        text.text = infoText;
        RectTransform imageRt = infoImage.GetComponent<RectTransform>();
        imageRt.sizeDelta = new Vector3(0, 0, 0);

        LeanTween
        .value(gameObject, updateTextColor, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), 0.3f)
        .setEase(LeanTweenType.easeOutQuint);
        LeanTween
        .size(imageRt, new Vector3(600, 100, 0), 0.3f)
        .setEase(LeanTweenType.easeOutQuint);

        LeanTween.delayedCall(gameObject, 1.7f, () =>
        {
            LeanTween
            .value(gameObject, updateTextColor, new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f), 0.15f);
            LeanTween
            .size(imageRt, new Vector3(0, 0, 0), 0.2f)
            .setOnComplete(() =>
            {
                OnInfoPopupClosed?.Invoke();
                gameObject.SetActive(false);
            });
        });
    }

    private void updateTextColor(Color color)
    {
        text.color = color;
    }
}

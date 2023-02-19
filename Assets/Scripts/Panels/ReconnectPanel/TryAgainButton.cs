using System;
using UnityEngine;
using UnityEngine.UI;

public class TryAgainButton : MonoBehaviour
{
    public static event Action OnTryAgainButtonClicked;
    private Button button;

    private void Awake()
    {
        button = gameObject.GetComponentInChildren<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        OnTryAgainButtonClicked?.Invoke();
    }
}

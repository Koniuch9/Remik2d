using System;
using UnityEngine;
using UnityEngine.UI;

public class CloseStatsPanelButton : MonoBehaviour
{
    public static event Action OnCloseStatsPanelButtonClicked;
    private Button button;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
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
        OnCloseStatsPanelButtonClicked?.Invoke();
    }
}

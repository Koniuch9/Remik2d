using System;
using UnityEngine;
using UnityEngine.UI;

public class CloseStackPanelButton : MonoBehaviour
{

    public static event Action OnCloseStackPanelButtonClicked;

    private void OnEnable()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        OnCloseStackPanelButtonClicked?.Invoke();
    }
}

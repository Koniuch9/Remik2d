using System;
using UnityEngine;
using UnityEngine.UI;

public class SortValueButton : MonoBehaviour
{
    public static event Action OnSortValueButtonClicked;

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
        OnSortValueButtonClicked?.Invoke();
    }
}

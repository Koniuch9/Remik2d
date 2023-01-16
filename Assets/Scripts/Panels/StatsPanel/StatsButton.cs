using System;
using UnityEngine;
using UnityEngine.UI;

public class StatsButton : MonoBehaviour
{
    public static event Action OnStatsButtonClicked;
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
        OnStatsButtonClicked?.Invoke();
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class TakeAllButton : MonoBehaviour
{
    public static event Action OnTakeAllButtonClicked;
    private Button button;
    private bool areThereCards = false;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        PropsManager.OnStackCardsUpdated += StackCardsUpdated;
        GameManager.OnDrawCardActive += DrawCardActive;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        PropsManager.OnStackCardsUpdated -= StackCardsUpdated;
        GameManager.OnDrawCardActive -= DrawCardActive;
    }

    private void DrawCardActive(bool active)
    {
        button.interactable = active;
    }

    private void StackCardsUpdated(string[] cards)
    {
        if (cards.Length > 1)
        {
            areThereCards = true;
        }
        else
        {
            areThereCards = false;
        }
    }

    private void OnClick()
    {
        if (areThereCards)
        {
            OnTakeAllButtonClicked?.Invoke();
        }
    }
}

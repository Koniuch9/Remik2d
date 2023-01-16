using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TakeDeckCardButton : MonoBehaviour
{
    public static event Action<string> OnTakeDeckCardButtonClicked;
    private Button button;
    private string topCardDescription;
    private bool canTakeCard = true;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        GameManager.OnDrawCardActive += DrawCardActive;
        PropsManager.OnDeckCardsUpdated += UpdateTopCard;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        GameManager.OnDrawCardActive -= DrawCardActive;
        PropsManager.OnDeckCardsUpdated -= UpdateTopCard;
    }

    private void UpdateTopCard(string[] cards)
    {
        if (cards.Length > 0)
        {
            topCardDescription = cards.Last();
        }
        else
        {
            button.interactable = false;
            canTakeCard = false;
        }
    }

    private void OnClick()
    {
        if (canTakeCard)
        {
            OnTakeDeckCardButtonClicked?.Invoke(topCardDescription);
            string[] cards = ((string)PropsManager.instance.GetProp(Props.DECK_CARDS))
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
            PropsManager.instance.SetProp(Props.DECK_CARDS, String.Join(',', cards.SkipLast(1).ToArray()));
        }
    }

    private void DrawCardActive(bool active)
    {
        button.interactable = active;
    }
}

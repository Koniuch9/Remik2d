using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StackCardsPanel : MonoBehaviour
{
    public static event Action<string[]> OnTakeAllFromStack;
    public static event Action<string> OnTakeFirstFromStack;
    [SerializeField] private GameObject firstCard;
    [SerializeField] private GameObject restCards;

    private Image firstCardImage;
    private string firstCardDescription;
    private string[] allCardsDescriptions;
    private List<GameObject> restCardsList;

    private void Awake()
    {
        firstCardImage = firstCard.GetComponent<Image>();
        restCardsList = new List<GameObject>();
    }

    private void OnEnable()
    {
        PropsManager.OnStackCardsUpdated += UpdateStackCards;
        StackButton.OnStackButtonClicked += ShowPanel;
        CloseStackPanelButton.OnCloseStackPanelButtonClicked += HidePanel;
        TakeAllButton.OnTakeAllButtonClicked += TakeAllButtonClicked;
        TakeFirstButton.OnTakeFirstButtonClicked += TakeFirstButtonClicked;
    }

    private void OnDisable()
    {
        PropsManager.OnStackCardsUpdated -= UpdateStackCards;
        StackButton.OnStackButtonClicked -= ShowPanel;
        CloseStackPanelButton.OnCloseStackPanelButtonClicked -= HidePanel;
        TakeAllButton.OnTakeAllButtonClicked -= TakeAllButtonClicked;
        TakeFirstButton.OnTakeFirstButtonClicked -= TakeFirstButtonClicked;
    }

    private void UpdateStackCards(string[] cards)
    {
        allCardsDescriptions = cards;
        if (cards.Length > 0)
        {
            firstCardDescription = cards.Last();
            firstCardImage.sprite = CardFactory.instance.getCardSprite(cards.Last());
            UpdateRestCards(cards.SkipLast(1).ToArray());
        }
        else
        {
            firstCardDescription = null;
            firstCardImage.sprite = null;
            foreach (GameObject go in restCardsList)
            {
                Destroy(go.gameObject);
            }
            restCardsList.Clear();
        }
    }

    private void UpdateRestCards(string[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < restCardsList.Count)
            {
                if (!restCardsList[i].GetComponent<CardData>().getDescription().Equals(cards[i]))
                {
                    Destroy(restCardsList[i].gameObject);
                    restCardsList.RemoveAt(i);
                    GameObject newCard = CardFactory.instance.createCard(restCards.transform, cards[i]);
                    restCardsList.Insert(i, newCard);
                    newCard.transform.SetSiblingIndex(i);
                }
            }
            else
            {
                GameObject newCard = CardFactory.instance.createCard(restCards.transform, cards[i]);
                restCardsList.Add(newCard);
                newCard.transform.SetSiblingIndex(i);
            }
        }
        for (int i = cards.Length; i < restCardsList.Count; i++)
        {
            Destroy(restCardsList[i].gameObject);
            restCardsList.RemoveAt(i);
        }
    }

    private void TakeAllButtonClicked()
    {
        HidePanel();
        OnTakeAllFromStack?.Invoke(allCardsDescriptions);
        PropsManager.instance.SetProp(Props.STACK_CARDS, "");
    }

    private void TakeFirstButtonClicked()
    {
        HidePanel();
        OnTakeFirstFromStack?.Invoke(firstCardDescription);
        string[] restCards = allCardsDescriptions.SkipLast(1).ToArray();
        if (restCards.Length == 0)
        {
            PropsManager.instance.SetProp(Props.STACK_CARDS, "");
        }
        else
        {
            PropsManager.instance.SetProp(Props.STACK_CARDS, String.Join(',', restCards));
        }
    }

    private void ShowPanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(-225f, 0f, 0f), 0.5f);
    }

    private void HidePanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(270f, 0f, 0f), 0.5f);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Meld : MonoBehaviour
{
    public static event Action<CardData, int> OnCardAddedToMeld;
    public static event Action<CardData, int, string> OnCardAddedToPlayerMeld;
    public static event Action<CardData, int> OnCardRemovedFromMeld;
    public static event Action OnInit;
    [SerializeField] private GameObject handlers;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject rightHandler;
    [SerializeField] private GameObject leftHandler;
    [SerializeField] private GameObject allHandler;

    [SerializeField] private string id;
    private List<GameObject> cardList;
    protected List<string> cardDataList;
    protected bool allowRemove = true;
    private bool isLaid = false;
    private string playerName;

    public string[] getDescriptionList()
    {
        return cardList.Select(go => go.GetComponent<CardData>().getDescription()).ToArray();
    }

    public string getId()
    {
        return id;
    }

    private void Awake()
    {
        cardList = new List<GameObject>();
        cardDataList = new List<string>();
    }

    public void Init(string id, bool allowRemove, bool isLaid, string playerName)
    {
        this.id = id;
        this.allowRemove = allowRemove;
        this.isLaid = isLaid;
        this.playerName = playerName;
        OnInit?.Invoke();
    }

    public void UpdateCards(string[] cardDescriptions)
    {
        foreach (GameObject go in cardList)
        {
            Destroy(go.gameObject);
        }
        cardList.Clear();
        cardDataList.Clear();
        int notJokerIndex = 0;
        for (int i = 0; i < cardDescriptions.Length; i++)
        {
            if (!cardDescriptions[i].Equals("XX"))
            {
                notJokerIndex = i;
                cardDataList.Add(cardDescriptions[i]);
                break;
            }
        }
        for (int i = notJokerIndex + 1; i < cardDescriptions.Length; i++)
        {
            cardDataList.Add(GetInternalData(cardDescriptions[i], false));
        }
        for (int i = notJokerIndex - 1; i >= 0; i--)
        {
            cardDataList.Insert(0, GetInternalData(cardDescriptions[i], true));
        }
        foreach (string s in cardDescriptions)
        {
            if (s.Length == 2)
            {
                GameObject newCard = CardFactory.instance.createMeldCard(content.transform, s, false, id);
                cardList.Add(newCard);
            }
        }
    }

    private void OnEnable()
    {
        CardManager.OnBeginDragCard += CardDragged;
        MeldCardManager.OnMeldCardClicked += CardClicked;
        CardManager.OnEndDragCard += CardDragEnded;
        LeftHandler.OnLeftHandlerCardAdded += CardAddedLeft;
        RightHandler.OnRightHandlerCardAdded += CardAddedRight;
        AllHandler.OnAllHandlerCardAdded += CardAddedAll;
    }


    private void OnDisable()
    {
        CardManager.OnBeginDragCard -= CardDragged;
        MeldCardManager.OnMeldCardClicked -= CardClicked;
        CardManager.OnEndDragCard -= CardDragEnded;
        LeftHandler.OnLeftHandlerCardAdded -= CardAddedLeft;
        RightHandler.OnRightHandlerCardAdded -= CardAddedRight;
        AllHandler.OnAllHandlerCardAdded -= CardAddedAll;
    }

    private void CardDragged(CardData cardData, int index)
    {
        bool canDropAll = this.canDropAll(cardData);
        bool canDropRight = this.canDropRight(cardData);
        bool canDropLeft = this.canDropLeft(cardData);
        GameUtils.PrintList<string>("CARD DRAGGED " + cardData.getDescription() + ": ", cardDataList);
        handlers.SetActive(canDropAll || canDropLeft || canDropRight);
        allHandler.SetActive(canDropAll);
        leftHandler.SetActive(canDropLeft);
        rightHandler.SetActive(canDropRight);
    }

    private void CardDragEnded(CardData cardData, int index)
    {
        handlers.SetActive(false);
    }

    private void CardAddedAll(string meldId, CardData cardData, int index)
    {
        CardAddedRight(meldId, cardData, index);
    }

    private void CardAddedRight(string meldId, CardData cardData, int index)
    {
        if (meldId.Equals(id))
        {
            AddCard(cardData, false, index);
        }
    }

    private void CardAddedLeft(string meldId, CardData cardData, int index)
    {
        if (meldId.Equals(id))
        {
            AddCard(cardData, true, index);
        }
    }

    private void AddCard(
        CardData cardData, bool left, int index, bool allowClick = true)
    {
        GameObject newCard =
            CardFactory.instance.createMeldCard(
                content.transform, cardData.getDescription(), allowClick, id);
        if (left)
        {
            cardList.Insert(0, newCard);
            cardDataList.Insert(0, GetInternalData(cardData, true));
            newCard.transform.SetSiblingIndex(0);
        }
        else
        {
            cardList.Add(newCard);
            cardDataList.Add(GetInternalData(cardData, false));
        }
        NotifyCardAdded(cardData, index);
    }

    private void NotifyCardAdded(CardData cardData, int index)
    {
        if (isLaid)
        {
            OnCardAddedToPlayerMeld?.Invoke(cardData, index, playerName);
        }
        else
        {
            OnCardAddedToMeld?.Invoke(cardData, index);
        }
    }

    private void CardClicked(CardData cardData, int index, string meldId)
    {
        if (canRemove(cardData, index) && meldId.Equals(id))
        {
            OnCardRemovedFromMeld?.Invoke(cardData, index);
            Destroy(cardList[index].gameObject);
            cardList.RemoveAt(index);
            cardDataList.RemoveAt(index);
        }
    }

    public abstract bool canDropAll(CardData cardData);
    public abstract bool canDropLeft(CardData cardData);
    public abstract bool canDropRight(CardData cardData);
    public abstract bool canRemove(CardData cardData, int index);
    public abstract bool isReady();
    protected abstract string GetInternalData(CardData cardData, bool left);
    protected abstract string GetInternalData(string cardDescription, bool left);
}

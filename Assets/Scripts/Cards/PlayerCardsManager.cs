using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PlayerCardsManager : MonoBehaviour
{
    public static event Action<string[]> OnCardsChangedInternal;
    public static event Action OnPlayerWon;
    public static event Action OnPlayerEndTurn;
    [SerializeField] private GameObject dragCardContainer;
    private List<GameObject> cardList;
    private Dictionary<string, int> cardDict;
    private void Start()
    {
        cardList = new List<GameObject>();
        cardDict = new Dictionary<string, int>();
    }

    private void OnEnable()
    {
        DeckCardAnimation.OnCardTakenFromDeckAnimationCompleted += AddCardFromDeck;
        SortColorButton.OnSortColorButtonClicked += SortCardsByColor;
        SortValueButton.OnSortValueButtonClicked += SortCardsByValue;
        PlayerPropsManager.OnPlayerCardsUpdated += UpdateCards;
        CardManager.OnBeginDragCard += BeginDragCard;
        CardManager.OnEndDragCard += EndDragCard;
        CardManager.OnSwitchPositions += SwitchCardPositions;
        StackButton.OnCardAddedToStack += CardAddedToStack;
        StackCardsPanel.OnTakeAllFromStack += AllStackCardsTaken;
        StackCardsPanel.OnTakeFirstFromStack += FirstStackCardTaken;
        Meld.OnCardAddedToMeld += CardAddedToMeld;
        Meld.OnCardRemovedFromMeld += CardRemovedFromMeld;
        MeldsManager.OnMeld += MeldHappened;
        Meld.OnCardAddedToPlayerMeld += CardAddedToPlayerMeld;
        PropsManager.OnGameTurnUpdated += NewGameTurn;
    }

    private void OnDisable()
    {
        DeckCardAnimation.OnCardTakenFromDeckAnimationCompleted -= AddCardFromDeck;
        SortColorButton.OnSortColorButtonClicked -= SortCardsByColor;
        SortValueButton.OnSortValueButtonClicked -= SortCardsByValue;
        PlayerPropsManager.OnPlayerCardsUpdated -= UpdateCards;
        CardManager.OnBeginDragCard -= BeginDragCard;
        CardManager.OnEndDragCard -= EndDragCard;
        CardManager.OnSwitchPositions -= SwitchCardPositions;
        StackButton.OnCardAddedToStack -= CardAddedToStack;
        StackCardsPanel.OnTakeAllFromStack -= AllStackCardsTaken;
        StackCardsPanel.OnTakeFirstFromStack -= FirstStackCardTaken;
        Meld.OnCardAddedToMeld -= CardAddedToMeld;
        Meld.OnCardRemovedFromMeld -= CardRemovedFromMeld;
        MeldsManager.OnMeld -= MeldHappened;
        Meld.OnCardAddedToPlayerMeld -= CardAddedToPlayerMeld;
        PropsManager.OnGameTurnUpdated -= NewGameTurn;
    }

    public bool isGameWon()
    {
        return GetCardDescriptions().Length == 0;
    }

    private void NewGameTurn(int gameTurn)
    {

    }

    private void BeginDragCard(CardData cardData, int index)
    {
        cardList[index].transform.SetParent(dragCardContainer.transform);
    }

    private void EndDragCard(CardData cardData, int index)
    {
        if (index < cardList.Count)
        {
            cardList[index].transform.SetParent(transform);
            cardList[index].transform.SetSiblingIndex(index);
        }
    }

    private void SwitchCardPositions(CardData cardData, int draggedIndex, int endedIndex)
    {
        if (draggedIndex > endedIndex)
        {
            GameObject tmp = cardList[draggedIndex];
            for (int i = draggedIndex; i > endedIndex; i--)
            {
                cardList[i] = cardList[i - 1];
            }
            cardList[endedIndex] = tmp;
        }
        else if (draggedIndex < endedIndex)
        {
            GameObject tmp = cardList[draggedIndex];
            for (int i = draggedIndex; i < endedIndex; i++)
            {
                cardList[i] = cardList[i + 1];
            }
            cardList[endedIndex] = tmp;
        }
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.SetSiblingIndex(i);
        }
        NotifyCardsChangedInternal();
    }

    private void CardAddedToMeld(CardData cardData, int index)
    {
        Destroy(cardList[index].gameObject);
        cardList.RemoveAt(index);
    }

    private void CardAddedToPlayerMeld(CardData cardData, int index, string playerName)
    {
        Destroy(cardList[index].gameObject);
        cardList.RemoveAt(index);
        string key = cardData.getDescription();
        cardDict[key]--;
        if (cardDict[key] == 0)
        {
            cardDict.Remove(key);
        }
        NotifyCardsChanged();
    }

    private void CardRemovedFromMeld(CardData cardData, int index)
    {
        AddCard(cardData.getDescription(), false);
    }

    private void MeldHappened(string[][] meldCards)
    {
        for (int i = 0; i < meldCards.Length; i++)
        {
            for (int j = 0; j < meldCards[i].Length; j++)
            {
                string key = meldCards[i][j];
                cardDict[key]--;
                if (cardDict[key] == 0)
                {
                    cardDict.Remove(key);
                }
            }
        }
        NotifyCardsChanged();
    }

    private void CardAddedToStack(CardData card, int index)
    {
        string key = cardList[index].GetComponent<CardData>().getDescription();
        cardDict[key]--;
        if (cardDict[key] == 0)
        {
            cardDict.Remove(key);
        }
        Destroy(cardList[index].gameObject);
        cardList.RemoveAt(index);
        if (cardDict.Count == 0)
        {
            OnPlayerWon?.Invoke();
        }
        else
        {
            OnPlayerEndTurn?.Invoke();
        }
        NotifyCardsChanged();
    }

    private void FirstStackCardTaken(string firstCardDescription)
    {
        AddCard(firstCardDescription);
        NotifyCardsChanged();
    }

    private void AllStackCardsTaken(string[] cards)
    {
        AddAllCards(cards);
        NotifyCardsChanged();
    }

    private void AddCardFromDeck(string description)
    {
        AddCard(description);
        NotifyCardsChanged();
    }

    private void AddRandomCard()
    {
        AddCard(CardUtils.getRandomCardDecription());
    }

    private void AddCard(string description, bool updateDict = true)
    {
        GameObject newCard = CardFactory.instance.createCard(transform, description, false, true);
        cardList.Add(newCard);
        if (updateDict)
        {
            if (cardDict.ContainsKey(description))
            {
                cardDict[description]++;
            }
            else
            {
                cardDict[description] = 1;
            }
        }
    }

    private void AddCard(CardData cardData)
    {
        AddCard(CardUtils.getDescriptionFromData(cardData));
    }

    private void AddAllCards(string[] cards)
    {
        foreach (string card in cards)
        {
            AddCard(card);
        }
    }

    private void SortCardsByValue()
    {
        cardList.Sort((a, b) => CardUtils.compareByValue(a, b));
        RecreateFromCardList();
        NotifyCardsChangedInternal();
    }

    private void SortCardsByColor()
    {
        cardList.Sort((a, b) => CardUtils.compareByColor(a, b));
        RecreateFromCardList();
        NotifyCardsChangedInternal();
    }

    private void RecreateFromCardList()
    {
        int i = 0;
        foreach (GameObject card in cardList)
        {
            card.gameObject.transform.SetSiblingIndex(i++);
        }
    }

    private void NotifyCardsChanged()
    {
        if (isGameWon())
        {
            PropsManager.instance.SetProp(Props.PLAYER_WON, PhotonNetwork.LocalPlayer.NickName);
        }
        PlayerPropsManager.instance.SetProp(
                    PhotonNetwork.LocalPlayer.NickName,
                    PlayerProps.CARDS,
                    String.Join(',', GetCardDescriptions()));
        NotifyCardsChangedInternal();
    }

    private void NotifyCardsChangedInternal()
    {
        OnCardsChangedInternal?.Invoke(GetCardDescriptions());
    }

    private void UpdateCards(string playerName, string[] cardDescriptions)
    {
        PrintDescriptions(cardDescriptions);
        if (playerName.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            Dictionary<string, int> dict = getCardsDict(cardDescriptions);
            if (dict.Count != cardDict.Count || dict.Except(cardDict).Any())
            {
                RemoveAllCards();
                foreach (string cardDescription in cardDescriptions)
                {
                    AddCard(cardDescription);
                }
                NotifyCardsChangedInternal();
            }
        }
    }

    private void RemoveAllCards()
    {
        foreach (GameObject card in cardList)
        {
            Destroy(card.gameObject);
        }
        cardList.Clear();
        cardDict.Clear();
    }

    private Dictionary<string, int> getCardsDict(string[] cardDescriptions)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        foreach (string description in cardDescriptions)
        {
            if (result.ContainsKey(description))
            {
                result[description]++;
            }
            else
            {
                result[description] = 1;
            }
        }
        return result;
    }

    private string[] GetCardDescriptions()
    {
        string[] result = cardDict.Aggregate(new string[0], (acc, current) =>
            acc.Concat(Enumerable.Repeat(current.Key, current.Value)).ToArray()
        );
        PrintDescriptions(result);
        return result;
    }

    private void PrintDescriptions(string[] d)
    {
        string s = "";
        foreach (string sx in d)
        {
            s += sx + ",";
        }
        Debug.Log("DSPTS: " + s);
    }

    private void PrintCards()
    {
        string s = "";
        foreach (GameObject go in cardList)
        {
            CardData cD = go.GetComponent<CardData>();
            s += cD.value + cD.type + go.transform.GetSiblingIndex() + ",";
        }
        Debug.Log("CARDS: " + s);
    }
}

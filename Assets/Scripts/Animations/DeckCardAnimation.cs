using System;
using Photon.Pun;
using UnityEngine;

public class DeckCardAnimation : MonoBehaviour
{
    public static event Action<string> OnCardTakenFromDeckAnimationCompleted;
    public static event Action<string, Transform> OnAnimteCardTakenFromDeck;
    private int deckCardsNumber = 0;

    private void OnEnable()
    {
        TakeDeckCardButton.OnTakeDeckCardButtonClicked += AnimateCardTakenFromStack;
        PropsManager.OnDeckCardsUpdated += DeckCardsUpdated;
    }

    private void OnDisable()
    {
        TakeDeckCardButton.OnTakeDeckCardButtonClicked -= AnimateCardTakenFromStack;
        PropsManager.OnDeckCardsUpdated -= DeckCardsUpdated;
    }

    private void DeckCardsUpdated(string[] cards)
    {
        string currentPlayerTurn = (string)PropsManager.instance.GetProp(Props.PLAYER_TURN);
        if (!currentPlayerTurn.Equals("") &&
            !currentPlayerTurn.Equals(PhotonNetwork.LocalPlayer.NickName) &&
            cards.Length < deckCardsNumber)
        {
            OnAnimteCardTakenFromDeck?.Invoke(currentPlayerTurn, transform);
        }
        deckCardsNumber = cards.Length;
    }

    private void AnimateCardTakenFromStack(string cardDescription)
    {
        GameObject newCard = CardFactory.instance.createCard(transform, cardDescription, true);
        CardManager cardManager = newCard.GetComponent<CardManager>();
        RectTransform cardRt = newCard.GetComponent<RectTransform>();
        float firstTime = 0.7f;
        float secondTime = 0.6f;
        LeanTween.move(cardRt, new Vector3(-300, 0, 0), firstTime).setEaseInCubic();
        LeanTween.rotate(cardRt, -90f, firstTime).setEaseInCubic();
        LeanTween.scale(cardRt, new Vector3(0f, 1f, 1f), firstTime).setEaseInCubic().setOnComplete(() =>
        {
            cardManager.turnCardFaceUp();
            LeanTween.move(cardRt, new Vector3(-400, 300, 0), secondTime).setEaseOutCubic();
            LeanTween.rotate(cardRt, -90f, secondTime).setEaseOutCubic();
            LeanTween
            .scale(cardRt, new Vector3(1f, 1f, 1f), secondTime)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(1f, () =>
                {
                    Destroy(newCard);
                    OnCardTakenFromDeckAnimationCompleted?.Invoke(cardDescription);
                });
            });
        });
    }
}

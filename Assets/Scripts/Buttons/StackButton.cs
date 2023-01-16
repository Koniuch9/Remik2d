using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action OnStackButtonClicked;
    public static event Action<CardData, int> OnCardAddedToStack;
    public static event Action<string, string, Transform> OnAnimateCardAddedToStack;
    public static event Action<string, string, Transform> OnAnimateCardTakenFromStack;
    public static event Action<string, string, string[], Transform> OnAnimateAllCardsTakenFromStack;

    [SerializeField] private GameObject stackCardAnimation;

    private Button button;
    private Image image;
    private bool canStackCard = false;
    private bool isDragging = false;
    private bool isPointerOnMe = false;
    private int stackCardsNumber = 0;
    private string[] stackCardDescriptions;
    private string topCardDescription = "";

    private void Awake()
    {
        button = gameObject.GetComponentInChildren<Button>();
        image = gameObject.GetComponent<Image>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
        GameManager.OnStackCardActive += StackCardActive;
        CardManager.OnBeginDragCard += SetDragging;
        CardManager.OnEndDragCard += UnsetDragging;
        PropsManager.OnStackCardsUpdated += StackCardsUpdated;
        PropsManager.OnPlayerWon += PlayerWon;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
        GameManager.OnStackCardActive -= StackCardActive;
        CardManager.OnBeginDragCard -= SetDragging;
        CardManager.OnEndDragCard -= UnsetDragging;
        PropsManager.OnStackCardsUpdated -= StackCardsUpdated;
        PropsManager.OnPlayerWon -= PlayerWon;
    }

    private void PlayerWon(string playerName)
    {
        stackCardDescriptions = null;
        stackCardsNumber = 0;
        topCardDescription = "";
    }

    private void OnClick()
    {
        OnStackButtonClicked?.Invoke();
    }

    private void StackCardsUpdated(string[] stackCards)
    {
        string currentPlayerTurn = (string)PropsManager.instance.GetProp(Props.PLAYER_TURN);
        if (stackCards != null &&
        !currentPlayerTurn.Equals("") &&
        !currentPlayerTurn.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            if (stackCards.Length == stackCardsNumber + 1)
            {
                OnAnimateCardAddedToStack?
                    .Invoke(currentPlayerTurn, stackCards.Last(), stackCardAnimation.transform);
            }
            else if (stackCards.Length == stackCardsNumber - 1)
            {
                OnAnimateCardTakenFromStack?
                    .Invoke(currentPlayerTurn, topCardDescription, stackCardAnimation.transform);
            }
            else if (stackCards.Length == 0 && stackCardsNumber > 1)
            {
                OnAnimateAllCardsTakenFromStack?
                    .Invoke(currentPlayerTurn, topCardDescription,
                    stackCardDescriptions, stackCardAnimation.transform);
            }
        }
        if (stackCards.Length > 0)
        {
            image.sprite = CardFactory.instance.getCardSprite(stackCards.Last());
        }
        else
        {
            image.sprite = null;
        }

        if (stackCards != null)
        {
            stackCardsNumber = stackCards.Length;
            stackCardDescriptions = stackCards;
            if (stackCards.Length > 0)
            {
                topCardDescription = stackCards.Last();
            }
            else
            {
                topCardDescription = "XX";
            }
        }
    }

    private void StackCardActive(bool active)
    {
        canStackCard = active;
    }

    private void SetDragging(CardData cardData, int index)
    {
        isDragging = true;
    }

    private void UnsetDragging(CardData cardData, int index)
    {
        if (isDragging && canStackCard && isPointerOnMe)
        {
            OnCardAddedToStack?.Invoke(cardData, index);
        }
        isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOnMe = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOnMe = false;
    }
}

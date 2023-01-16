using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardManager : MonoBehaviour,
    IPointerEnterHandler,
    IBeginDragHandler,
    IDragHandler,
    IPointerExitHandler,
    IEndDragHandler
{
    public static event Action<CardData, int> OnBeginDragCard;
    public static event Action<CardData, int> OnEndDragCard;
    public static event Action<CardData, int, int> OnSwitchPositions;
    [SerializeField] private Texture2D cardTexture;
    [SerializeField] private Texture2D backTexture;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color normalColor;
    private GameObject dragCardContainer;
    private bool allowDrag;
    private int draggedSiblingIndex;
    private bool checkPointerPosition = true;
    private bool isPointerOnMe = false;
    private bool allowSwitching = true;

    private Sprite cardSprite;
    private Sprite backSprite;

    private Image cardImage;

    private void Awake()
    {
        cardImage = gameObject.GetComponent<Image>();
        if (cardTexture != null)
        {
            cardSprite = createSprite(cardTexture);
        }
        if (backTexture != null)
        {
            backSprite = createSprite(backTexture);
        }
    }

    private void OnEnable()
    {
        OnEndDragCard += CheckSwitchPositions;
    }

    private void OnDisable()
    {
        OnEndDragCard -= CheckSwitchPositions;
    }

    public void Init(
        Texture2D cT,
        GameObject cardDragContainer,
        bool allowDrag,
        Texture2D bT = null)
    {
        dragCardContainer = cardDragContainer;
        cardTexture = cT;
        this.allowDrag = allowDrag;
        cardSprite = createSprite(cT);
        if (bT != null)
        {
            backTexture = bT;
            backSprite = createSprite(bT);
        }
    }

    public void turnCardFaceUp()
    {
        cardImage.sprite = cardSprite;
    }

    public void turnCardFaceDown()
    {
        cardImage.sprite = backSprite;
    }

    private Sprite createSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private void CheckSwitchPositions(CardData cardData, int siblingIndex)
    {
        if (isPointerOnMe && allowSwitching)
        {
            OnSwitchPositions?.Invoke(cardData, siblingIndex, transform.GetSiblingIndex());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (checkPointerPosition)
        {
            cardImage.color = hoverColor;
            isPointerOnMe = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (allowDrag)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (checkPointerPosition)
        {
            cardImage.color = normalColor;
            isPointerOnMe = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (allowDrag)
        {
            checkPointerPosition = false;
            isPointerOnMe = false;
            draggedSiblingIndex = transform.GetSiblingIndex();
            OnBeginDragCard?.Invoke(gameObject.GetComponent<CardData>(), draggedSiblingIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (allowDrag)
        {
            checkPointerPosition = true;
            cardImage.color = normalColor;
            isPointerOnMe = false;
            OnEndDragCard?.Invoke(gameObject.GetComponent<CardData>(), draggedSiblingIndex);
        }
    }
}

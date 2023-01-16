using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Handler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Color highlightColor = new Color(0f, 0f, 0.7f, 0.7f);
    public static Color normalColor = new Color(0f, 0f, 0.5f, 0.5f);
    private Image image;
    private bool isPointerOnMe = false;
    [SerializeField] private Meld parentMeld;
    [SerializeField] protected string meldId;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
        image.color = normalColor;
        meldId = parentMeld.getId();
    }
    private void OnEnable()
    {
        isPointerOnMe = false;
        image.color = normalColor;
        CardManager.OnEndDragCard += CardDragged;
    }

    private void OnDisable()
    {
        CardManager.OnEndDragCard -= CardDragged;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
        isPointerOnMe = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightColor;
        isPointerOnMe = true;
    }

    private void CardDragged(CardData cardData, int index)
    {
        if (isPointerOnMe)
        {
            isPointerOnMe = false;
            Handle(cardData, index);
        }
    }

    protected abstract void Handle(CardData cardData, int index);
}

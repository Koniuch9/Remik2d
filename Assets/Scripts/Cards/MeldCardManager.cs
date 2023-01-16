using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MeldCardManager : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public static event Action<CardData, int, string> OnMeldCardClicked;
    [SerializeField] private Texture2D cardTexture;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color normalColor;
    private bool isPointerOnMe = false;
    private bool allowClick = false;
    private Sprite cardSprite;
    private Image cardImage;
    private string meldId;

    private void Awake()
    {
        cardImage = gameObject.GetComponent<Image>();
        if (cardTexture != null)
        {
            cardImage.sprite = createSprite(cardTexture);
        }
    }

    public void Init(Texture2D cT, bool allowClick, string meldId)
    {
        cardTexture = cT;
        cardSprite = createSprite(cT);
        cardImage.sprite = cardSprite;
        this.allowClick = allowClick;
        this.meldId = meldId;
    }

    private Sprite createSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CARD CLICKED: " + allowClick);
        if (allowClick)
        {
            OnMeldCardClicked?.Invoke(gameObject.GetComponent<CardData>(), transform.GetSiblingIndex(), meldId);
        }
    }
}

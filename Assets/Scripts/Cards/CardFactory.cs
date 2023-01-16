using UnityEngine;
using UnityEngine.UI;

public class CardFactory : MonoBehaviour
{
    public static CardFactory instance { get; private set; }
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject meldCardPrefab;
    [SerializeField] private GameObject endCardPrefab;
    [SerializeField] private GameObject cardDragContainer;

    private void Awake()
    {
        if (instance != null && instance == this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public Sprite getCardSprite(string cardDescription)
    {
        string resName = CardUtils.getCardResourceName(cardDescription);
        if (resName.Equals(""))
        {
            resName = "cardJoker";
        }
        Texture2D tex = Resources.Load<Texture2D>(resName);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public GameObject createDummyCard(Transform parent)
    {
        return Instantiate(endCardPrefab, parent, false);
    }

    public GameObject createEndCard(Transform parent, string descripion)
    {
        GameObject newCard = Instantiate(endCardPrefab, parent, false);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        newCard.GetComponent<Image>().sprite = getCardSprite(descripion);
        return newCard;
    }

    public GameObject createMeldCard(Transform parent, string descripion, bool allowClick, string meldId)
    {
        string resName = CardUtils.getCardResourceName(descripion);
        if (resName.Equals(""))
        {
            resName = "cardJoker";
        }
        Texture2D tex = Resources.Load<Texture2D>(resName);
        GameObject newCard = Instantiate(meldCardPrefab, parent, false);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        MeldCardManager cardManager = newCard.GetComponent<MeldCardManager>();
        CardData cardData = newCard.GetComponent<CardData>();
        cardData.value = "" + descripion[0];
        cardData.type = "" + descripion[1];
        cardManager.Init(tex, allowClick, meldId);
        return newCard;
    }

    public GameObject createCard(Transform parent, string descripion, bool faceDown = false, bool allowDrag = false)
    {
        string resName = CardUtils.getCardResourceName(descripion);
        if (resName.Equals(""))
        {
            resName = "cardJoker";
        }
        Texture2D tex = Resources.Load<Texture2D>(resName);
        GameObject newCard = Instantiate(cardPrefab, parent, false);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        CardManager cardManager = newCard.GetComponent<CardManager>();
        CardData cardData = newCard.GetComponent<CardData>();
        cardData.value = "" + descripion[0];
        cardData.type = "" + descripion[1];
        cardManager.Init(tex, cardDragContainer, allowDrag);
        if (faceDown)
        {
            cardManager.turnCardFaceDown();
        }
        else
        {
            cardManager.turnCardFaceUp();
        }
        return newCard;
    }

}

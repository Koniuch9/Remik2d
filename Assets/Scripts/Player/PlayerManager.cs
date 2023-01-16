using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Color myBorderColor;
    public Color enemyBorderColor;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private TextMeshProUGUI cardsNumber;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject meldPanel;
    [SerializeField] private Sprite[] happyEmojis;
    [SerializeField] private Sprite[] angryEmojis;
    [SerializeField] private Sprite[] laughEmojis;
    [SerializeField] private GameObject emoji;
    [SerializeField] private PlayerMeldsManager playerMeldsManager;
    private Image playerBorder;
    private string playerName;
    private AudioSource currentAudioSource;
    private bool emojiHidden = true;

    public string getPlayerName()
    {
        return playerName;
    }

    private void Awake()
    {
        playerBorder = playerPanel.gameObject.GetComponent<Image>();
    }

    private void OnEnable()
    {
        PlayerPropsManager.OnPlayerCardsUpdated += UpdatePlayerCards;
        PropsManager.OnPlayerTurnUpdated += UpdatePlayerTurn;
        AudioManager.OnHappySound += PlayHappySound;
        AudioManager.OnAngrySound += PlayAngrySound;
        AudioManager.OnLaugh += PlayLaugh;
        DeckCardAnimation.OnAnimteCardTakenFromDeck += AnimateCardTakenFromDeck;
        StackButton.OnAnimateCardTakenFromStack += AnimateCardTakenFromStack;
        StackButton.OnAnimateCardAddedToStack += AnimateCardAddedToStack;
        StackButton.OnAnimateAllCardsTakenFromStack += AnimateAllCardsTakenFromStack;
        PlayerMeldsManager.OnAnimateCardAddedToPlayerMeld += AnimateCardAddedToPlayerMeld;
    }

    private void OnDisable()
    {
        PlayerPropsManager.OnPlayerCardsUpdated += UpdatePlayerCards;
        PropsManager.OnPlayerTurnUpdated -= UpdatePlayerTurn;
        AudioManager.OnHappySound -= PlayHappySound;
        AudioManager.OnAngrySound -= PlayAngrySound;
        AudioManager.OnLaugh -= PlayLaugh;
        DeckCardAnimation.OnAnimteCardTakenFromDeck += AnimateCardTakenFromDeck;
        StackButton.OnAnimateCardTakenFromStack -= AnimateCardTakenFromStack;
        StackButton.OnAnimateCardAddedToStack -= AnimateCardAddedToStack;
        StackButton.OnAnimateAllCardsTakenFromStack -= AnimateAllCardsTakenFromStack;
        PlayerMeldsManager.OnAnimateCardAddedToPlayerMeld -= AnimateCardAddedToPlayerMeld;
    }

    private void Update()
    {
        if (currentAudioSource != null &&
        !currentAudioSource.isPlaying &&
        !emojiHidden &&
        emoji != null)
        {
            LeanTween.scale(emoji.GetComponent<RectTransform>(), new Vector3(0.1f, 0.1f, 0.1f), 0.2f)
                .setOnComplete(() => emoji.SetActive(false));
            emojiHidden = true;
            currentAudioSource = null;
        }
    }

    public void Init(string playerName, int howManyCards, string playerNameTurn)
    {
        this.playerName = playerName;
        playerNameTxt.text = playerName;
        cardsNumber.text = howManyCards.ToString();
        if (playerName.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            playerBorder.color = myBorderColor;
        }
        else
        {
            playerBorder.color = enemyBorderColor;
        }
        playerMeldsManager.Init(playerName);
        UpdatePlayerTurn(playerNameTurn);
    }

    private void AnimateCardAddedToPlayerMeld(string playerName, string cardDescription, Transform parent)
    {
        if (this.playerName.Equals(playerName))
        {
            GameObject newCard = CardFactory.instance.createEndCard(playerPanel.transform, cardDescription);
            RectTransform cardRt = newCard.GetComponent<RectTransform>();
            cardRt.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            LeanTween.move(newCard, parent, 1.5f).setOnComplete(() => Destroy(newCard.gameObject));
            LeanTween.scale(cardRt, new Vector3(1f, 1f, 1f), 1f);
        }
    }

    private void AnimateAllCardsTakenFromStack(
        string playerName, string cardDescription, string[] stackCards, Transform parent)
    {
        if (this.playerName.Equals(playerName))
        {
            GameObject newCard = CardFactory.instance.createEndCard(parent, cardDescription);
            for (int i = 0; i < stackCards.Length - 1; i++)
            {
                GameObject dummyCard = CardFactory.instance.createEndCard(parent, stackCards[i]);
                LeanTween.move(dummyCard, playerPanel.transform, 1.5f)
                .setDelay((i + 1) * 0.1f).setOnComplete(() => Destroy(dummyCard.gameObject));
                LeanTween.scale(dummyCard.GetComponent<RectTransform>(), new Vector3(0.2f, 0.2f, 0.2f), 1.4f);
            }
            RectTransform cardRt = newCard.GetComponent<RectTransform>();
            LeanTween.move(newCard, playerPanel.transform, 1.5f).setOnComplete(() => Destroy(newCard.gameObject));
            LeanTween.scale(cardRt, new Vector3(0.2f, 0.2f, 0.2f), 1.4f);
        }
    }

    private void AnimateCardAddedToStack(string playerName, string cardDescription, Transform parent)
    {
        if (this.playerName.Equals(playerName))
        {
            GameObject newCard = CardFactory.instance.createEndCard(playerPanel.transform, cardDescription);
            RectTransform cardRt = newCard.GetComponent<RectTransform>();
            cardRt.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            LeanTween.move(newCard, parent, 1.5f).setOnComplete(() => Destroy(newCard.gameObject));
            LeanTween.scale(cardRt, new Vector3(1f, 1f, 1f), 1.4f);
        }
    }

    private void AnimateCardTakenFromStack(string playerName, string cardDescription, Transform parent)
    {
        if (this.playerName.Equals(playerName))
        {
            GameObject newCard = CardFactory.instance.createEndCard(parent, cardDescription);
            RectTransform cardRt = newCard.GetComponent<RectTransform>();
            LeanTween.move(newCard, playerPanel.transform, 1.5f).setOnComplete(() => Destroy(newCard.gameObject));
            LeanTween.scale(cardRt, new Vector3(0.2f, 0.2f, 0.2f), 1.4f);
        }
    }

    private void AnimateCardTakenFromDeck(string playerName, Transform parent)
    {
        if (this.playerName.Equals(playerName))
        {
            GameObject newCard = CardFactory.instance.createCard(parent, "XX", true);
            LeanTween.scale(newCard.GetComponent<RectTransform>(), new Vector3(0.2f, 0.2f, 0.2f), 1.4f);
            LeanTween.move(newCard, playerPanel.transform, 1.5f)
                .setOnComplete(() => Destroy(newCard.gameObject));
        }
    }

    private void PlayLaugh(string playerName, AudioSource source)
    {
        if (this.playerName.Equals(playerName))
        {
            RectTransform emojiRt = emoji.GetComponent<RectTransform>();
            emoji.GetComponent<Image>().sprite = GameUtils.GetRandom<Sprite>(laughEmojis);
            emojiRt.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            emoji.SetActive(true);
            LeanTween.scale(emojiRt, new Vector3(1f, 1f, 1f), 0.2f);
            currentAudioSource = source;
            emojiHidden = false;
            source.Play();
        }
    }

    private void PlayAngrySound(string playerName, AudioSource source)
    {
        if (this.playerName.Equals(playerName))
        {
            RectTransform emojiRt = emoji.GetComponent<RectTransform>();
            emoji.GetComponent<Image>().sprite = GameUtils.GetRandom<Sprite>(angryEmojis);
            emojiRt.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            emoji.SetActive(true);
            LeanTween.scale(emojiRt, new Vector3(1f, 1f, 1f), 0.2f);
            currentAudioSource = source;
            emojiHidden = false;
            source.Play();
        }
    }

    private void PlayHappySound(string playerName, AudioSource source)
    {
        if (this.playerName.Equals(playerName))
        {
            RectTransform emojiRt = emoji.GetComponent<RectTransform>();
            emoji.GetComponent<Image>().sprite = GameUtils.GetRandom<Sprite>(happyEmojis);
            emojiRt.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            emoji.SetActive(true);
            LeanTween.scale(emojiRt, new Vector3(1f, 1f, 1f), 0.2f);
            currentAudioSource = source;
            emojiHidden = false;
            source.Play();
        }
    }

    private void UpdatePlayerCards(string playerName, string[] cards)
    {
        if (this.playerName.Equals(playerName))
        {
            cardsNumber.text = cards.Length.ToString();
        }
    }

    private void UpdatePlayerTurn(string playerTurnName)
    {
        if (playerName.Equals(playerTurnName))
        {
            LeanTween.moveLocalX(playerPanel, 25, 0.5f);
        }
        else
        {
            LeanTween.moveLocalX(playerPanel, -25, 0.5f);
        }
    }
}

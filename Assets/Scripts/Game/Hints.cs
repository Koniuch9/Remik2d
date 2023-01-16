using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour
{
    [SerializeField] private GameObject stackArrow;
    [SerializeField] private GameObject deckArrow;
    [SerializeField] private TextMeshProUGUI hint;
    private Image stackImage;
    private Image deckImage;
    private bool stackPlus = false;
    private bool deckPlus = false;

    private void Awake()
    {
        stackArrow.SetActive(false);
        deckArrow.SetActive(false);
        hint.gameObject.SetActive(false);
        stackImage = stackArrow.GetComponent<Image>();
        deckImage = deckArrow.GetComponent<Image>();
    }

    private void OnEnable()
    {
        PropsManager.OnPlayerTurnUpdated += PlayerTurnUpdated;
        StackCardsPanel.OnTakeAllFromStack += CardTaken;
        StackCardsPanel.OnTakeFirstFromStack += CardTaken;
        TakeDeckCardButton.OnTakeDeckCardButtonClicked += CardTaken;
    }

    private void OnDisable()
    {
        PropsManager.OnPlayerTurnUpdated -= PlayerTurnUpdated;
        StackCardsPanel.OnTakeAllFromStack -= CardTaken;
        StackCardsPanel.OnTakeFirstFromStack -= CardTaken;
        TakeDeckCardButton.OnTakeDeckCardButtonClicked -= CardTaken;
    }

    private void Update()
    {
        if (stackArrow.activeSelf)
        {
            Color c = stackImage.color;
            c.b += (stackPlus ? 1 : -1) * Time.deltaTime;
            if (c.b <= 0f)
            {
                stackPlus = true;
            }
            else if (c.b >= 1f)
            {
                stackPlus = false;
            }
            stackImage.color = c;
        }
        if (deckArrow.activeSelf)
        {
            Color c = deckImage.color;
            c.b += (deckPlus ? 1 : -1) * Time.deltaTime;
            if (c.b <= 0f)
            {
                deckPlus = true;
            }
            else if (c.b >= 1f)
            {
                deckPlus = false;
            }
            deckImage.color = c;
        }
    }

    private void CardTaken(object o)
    {
        deckArrow.SetActive(false);
        hint.text = "Rzuc karte";
    }

    private void PlayerTurnUpdated(string playerName)
    {
        if (PhotonNetwork.LocalPlayer.NickName.Equals(playerName))
        {
            stackArrow.SetActive(true);
            deckArrow.SetActive(true);
            hint.text = "Wez karte";
            hint.gameObject.SetActive(true);
        }
        else
        {
            stackArrow.SetActive(false);
            deckArrow.SetActive(false);
            hint.gameObject.SetActive(false);
        }
    }
}

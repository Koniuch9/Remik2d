using TMPro;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCardsNumber;
    [SerializeField] private TextMeshProUGUI stackCardsNumber;
    [SerializeField] private TextMeshProUGUI gameType;

    private void OnEnable()
    {
        PropsManager.OnDeckCardsUpdated += SetDeckCardsNumber;
        PropsManager.OnStackCardsUpdated += SetStackCardsNumber;
        PropsManager.OnGameTypeUpdated += SetGameType;
    }

    private void OnDisable()
    {
        PropsManager.OnDeckCardsUpdated -= SetDeckCardsNumber;
        PropsManager.OnGameTypeUpdated -= SetGameType;
        PropsManager.OnStackCardsUpdated -= SetStackCardsNumber;
    }

    private void SetDeckCardsNumber(string[] cards)
    {
        deckCardsNumber.text = cards.Length.ToString();
    }

    private void SetStackCardsNumber(string[] cards)
    {
        stackCardsNumber.text = cards.Length.ToString();
    }

    private void SetGameType(GameType type)
    {
        switch (type)
        {
            case GameType.TWO_TRIPLES:
                gameType.text = "2 trojki";
                break;
            case GameType.SEQUENCE_TRIPLE:
                gameType.text = "serwer, trojka";
                break;
            case GameType.TWO_SEQUENCE:
                gameType.text = "2 serwery";
                break;
            case GameType.THREE_TRIPLES:
                gameType.text = "ULUBIONE !";
                break;
            case GameType.SEQUENCE_TWO_TRIPLES:
                gameType.text = "serwer, 2 trojki";
                break;
            case GameType.TWO_SEQUENCE_TRIPLE:
                gameType.text = "2 serwery, trojka";
                break;
            case GameType.THREE_SEQUENCE:
                gameType.text = "3 serwery";
                break;
            default:
                break;
        }
    }
}

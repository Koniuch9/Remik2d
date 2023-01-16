using TMPro;
using UnityEngine;

public class PlayerEnd : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject playerCardsContent;
    [SerializeField] private TextMeshProUGUI playerPoints;

    public void Init(string playerName, string[] playerCards, string playerPoints, bool doublePoints)
    {
        this.playerName.text = playerName;
        foreach (string cardDescription in playerCards)
        {
            CardFactory.instance.createEndCard(playerCardsContent.transform, cardDescription);
        }
        if (playerPoints.Equals("-100"))
        {
            this.playerPoints.color = new Color(0f, 1f, 0f);
        }
        else if (doublePoints)
        {
            this.playerPoints.color = new Color(1f, 0f, 0f);
        }
        else
        {
            this.playerPoints.color = new Color(1f, 1f, 1f);
        }
        this.playerPoints.text = playerPoints;
    }
}

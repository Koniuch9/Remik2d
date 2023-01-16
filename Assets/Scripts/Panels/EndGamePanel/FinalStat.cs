using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalStat : MonoBehaviour
{
    [SerializeField] private Image badge;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerPoints;

    public void Init(string name, Sprite badgeSprite, string points)
    {
        badge.sprite = badgeSprite;
        playerName.text = name;
        playerPoints.text = points;
    }
}

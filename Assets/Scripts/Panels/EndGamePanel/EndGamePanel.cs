using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject finalStats;
    [SerializeField] private Sprite firstPlaceSprite;
    [SerializeField] private Sprite secondPlaceSprite;
    [SerializeField] private Sprite thirdPlaceSprite;
    [SerializeField] private Sprite restSprite;
    [SerializeField] private GameObject finalStatPrefab;

    private void OnEnable()
    {
        string[] playersInOrder = ((string)PropsManager.instance.GetProp(Props.PLAYERS)).Split(',');
        List<PlayerClass> playersList = new List<PlayerClass>();
        foreach (string playerName in playersInOrder)
        {
            int playerPoints =
                ((string)PlayerPropsManager.instance.GetProp(playerName, PlayerProps.POINTS)).Split(',')
                .Select(s => int.Parse(s)).Aggregate(0, (acc, curr) => acc + curr);
            playersList.Add(new PlayerClass(playerName, playerPoints));
        }
        playersList.Sort((p1, p2) => p1.points - p2.points);

        for (int i = 0; i < playersList.Count; i++)
        {
            GameObject newStat = Instantiate(finalStatPrefab, finalStats.transform);
            FinalStat finalStat = newStat.GetComponent<FinalStat>();
            Sprite badge = restSprite;
            switch (i)
            {
                case 0:
                    badge = firstPlaceSprite;
                    break;
                case 1:
                    badge = secondPlaceSprite;
                    break;
                case 3:
                    badge = thirdPlaceSprite;
                    break;
                default:
                    break;
            }
            finalStat.Init(playersList[i].name, badge, playersList[i].points.ToString());
        }
    }

    private class PlayerClass
    {
        public string name;
        public int points;

        public PlayerClass(string name, int points)
        {
            this.name = name;
            this.points = points;
        }
    }

}

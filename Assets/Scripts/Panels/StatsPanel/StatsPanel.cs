using System;
using TMPro;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private GameObject playersHeader;
    [SerializeField] private GameObject statsCellPrefab;
    [SerializeField] private GameObject stats;

    private string winCharacter = "————";
    private string beginCharacter = "-";
    private bool playersUpdated = false;

    private void OnEnable()
    {
        StatsButton.OnStatsButtonClicked += ShowPanel;
        CloseStatsPanelButton.OnCloseStatsPanelButtonClicked += HidePanel;
        PlayerPropsManager.OnPlayerPointsUpdated += PlayerPointsUpdated;
        PropsManager.OnPlayersUpdated += PlayersUpdated;
    }

    private void OnDisable()
    {
        StatsButton.OnStatsButtonClicked -= ShowPanel;
        CloseStatsPanelButton.OnCloseStatsPanelButtonClicked -= HidePanel;
        PlayerPropsManager.OnPlayerPointsUpdated -= PlayerPointsUpdated;
        PropsManager.OnPlayersUpdated -= PlayersUpdated;
    }

    private void PlayersUpdated(string[] players)
    {
        if (playersUpdated) return;
        playersUpdated = true;
        int howManyPlayers = players.Length;
        foreach (Transform go in stats.transform)
        {
            if (go.GetSiblingIndex() >= howManyPlayers * 15)
            {
                go.gameObject.SetActive(false);
            }
        }
        foreach (Transform go in playersHeader.transform)
        {
            Destroy(go.gameObject);
        }
        foreach (string player in players)
        {
            GameObject cell = Instantiate(statsCellPrefab, playersHeader.transform);
            cell.GetComponentInChildren<TextMeshProUGUI>().text = player;
        }
        ;
        for (int j = 0, i = 0; j < players.Length; j++, i++)
        {
            int x = i;
            while (x <= 13)
            {
                stats.transform.GetChild(x + i + 14 * j).gameObject
                                .GetComponentInChildren<TextMeshProUGUI>().text = beginCharacter;
                x += players.Length;
            }
        }
    }

    private void PlayerPointsUpdated(string player, int[] points)
    {
        if (PropsManager.instance.GetProp(Props.PLAYERS) == null) return;
        string[] players = ((string)PropsManager.instance.GetProp(Props.PLAYERS))
            .Split(',', StringSplitOptions.RemoveEmptyEntries);
        int playerIndex = Array.IndexOf(players, player);
        int sum = 0;
        int sum2 = 0;
        for (int i = 0; i < points.Length; i++)
        {
            sum2 += points[i] == -100 ? 0 : points[i];
            stats.transform.GetChild(i + 15 * playerIndex).gameObject
                .GetComponentInChildren<TextMeshProUGUI>().text =
                points[i] == -100 ? winCharacter : sum2.ToString();
            sum += points[i];
        }
        stats.transform.GetChild(15 * (playerIndex + 1) - 1).gameObject
            .GetComponentInChildren<TextMeshProUGUI>().text = sum.ToString();
    }

    private void ShowPanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(500f, 0f, 0f), 0.5f);
    }

    private void HidePanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(-550f, 0f, 0f), 0.5f);
    }
}

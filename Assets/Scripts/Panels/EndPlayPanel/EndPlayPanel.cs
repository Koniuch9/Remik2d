using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EndPlayPanel : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject playersEnd;
    [SerializeField] private GameObject playerEndPrefab;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        continueButton.SetActive(false);
        playersEnd.SetActive(false);
        LeanTween.value(gameObject, ChangeImageOpacity, 0f, 1f, 2.5f).setOnComplete(EndPlay).setDelay(1.5f);
    }

    private void OnDisable()
    {
        image.color = new Color(0.1f, 0.1f, 0.1f, 0f);
    }

    private void ChangeImageOpacity(float value)
    {
        image.color = new Color(0.1f, 0.1f, 0.1f, value);
    }

    private void EndPlay()
    {
        continueButton.SetActive(true);
        playersEnd.SetActive(true);
        foreach (Transform go in playersEnd.transform)
        {
            Destroy(go.gameObject);
        }
        string[] playersInOrder = ((string)PropsManager.instance.GetProp(Props.PLAYERS)).Split(',');
        string playerWon = (string)PropsManager.instance.GetProp(Props.PLAYER_WON);
        int endTurn = (int)PropsManager.instance.GetProp(Props.PLAY_TURN);
        int meldTurn = (int)PlayerPropsManager.instance.GetProp(playerWon, PlayerProps.MELD_TURN);
        bool doublePoints = endTurn == meldTurn;
        Dictionary<string, Dictionary<PlayerProps, object>> players =
               new Dictionary<string, Dictionary<PlayerProps, object>>();
        foreach (string playerName in playersInOrder)
        {
            GameObject newPlayer = Instantiate(playerEndPrefab, playersEnd.transform);
            PlayerEnd playerEnd = newPlayer.GetComponent<PlayerEnd>();
            string[] playerCards =
                ((string)PlayerPropsManager.instance.GetProp(playerName, PlayerProps.CARDS))
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            string playerPoints = GameUtils.countPoints(playerCards, doublePoints).ToString();
            playerEnd.Init(playerName, playerCards, playerPoints, doublePoints);

            if (PhotonNetwork.IsMasterClient)
            {
                string playerAllPoints =
                    (string)PlayerPropsManager.instance.GetProp(playerName, PlayerProps.POINTS);
                players[playerName] = new Dictionary<PlayerProps, object>();
                players[playerName][PlayerProps.POINTS] =
                    playerAllPoints != null && playerAllPoints.Length == 0 ?
                    playerPoints : playerAllPoints + "," + playerPoints;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerPropsManager.instance.SetProps(players);
        }
    }
}

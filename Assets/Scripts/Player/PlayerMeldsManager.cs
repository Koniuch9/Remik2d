using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PlayerMeldsManager : MonoBehaviour
{
    public static event Action<string, string, Transform> OnAnimateCardAddedToPlayerMeld;
    private List<GameObject> meldList;
    private string playerName;
    private string[][] melds;

    private void Awake()
    {
        meldList = new List<GameObject>();
    }

    private void OnEnable()
    {
        PlayerPropsManager.OnPlayerMeldCardsUpdated += UpdatePlayerMelds;
        Meld.OnCardAddedToPlayerMeld += CardAddedToPlayerMeld;
    }


    private void OnDisable()
    {
        PlayerPropsManager.OnPlayerMeldCardsUpdated -= UpdatePlayerMelds;
        Meld.OnCardAddedToPlayerMeld -= CardAddedToPlayerMeld;
    }

    public void Init(string playerName)
    {
        this.playerName = playerName;
        Debug.Log("PN: " + playerName);
    }

    private void AddSequenceMeld()
    {
        GameObject newMeld =
            MeldFactory.instance.createSequenceMeld(
                transform, Guid.NewGuid().ToString("N"), false, true, playerName);
        meldList.Add(newMeld);
    }

    private void AddTripleMeld()
    {
        GameObject newMeld =
            MeldFactory.instance.createTripleMeld(
                transform, Guid.NewGuid().ToString("N"), false, true, playerName);
        meldList.Add(newMeld);
    }

    private void CardAddedToPlayerMeld(CardData cardData, int index, string playerName)
    {
        if (this.playerName.Equals(playerName))
        {
            string[][] meldCards = new string[meldList.Count][];
            for (int i = 0; i < meldList.Count; i++)
            {
                meldCards[i] = meldList[i].GetComponent<Meld>().getDescriptionList();
            }
            string meldResult = String.Join('#', meldCards.Select(sarr => String.Join(',', sarr)));
            PlayerPropsManager.instance.SetProp(playerName, PlayerProps.MELD_CARDS, meldResult);
        }
    }

    private void UpdatePlayerMelds(string playerName, string[][] meldCards)
    {
        if (this.playerName.Equals(playerName))
        {
            if (meldCards.Length > 1)
            {
                if (melds != null && melds.Length == meldCards.Length)
                {
                    for (int i = 0; i < meldCards.Length; i++)
                    {
                        if (melds[i].Length < meldCards[i].Length)
                        {
                            string odd = spotOddOneOut(melds[i], meldCards[i]);
                            string currentPlayerTurn = (string)PropsManager.instance.GetProp(Props.PLAYER_TURN);
                            if (!currentPlayerTurn.Equals("") && !currentPlayerTurn.Equals(PhotonNetwork.LocalPlayer.NickName))
                            {
                                OnAnimateCardAddedToPlayerMeld?.Invoke(currentPlayerTurn,
                                    odd, meldList[i].transform);
                            }
                            break;
                        }
                    }
                }
                if (meldList.Count != meldCards.Length)
                {
                    RebuildMelds((GameType)PropsManager.instance.GetProp(Props.GAME_TYPE));
                }
                for (int i = 0; i < meldCards.Length; i++)
                {
                    meldList[i].GetComponent<Meld>().UpdateCards(meldCards[i]);
                }
            }
            else
            {
                foreach (GameObject go in meldList)
                {
                    Destroy(go.gameObject);
                }
                meldList.Clear();
            }
            melds = meldCards;
        }
    }

    private string spotOddOneOut(string[] smaller, string[] bigger)
    {
        Dictionary<string, int> smallerDict = buildDict(smaller);
        Dictionary<string, int> biggerDict = buildDict(bigger);
        string odd = biggerDict.Except(smallerDict).First().Key;
        return odd;
    }

    private Dictionary<string, int> buildDict(string[] arr)
    {
        return arr.Aggregate(new Dictionary<string, int>(), (acc, curr) =>
        {
            if (acc.ContainsKey(curr))
            {
                acc[curr]++;
            }
            else
            {
                acc[curr] = 1;
            }
            return acc;
        });
    }

    private void RebuildMelds(GameType gameType)
    {
        foreach (GameObject go in meldList)
        {
            Destroy(go.gameObject);
        }
        meldList.Clear();
        foreach (char c in GameUtils.meldsFromType(gameType))
        {
            if (c == 'S') AddSequenceMeld();
            if (c == 'T') AddTripleMeld();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MeldsManager : MonoBehaviour
{
    public static event Action<string[][]> OnMeld;
    [SerializeField] private GameObject meldButton;
    private List<GameObject> meldList;

    private void Awake()
    {
        meldList = new List<GameObject>();
    }

    private void OnEnable()
    {
        Meld.OnCardAddedToMeld += MeldCardsChanged;
        Meld.OnCardRemovedFromMeld += MeldCardsChanged;
        PropsManager.OnGameTypeUpdated += RebuildMelds;
        MeldButton.OnMeldButtonClicked += DoMeld;
    }

    private void OnDisable()
    {
        Meld.OnCardAddedToMeld -= MeldCardsChanged;
        Meld.OnCardRemovedFromMeld -= MeldCardsChanged;
        PropsManager.OnGameTypeUpdated -= RebuildMelds;
        MeldButton.OnMeldButtonClicked -= DoMeld;
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

    private void AddSequenceMeld()
    {
        GameObject newMeld =
            MeldFactory.instance.createSequenceMeld(transform, Guid.NewGuid().ToString("N"), true, false);
        meldList.Add(newMeld);
    }

    private void AddTripleMeld()
    {
        GameObject newMeld =
            MeldFactory.instance.createTripleMeld(transform, Guid.NewGuid().ToString("N"), true, false);
        meldList.Add(newMeld);
    }

    private void DoMeld()
    {
        string[][] meldCards = new string[meldList.Count][];
        for (int i = 0; i < meldList.Count; i++)
        {
            meldCards[i] = meldList[i].GetComponent<Meld>().getDescriptionList();
        }
        string meldResult = String.Join('#', meldCards.Select(sarr => String.Join(',', sarr)));
        Dictionary<string, Dictionary<PlayerProps, object>> players =
               new Dictionary<string, Dictionary<PlayerProps, object>>();
        players[PhotonNetwork.LocalPlayer.NickName] = new Dictionary<PlayerProps, object>();
        players[PhotonNetwork.LocalPlayer.NickName][PlayerProps.MELD_CARDS] = meldResult;
        players[PhotonNetwork.LocalPlayer.NickName][PlayerProps.MELD_TURN] = (int)PropsManager.instance.GetProp(Props.PLAY_TURN);
        PlayerPropsManager.instance.SetProps(players);
        OnMeld?.Invoke(meldCards);
        meldButton.SetActive(false);
        foreach (GameObject go in meldList)
        {
            Destroy(go.gameObject);
        }
        meldList.Clear();
    }

    private void MeldCardsChanged(CardData cardData, int index)
    {
        meldButton.SetActive(CheckReadyToMeld());
    }

    private bool CheckReadyToMeld()
    {
        if ((int)PropsManager.instance.GetProp(Props.PLAY_TURN)
            <= PhotonNetwork.PlayerList.Length) return false;
        if (meldList.Count == 0) return false;
        foreach (GameObject go in meldList)
        {
            if (!go.GetComponent<Meld>().isReady()) return false;
        }
        return true;
    }
}

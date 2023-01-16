using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static event Action<bool> OnDrawCardActive;
    public static event Action<bool> OnMeldActive;
    public static event Action<bool> OnGiveCardActive;
    public static event Action<bool> OnStackCardActive;
    public static event Action<bool> OnEndTurnActive;

    private const string PLAYER_LOADED_LEVEL = "pll";

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playersPanel;
    [SerializeField] private PlayerCardsManager playerCardsManager;
    [SerializeField] private GameObject endPlayPanel;
    [SerializeField] private GameObject endGamePanel;

    private bool isGameStarted;
    private string[] players;
    private string nextPlayer = "";
    private int startTurn = 0;

    void Start()
    {
        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        isGameStarted = false;
    }

    // public override void OnJoinedRoom()
    // {
    //     Hashtable props = new Hashtable
    //         {
    //             {PLAYER_LOADED_LEVEL, true}
    //         };
    //     PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    //     isGameStarted = false;
    //     PropsManager.instance.PrintProps();
    // }

    public override void OnEnable()
    {
        base.OnEnable();
        PropsManager.OnGameTurnUpdated += NewGameTurn;
        PropsManager.OnPlayerTurnUpdated += NewPlayerTurn;
        TakeDeckCardButton.OnTakeDeckCardButtonClicked += CardTaken;
        StackButton.OnCardAddedToStack += CardStacked;
        StackCardsPanel.OnTakeAllFromStack += AllStackCardsTaken;
        StackCardsPanel.OnTakeFirstFromStack += FirstStackCardTaken;
        PropsManager.OnPlayerWon += PlayerWon;
        ContinueButton.OnContinueButtonClicked += ContinueButtonClicked;
        PlayerCardsManager.OnPlayerEndTurn += PlayerEndTurn;
        PlayerCardsManager.OnPlayerWon += NotifyPlayerWon;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PropsManager.OnGameTurnUpdated -= NewGameTurn;
        PropsManager.OnPlayerTurnUpdated -= NewPlayerTurn;
        TakeDeckCardButton.OnTakeDeckCardButtonClicked -= CardTaken;
        StackButton.OnCardAddedToStack -= CardStacked;
        StackCardsPanel.OnTakeAllFromStack -= AllStackCardsTaken;
        StackCardsPanel.OnTakeFirstFromStack -= FirstStackCardTaken;
        PropsManager.OnPlayerWon -= PlayerWon;
        ContinueButton.OnContinueButtonClicked -= ContinueButtonClicked;
        PlayerCardsManager.OnPlayerEndTurn -= PlayerEndTurn;
        PlayerCardsManager.OnPlayerWon -= NotifyPlayerWon;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayersLoadedLevel() && !isGameStarted)
            {
                StartGame();
            }
        }
    }

    private void PlayerWon(string playerName)
    {
        if (!playerName.Equals(""))
        {
            endPlayPanel.SetActive(true);
        }
    }

    private void NotifyPlayerWon()
    {
        PropsManager.instance.SetProp(Props.PLAYER_WON, PhotonNetwork.LocalPlayer.NickName);
    }

    private void ContinueButtonClicked()
    {
        bool showEndGamePanel = true;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            string[] points =
                ((string)PlayerPropsManager.instance.GetProp(p.NickName, PlayerProps.POINTS))
                .Split(',', StringSplitOptions.RemoveEmptyEntries);
            //TODO change to 14
            if (points.Length != 14)
            {
                showEndGamePanel = false;
                break;
            }
        }
        if (showEndGamePanel && (int)PropsManager.instance.GetProp(Props.GAME_TURN) == 13)
        {
            endGamePanel.SetActive(true);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
        endPlayPanel.SetActive(false);
    }

    private bool CheckAllPlayersLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out object playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }
            return false;
        }
        // TODO: REMOVE AFTER TEST
        // if (PhotonNetwork.PlayerList.Length == 2)
        // {
        //     return true;
        // }
        // return false;
        return true;
    }

    private void StartGame()
    {
        isGameStarted = true;
        InitGame();
    }

    private void NewGameTurn(int gameTurn)
    {
        if (gameTurn != startTurn) return;
        ClearPlayers();
        string[] playersInOrder = ((string)PropsManager.instance.GetProp(Props.PLAYERS)).Split(',');
        foreach (string player in playersInOrder)
        {
            SetupPlayer(player,
            GameUtils.cardsNumberFromType(GameUtils.typeFromTurn(gameTurn)),
            (string)PropsManager.instance.GetProp(Props.PLAYER_TURN));
        }
        int myIdx = Array.IndexOf(playersInOrder, PhotonNetwork.LocalPlayer.NickName);
        nextPlayer = playersInOrder[(myIdx + 1) % playersInOrder.Length];
    }

    private void NewPlayerTurn(string playerName)
    {
        if (playerName.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            OnDrawCardActive?.Invoke(true);
        }
    }

    private void AllStackCardsTaken(string[] cards)
    {
        OnDrawCardActive?.Invoke(false);
        OnStackCardActive?.Invoke(true);
        OnMeldActive?.Invoke(true);
        OnGiveCardActive?.Invoke(true);
    }

    private void FirstStackCardTaken(string firstCard)
    {
        OnDrawCardActive?.Invoke(false);
        OnStackCardActive?.Invoke(true);
        OnMeldActive?.Invoke(true);
        OnGiveCardActive?.Invoke(true);
    }

    private void CardTaken(string cardDescription)
    {
        OnDrawCardActive?.Invoke(false);
        OnStackCardActive?.Invoke(true);
        OnMeldActive?.Invoke(true);
        OnGiveCardActive?.Invoke(true);
    }

    private void CardStacked(CardData cardData, int index)
    {
        OnStackCardActive?.Invoke(false);
        OnMeldActive?.Invoke(false);
        OnGiveCardActive?.Invoke(false);
        AddStackCard(cardData);
    }

    private void AddStackCard(CardData cardData)
    {
        string[] stackCards = ((string)PropsManager.instance.GetProp(Props.STACK_CARDS))
            .Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (stackCards.Length == 1 && stackCards[0].Equals(""))
        {
            stackCards[0] = cardData.getDescription();
        }
        else
        {
            stackCards = stackCards.Append(cardData.getDescription()).ToArray();
        }
        PropsManager.instance.SetProp(Props.STACK_CARDS, String.Join(',', stackCards));
    }

    private void PlayerEndTurn()
    {
        Dictionary<Props, object> propsToSet = new Dictionary<Props, object>();
        propsToSet[Props.PLAYER_TURN] = nextPlayer;
        propsToSet[Props.PLAY_TURN] = (int)PropsManager.instance.GetProp(Props.PLAY_TURN) + 1;
        PropsManager.instance.SetProps(propsToSet);
    }

    private void InitGame()
    {
        Dictionary<string, Dictionary<PlayerProps, object>> players =
               new Dictionary<string, Dictionary<PlayerProps, object>>();
        Dictionary<Props, object> propsToSet = new Dictionary<Props, object>();

        string[] deck = GameUtils.GetDeckCards(
            (int)PropsManager.instance.GetProp(Props.NO_DECKS),
            (int)PropsManager.instance.GetProp(Props.NO_JOKERS)
        );

        deck = GameUtils.Shuffle(deck);

        object gameTurnFromProps = PropsManager.instance.GetProp(Props.GAME_TURN);
        int gameTurn;
        if (gameTurnFromProps == null)
        {
            gameTurn = startTurn;
        }
        else
        {
            gameTurn = (int)gameTurnFromProps + 1;
        }

        GameType gameType = GameUtils.typeFromTurn(gameTurn);

        int howManyCards = GameUtils.cardsNumberFromType(gameType);
        Dictionary<string, string> dealtCards;
        //TODO: REMOVE aftger tests
        if (true)
        {
            dealtCards = GameUtils.DealPlayers(deck, PhotonNetwork.PlayerList, howManyCards);
        }
        else
        {
            switch (gameTurn)
            {
                case 0:
                case 13:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,2D,2C,3D,3C,3H,XX,XX" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,XX,XX"},
                    };
                    break;
                case 1:
                case 12:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,3C,4C,3D,3C,3H,XX,XX,XX" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,XX,XX,XX"},
                    };
                    break;
                case 2:
                case 11:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,3C,4C,3D,4D,5D,6D,XX,XX,XX" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,7C,XX,XX,XX"},
                    };
                    break;
                case 3:
                case 10:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,2C,2C,3D,3D,3D,4D,XX,XX,XX,XX" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,7C,XX,XX,XX,XX"},
                    };
                    break;
                case 4:
                case 9:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,3C,4C,3D,3D,3D,4D,XX,XX,XX,XX,XX" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,7C,XX,XX,XX,XX,XX"},
                    };
                    break;
                case 5:
                case 8:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,3C,4C,3D,4D,5D,7H,XX,XX,XX,XX,XX,8H,9H" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,7C,XX,XX,XX,XX,XX,KH"},
                    };
                    break;
                case 6:
                case 7:
                    dealtCards = new Dictionary<string, string> {
                    { "Koniu", "2C,3C,4C,3D,4D,5D,7H,XX,XX,XX,XX,XX,8H,9H" },
                    { "Koniuclone0", "2C,3C,4C,5C,6C,3H,7C,XX,XX,XX,XX,XX,KH"},
                    };
                    break;
                default:
                    dealtCards = new Dictionary<string, string>();
                    break;
            }
            // dealtCards = new Dictionary<string, string> {
            // { "Koniu", "2C,2D,2C,3D,3C,3H,XX,XX" },
            // { "Koniuclone0", "2C,3C,4C,5C,6C,3H,XX,XX"},
            // // { "Koniuclone1", "2C,2D,2C,3D,3C,3H,XX,XX"}
            // };
        }
        string[] deckCardsAfterDeal = deck.Skip(PhotonNetwork.PlayerList.Length * howManyCards).ToArray();
        string deckCards = String.Join(',', deckCardsAfterDeal);
        string[] playersShuffled;

        if (gameTurn == startTurn)
        {
            playersShuffled =
                GameUtils.Shuffle(PhotonNetwork.PlayerList.Select(p => p.NickName).ToArray());
        }
        else
        {
            playersShuffled = ((string)PropsManager.instance.GetProp(Props.PLAYERS)).Split(',');
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            players[p.NickName] = new Dictionary<PlayerProps, object>();
            players[p.NickName][PlayerProps.MELD_CARDS] = "";
            players[p.NickName][PlayerProps.CARDS] = dealtCards[p.NickName];
            if (gameTurn == startTurn) players[p.NickName][PlayerProps.POINTS] = "";
        }

        propsToSet[Props.GAME_TURN] = gameTurn;
        propsToSet[Props.GAME_TYPE] = gameType;
        propsToSet[Props.DECK_CARDS] = deckCards;
        propsToSet[Props.STACK_CARDS] = "";
        propsToSet[Props.PLAYER_WON] = "";
        propsToSet[Props.PLAYER_DREW_CARD] = 0;
        propsToSet[Props.PLAYERS] = String.Join(',', playersShuffled);
        propsToSet[Props.PLAYER_TURN] = playersShuffled[gameTurn % playersShuffled.Length];
        propsToSet[Props.PLAY_TURN] = 1;

        PlayerPropsManager.instance.SetProps(players);
        PropsManager.instance.SetProps(propsToSet);
    }

    private void SetupPlayer(string playerName, int howManyCards, string playerNameTurn)
    {
        GameObject newPlayer = Instantiate(playerPrefab, playersPanel.transform);
        newPlayer.GetComponent<PlayerManager>().Init(playerName, howManyCards, playerNameTurn);
    }

    private void ClearPlayers()
    {
        foreach (Transform child in playersPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}

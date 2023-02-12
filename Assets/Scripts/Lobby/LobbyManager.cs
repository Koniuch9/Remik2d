// #define PARREL
#undef PARREL
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if (PARREL)
using ParrelSync;
#endif
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static string PLAYER_NAME_KEY = "name";
    public static string PLAYER_LAST_ROOM_NAME_KEY = "lastRoom";
    private static string PLAYER_READY = "pr";
    private static string PLAYER_LOADED_LEVEL = "pll";
    private bool isPlayerReady = false;

    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject CreateRoomPanel;
    [SerializeField] private GameObject InsideRoomPanel;

    [Header("Login Panel")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button loginButton;

    [Header("Main Panel")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private GameObject roomListContent;
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private Button leaveGameButton;
    [SerializeField] private Button logOutButton;

    [Header("Create Room Panel")]
    [SerializeField] private TMP_InputField deckNumberInput;
    [SerializeField] private TMP_InputField jokerNumberInput;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button confirmCreateRoomButton;
    [SerializeField] private Button backButton;

    [Header("Inside Room Panel")]
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitRoomButton;
    [SerializeField] private GameObject playersPanel;
    [SerializeField] private GameObject playerPanelPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
#if (PARREL)
        if (ClonesManager.IsClone())
        {
            PLAYER_NAME_KEY += ClonesManager.GetArgument();
        }
#endif
    }

    public override void OnEnable()
    {
        base.OnEnable();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        playerNameInput.onValueChanged.AddListener(OnPlayerNameInputChanged);
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        leaveGameButton.onClick.AddListener(OnLeaveGameButtonClicked);
        logOutButton.onClick.AddListener(OnLogoutButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        confirmCreateRoomButton.onClick.AddListener(OnConfirmCreateRoomButtonClicked);
        deckNumberInput.onValueChanged.AddListener(OnCreateRoomValuesChanged);
        jokerNumberInput.onValueChanged.AddListener(OnCreateRoomValuesChanged);
        roomNameInput.onValueChanged.AddListener(OnCreateRoomValuesChanged);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        quitRoomButton.onClick.AddListener(OnQuitRoomButtonClicked);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        playerNameInput.onValueChanged.RemoveListener(OnPlayerNameInputChanged);
        createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
        leaveGameButton.onClick.RemoveListener(OnLeaveGameButtonClicked);
        logOutButton.onClick.RemoveListener(OnLogoutButtonClicked);
        backButton.onClick.RemoveListener(OnBackButtonClicked);
        confirmCreateRoomButton.onClick.RemoveListener(OnConfirmCreateRoomButtonClicked);
        deckNumberInput.onValueChanged.RemoveListener(OnCreateRoomValuesChanged);
        jokerNumberInput.onValueChanged.RemoveListener(OnCreateRoomValuesChanged);
        roomNameInput.onValueChanged.RemoveListener(OnCreateRoomValuesChanged);
        readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        quitRoomButton.onClick.RemoveListener(OnQuitRoomButtonClicked);
    }

    void Start()
    {
        string playerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "");
        if (!playerName.Equals(""))
        {
            ConnectWithName(playerName);
        }
        else
        {
            SetActivePanel(LoginPanel.name);
        }
    }

    public override void OnConnectedToMaster()
    {
        SetActivePanel(MainPanel.name);
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddNewPlayer(newPlayer, playerListEntries.Count);
        startGameButton.interactable = CheckPlayersReady();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
        startGameButton.interactable = CheckPlayersReady();
    }

    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetString(PLAYER_LAST_ROOM_NAME_KEY, PhotonNetwork.CurrentRoom.Name);
        object isGameStarted = PropsManager.instance.GetProp(Props.GAME_STARTED);
        if (isGameStarted != null)
        {
            if ((bool)isGameStarted)
            {
                PhotonNetwork.LoadLevel("GameScene");
                return;
            }
        }
        SetActivePanel(InsideRoomPanel.name);
        isPlayerReady = false;
        readyButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Gotowy";
        cachedRoomList.Clear();

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            GameObject entry = AddNewPlayer(p, i);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerPanel>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        startGameButton.interactable = CheckPlayersReady();
        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerPanel>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        startGameButton.interactable = CheckPlayersReady();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.interactable = CheckPlayersReady();
        }
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(MainPanel.name);

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        int i = 0;
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            AddNewRoom(info, i);
            i++;
        }
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        if (PhotonNetwork.PlayerList.Length < 2 || PhotonNetwork.PlayerList.Length > 5) return false;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private GameObject AddNewPlayer(Player p, int position)
    {
        GameObject newPlayer = Instantiate(playerPanelPrefab);
        newPlayer.transform.SetParent(playersPanel.transform);
        newPlayer.transform.localScale = Vector3.one;
        newPlayer.GetComponent<PlayerPanel>().Init(PhotonNetwork.LocalPlayer.NickName, p.NickName, false);
        playerListEntries.Add(p.ActorNumber, newPlayer);
        return newPlayer;
    }

    private void AddNewRoom(RoomInfo info, int position)
    {
        GameObject entry = Instantiate(roomButtonPrefab);
        entry.transform.SetParent(roomListContent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<RoomButton>().Init(info.Name);
        roomListEntries.Add(info.Name, entry);
    }

    private void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    private void OnQuitRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnReadyButtonClicked()
    {
        isPlayerReady = !isPlayerReady;
        readyButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = isPlayerReady ? "Nie gotowy" : "Gotowy";
        Hashtable props = new Hashtable() { { PLAYER_READY, isPlayerReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void OnConfirmCreateRoomButtonClicked()
    {
        string roomName = roomNameInput.text;
        byte maxPlayers = 5;
        Hashtable props = new Hashtable {
            {((int)Props.NO_DECKS).ToString(), int.Parse(deckNumberInput.text)},
            {((int)Props.NO_JOKERS).ToString(), int.Parse(jokerNumberInput.text)}
        };
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 3000, CustomRoomProperties = props };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    private void OnCreateRoomValuesChanged(string value)
    {
        if (CheckCanRoomBeCreated())
        {
            confirmCreateRoomButton.interactable = true;
        }
        else
        {
            confirmCreateRoomButton.interactable = false;
        }
    }

    private bool CheckCanRoomBeCreated()
    {
        return !deckNumberInput.text.Equals("")
        && !jokerNumberInput.text.Equals("")
        && !roomNameInput.text.Equals("");
    }

    private void OnBackButtonClicked()
    {
        SetActivePanel(MainPanel.name);
    }

    private void OnLogoutButtonClicked()
    {
        PlayerPrefs.SetString(PLAYER_NAME_KEY, "");
        SetActivePanel(LoginPanel.name);
        PhotonNetwork.Disconnect();
    }

    private void OnLeaveGameButtonClicked()
    {
        Application.Quit();
    }

    private void OnCreateRoomButtonClicked()
    {
        SetActivePanel(CreateRoomPanel.name);
    }

    private void OnPlayerNameInputChanged(string value)
    {
        if (value.Length >= 3 && value.Length <= 12)
        {
            loginButton.interactable = true;
        }
        else
        {
            loginButton.interactable = false;
        }
    }

    private void OnLoginButtonClicked()
    {
        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerNameInput.text);
        ConnectWithName(playerNameInput.text);
    }

    private void ConnectWithName(string name)
    {
        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void SetActivePanel(string name)
    {
        LoginPanel.SetActive(name.Equals(LoginPanel.name));
        MainPanel.SetActive(name.Equals(MainPanel.name));
        CreateRoomPanel.SetActive(name.Equals(CreateRoomPanel.name));
        InsideRoomPanel.SetActive(name.Equals(InsideRoomPanel.name));
    }
}

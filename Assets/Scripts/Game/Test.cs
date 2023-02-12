#define PARREL
// #undef PARREL
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
#if (PARREL)
using ParrelSync;
#endif

public class Test : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        string playerName = "Koniu";
#if (PARREL)
        if (ClonesManager.IsClone())
        {
            playerName += ClonesManager.GetArgument();
        }
#endif
        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JOINED LOBBY TEST");
        Hashtable props = new Hashtable {
            {((int)Props.NO_DECKS).ToString(), 2},
            {((int)Props.NO_JOKERS).ToString(), 5}
        };
        RoomOptions options = new RoomOptions { MaxPlayers = 3, PlayerTtl = 3000, CustomRoomProperties = props };
        PhotonNetwork.JoinOrCreateRoom("room", options, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("JOINING ROOM FAILED: " + returnCode + ", " + message);
    }

    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetString(LobbyManager.PLAYER_LAST_ROOM_NAME_KEY, PhotonNetwork.CurrentRoom.Name);
        Debug.Log("JOINED ROOM");
    }
}

using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;

public class ReconnectManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    public static event Action OnTryingToReconnect;
    public static event Action OnFailedToReconnect;
    private LoadBalancingClient loadBalancingClient;
    private AppSettings appSettings;
    private bool isReconnecting = false;

    private void Start()
    {
        this.loadBalancingClient = PhotonNetwork.NetworkingClient;
        this.appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
        this.loadBalancingClient.AddCallbackTarget(this);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        TryAgainButton.OnTryAgainButtonClicked += NotQuickRecover;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        TryAgainButton.OnTryAgainButtonClicked -= NotQuickRecover;
        if (this.loadBalancingClient == null) return;
        this.loadBalancingClient.RemoveCallbackTarget(this);
    }

    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
    {
        isReconnecting = true;
        if (this.CanRecoverFromDisconnect(cause))
        {
            this.Recover();
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
            // TODO Remove after test
            case DisconnectCause.DisconnectByClientLogic:
                return true;
        }
        return false;
    }

    private void Recover()
    {
        OnTryingToReconnect?.Invoke();
        if (!loadBalancingClient.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!loadBalancingClient.ReconnectToMaster())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!loadBalancingClient.ConnectUsingSettings(appSettings))
                {
                    Debug.LogError("ConnectUsingSettings failed");
                    OnFailedToReconnect?.Invoke();
                }
            }
        }
    }

    private void NotQuickRecover()
    {
        OnTryingToReconnect?.Invoke();
        if (!loadBalancingClient.ReconnectToMaster())
        {
            Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
            if (!loadBalancingClient.ConnectUsingSettings(appSettings))
            {
                Debug.LogError("ConnectUsingSettings failed");
                OnFailedToReconnect?.Invoke();
            }
        }
    }

    public override void OnJoinedLobby()
    {
        if (isReconnecting && !GameManager.isDebug)
        {
            LeanTween.delayedCall(gameObject, 1f, () =>
            {
                Debug.Log("RECONNECTING TO ROOM");
                PhotonNetwork.JoinRoom(PlayerPrefs.GetString(LobbyManager.PLAYER_LAST_ROOM_NAME_KEY));
            });
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isReconnecting && !PhotonNetwork.InLobby && !GameManager.isDebug)
        {
            LeanTween.delayedCall(gameObject, 1f, () =>
            {
                Debug.Log("RECONNECTING JOINING LOBBY");
                PhotonNetwork.JoinLobby();
            });
        }
    }

    public override void OnJoinedRoom()
    {
        isReconnecting = false;
    }
}

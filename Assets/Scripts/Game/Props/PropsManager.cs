using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PropsManager : MonoBehaviourPunCallbacks
{
    public static event Action<string[]> OnDeckCardsUpdated;
    public static event Action<GameType> OnGameTypeUpdated;
    public static event Action<int> OnGameTurnUpdated;
    public static event Action<string> OnPlayerTurnUpdated;
    public static event Action<bool> OnPlayerDrewCardUpdated;
    public static event Action<string[]> OnPlayersUpdated;
    public static event Action<string[]> OnStackCardsUpdated;
    public static event Action<string> OnPlayerWon;

    public static PropsManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance == this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public Hashtable GetRoomProperties()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        PrintProps();
        foreach (object key in propertiesThatChanged.Keys)
        {
            if (!(key is string)) continue;
            object prop = propertiesThatChanged[key];
            if (int.TryParse((string)key, out int x))
            {
                switch ((Props)x)
                {
                    case Props.DECK_CARDS:
                        OnDeckCardsUpdated?.Invoke(((string)prop).Split(',', StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case Props.GAME_TURN:
                        OnGameTurnUpdated?.Invoke((int)prop);
                        break;
                    case Props.GAME_TYPE:
                        OnGameTypeUpdated?.Invoke((GameType)((int)prop));
                        break;
                    case Props.PLAYER_TURN:
                        OnPlayerTurnUpdated?.Invoke((string)prop);
                        break;
                    case Props.PLAYER_DREW_CARD:
                        OnPlayerDrewCardUpdated?.Invoke((bool)prop);
                        break;
                    case Props.PLAYER_WON:
                        OnPlayerWon?.Invoke((string)prop);
                        break;
                    case Props.PLAYERS:
                        OnPlayersUpdated?.Invoke(((string)prop).Split(',', StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case Props.STACK_CARDS:
                        OnStackCardsUpdated?.Invoke(((string)prop).Split(',', StringSplitOptions.RemoveEmptyEntries));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void SetProp(Props prop, object value)
    {
        Debug.Log("setting prop: " + prop.ToString() + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds());
        Hashtable props = new Hashtable();
        props[((int)prop).ToString()] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void SetProps(Dictionary<Props, object> values)
    {
        Hashtable props = new Hashtable();
        foreach (Props prop in values.Keys)
        {
            props[((int)prop).ToString()] = values[prop];
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public object GetProp(Props prop)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[((int)prop).ToString()];
    }

    public void PrintProps()
    {
        string s = "";
        foreach (string key in PhotonNetwork.CurrentRoom.CustomProperties.Keys)
        {
            // if (PhotonNetwork.CurrentRoom.CustomProperties[key] is Dictionary<string, Dictionary<string, object>>)
            // {
            //     s += key + ": \n";
            //     Dictionary<string, Dictionary<string, object>> dict =
            //     (Dictionary<string, Dictionary<string, object>>)PhotonNetwork.CurrentRoom.CustomProperties[key];
            //     foreach (string key2 in dict.Keys)
            //     {
            //         s += key2 + ":\n";
            //         foreach (string key3 in dict[key2].Keys)
            //         {
            //             s += key3 + ": " + dict[key2][key3] + "\n";
            //         }
            //     }
            // }
            // else
            // {
            if (int.TryParse(key, out int x))
            {
                s += ((Props)x).ToString()
                    + ": "
                    + PhotonNetwork.CurrentRoom.CustomProperties[key].ToString()
                    + "\n";
            }
            else
            {
                s += key
                    + ": "
                    + PhotonNetwork.CurrentRoom.CustomProperties[key].ToString()
                    + "\n";
            }

            // }

        }
        Debug.Log("custom props:\n" + s);
    }
}

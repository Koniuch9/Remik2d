using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PlayerPropsManager : MonoBehaviourPunCallbacks
{

    public static event Action<string, string[]> OnPlayerCardsUpdated;
    public static event Action<string, string[][]> OnPlayerMeldCardsUpdated;
    public static event Action<string, int[]> OnPlayerPointsUpdated;

    public static PlayerPropsManager instance { get; private set; }

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

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        PropsManager.instance.PrintProps();
        foreach (object key in propertiesThatChanged.Keys)
        {
            if (!(key is string)) continue;
            string[] keyParts = getNameAndKey((string)key);
            object prop = propertiesThatChanged[key];
            if (keyParts != null)
            {
                switch ((PlayerProps)(int.Parse(keyParts[1])))
                {
                    case PlayerProps.CARDS:
                        OnPlayerCardsUpdated?.Invoke(keyParts[0], ((string)prop).Split(',', StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case PlayerProps.MELD_CARDS:
                        OnPlayerMeldCardsUpdated?.Invoke(keyParts[0],
                            ((string)prop).Split('#').Select(str => str.Split(',')).ToArray());
                        break;
                    case PlayerProps.POINTS:
                        OnPlayerPointsUpdated?.Invoke(keyParts[0],
                            ((string)prop).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void SetProp(string playerName, PlayerProps prop, object value)
    {
        Hashtable props = new Hashtable();
        props[playerName + "_" + ((int)prop).ToString()] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public void SetProps(Dictionary<string, Dictionary<PlayerProps, object>> values)
    {
        Hashtable props = new Hashtable();
        foreach (string key in values.Keys)
        {
            foreach (PlayerProps prop in values[key].Keys)
            {
                props[key + "_" + ((int)prop).ToString()] = values[key][prop];
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }


    public object GetProp(string playerName, PlayerProps prop)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[playerName + "_" + ((int)prop).ToString()];
    }

    private string[] getNameAndKey(string propKey)
    {
        string[] parts = propKey.Split("_");
        if (parts.Length == 2)
        {
            return parts;
        }
        return null;
    }
}

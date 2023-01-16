using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Button button;

    private void Awake()
    {
        text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        button = gameObject.GetComponent<Button>();
    }

    public void Init(string roomName)
    {
        text.text = roomName;
        button.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            PhotonNetwork.JoinRoom(roomName);
        });
    }
}

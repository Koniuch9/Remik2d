using Photon.Pun;
using UnityEngine;

public class PopupManager : MonoBehaviour
{

    [SerializeField] private GameObject infoPopup;
    [SerializeField] private GameObject stackCardsPopup;
    [SerializeField] private GameObject endGamePopup;
    [SerializeField] private GameObject gameStatsPopup;

    private void OnEnable()
    {
        PropsManager.OnPlayerTurnUpdated += ShowPlayerTurnPopup;
    }

    private void OnDisable()
    {
        PropsManager.OnPlayerTurnUpdated -= ShowPlayerTurnPopup;
    }

    private void ShowPlayerTurnPopup(string playerName)
    {
        if (playerName.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            InfoPopupBehaviour.infoText = "Twoja tura";
            infoPopup.SetActive(true);
        }
    }
}

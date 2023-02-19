using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReconnectPanel : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private GameObject tryAgainButton;
    [SerializeField] private TextMeshProUGUI text;
    private float rpm = 150f;

    private void OnEnable()
    {
        text.text = "Ponowne łączenie...";
        ReconnectManager.OnFailedToReconnect += ReconnectFailed;
        ReconnectManager.OnTryingToReconnect += Reconnecting;
    }

    private void OnDisable()
    {
        ReconnectManager.OnFailedToReconnect -= ReconnectFailed;
        ReconnectManager.OnTryingToReconnect -= Reconnecting;
    }

    private void Reconnecting()
    {
        text.text = "Ponowne łączenie...";
        tryAgainButton.SetActive(false);
    }

    private void ReconnectFailed()
    {
        text.text = "Nie udalo sie polaczyc, sprobuj ponownie";
        tryAgainButton.SetActive(true);
    }

    private void Update()
    {
        loadingImage.transform.Rotate(0, 0, -rpm * Time.deltaTime);
    }
}

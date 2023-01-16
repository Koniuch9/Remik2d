using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public Color myPlayerColor;
    public Sprite readySrpite;
    public Sprite notReadySprite;

    [SerializeField] private Image rightImage;
    private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI playerName; 
    private string myPlayerName;

    private void Awake() {
        backgroundImage = gameObject.GetComponent<Image>();
        Debug.Log("backImag: " + backgroundImage);
    }

    public void Init(string myPlayerName, string playerName, bool isReady) {
        this.myPlayerName = myPlayerName;
        this.playerName.text = playerName;
        this.rightImage.sprite = isReady ? readySrpite : notReadySprite;
        if (myPlayerName.Equals(playerName)) {
            backgroundImage.color = myPlayerColor;
        }
    }

    public void SetPlayerReady(bool isReady) {
        this.rightImage.sprite = isReady ? readySrpite : notReadySprite;
    }
}

using UnityEngine;

public class MeldPanel : MonoBehaviour
{
    private void OnEnable()
    {
        ShowMeldPanelButton.OnShowMeldButtonClicked += ShowPanel;
        CloseMeldPanelButton.OnCloseMeldPanelButtonClicked += HidePanel;
        MeldsManager.OnMeld += MeldHappened;
    }

    private void OnDisable()
    {
        ShowMeldPanelButton.OnShowMeldButtonClicked -= ShowPanel;
        CloseMeldPanelButton.OnCloseMeldPanelButtonClicked -= HidePanel;
        MeldsManager.OnMeld -= MeldHappened;
    }
    private void ShowPanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(0f, 200f, 0f), 0.2f);
    }

    private void HidePanel()
    {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(0f, -250f, 0f), 0.2f);
    }

    private void MeldHappened(string[][] meldCards)
    {
        HidePanel();
    }
}

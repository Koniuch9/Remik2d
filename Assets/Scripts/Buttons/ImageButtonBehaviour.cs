using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color dimmColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color lightColor = new Color(1f, 1f, 1f, 1f);
    private Image image;

    void Start() {
        image = gameObject.GetComponent<Image>();
        image.color = lightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween
        .value(gameObject, UpdateColor, dimmColor, lightColor, 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween
        .value(gameObject, UpdateColor, lightColor, dimmColor, 0.1f) ;
    }
    private void UpdateColor(Color color) {
        image.color = color;
    }
}

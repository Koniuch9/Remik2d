using UnityEngine;

public class CardData : MonoBehaviour
{
    public string type;
    public string value;

    public CardData(string value, string type)
    {
        this.type = type;
        this.value = value;
    }

    public string getDescription()
    {
        return "" + value + type;
    }
}

using System.Linq;

public class TripleMeld : Meld
{
    public override bool canDropAll(CardData cardData)
    {
        if (cardDataList.Count == 0)
        {
            return !cardData.value.Equals("X") && !cardData.type.Equals("X");
        }
        return
            (cardData.value.Equals("X") &&
            cardData.type.Equals("X")) ||
            cardData.value.Equals("" + cardDataList.Last()[0]);
    }

    public override bool canDropLeft(CardData cardData)
    {
        return false;
    }

    public override bool canDropRight(CardData cardData)
    {
        return false;
    }

    public override bool canRemove(CardData cardData, int index)
    {
        return allowRemove;
    }

    public override bool isReady()
    {
        return cardDataList.Count >= 3;
    }

    protected override string GetInternalData(CardData cardData, bool left)
    {
        if (cardDataList.Count > 0)
        {
            return cardDataList[0][0] + "";
        }
        return cardData.value + "";
    }

    protected override string GetInternalData(string cardDescription, bool left)
    {
        if (cardDataList.Count > 0)
        {
            return cardDataList[0][0] + "";
        }
        return cardDescription;
    }
}

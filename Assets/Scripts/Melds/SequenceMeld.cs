using System.Linq;
using UnityEngine;

public class SequenceMeld : Meld
{
    public override bool canDropAll(CardData cardData)
    {
        return cardDataList.Count == 0 && !cardData.value.Equals("X") && !cardData.type.Equals("X");
    }

    public override bool canDropLeft(CardData cardData)
    {
        if (cardDataList.Count > 0)
        {
            string firstValue = "" + cardDataList[0][0];
            string firstType = "" + cardDataList[0][1];
            if (cardData.value.Equals("X") && cardData.type.Equals("X"))
            {
                return true;
            }
            int diff = CardUtils.sequenceMap[firstValue] - CardUtils.sequenceMap[cardData.value];
            return cardData.type.Equals(firstType) && (diff == 1 || diff == -12);
        }
        return false;
    }

    public override bool canDropRight(CardData cardData)
    {
        if (cardDataList.Count > 0)
        {
            string lastValue = "" + cardDataList.Last()[0];
            string lastType = "" + cardDataList.Last()[1];
            if (cardData.value.Equals("X") && cardData.type.Equals("X"))
            {
                return true;
            }
            int diff = CardUtils.sequenceMap[lastValue] - CardUtils.sequenceMap[cardData.value];
            return cardData.type.Equals(lastType) && (diff == -1 || diff == 12);
        }
        return false;
    }

    public override bool canRemove(CardData cardData, int index)
    {
        return allowRemove && (index == cardDataList.Count - 1 || index == 0);
    }

    public override bool isReady()
    {
        return cardDataList.Count >= 4;
    }

    protected override string GetInternalData(CardData cardData, bool left)
    {
        if (cardDataList.Count > 0)
        {
            if (left)
            {
                return CardUtils.sequenceMinusOneMap["" + cardDataList[0][0]] + cardDataList[0][1];
            }
            else
            {
                return CardUtils.sequencePlusOneMap["" + cardDataList.Last()[0]] + cardDataList.Last()[1];
            }
        }
        return cardData.value + cardData.type;
    }

    protected override string GetInternalData(string cardDescription, bool left)
    {
        if (cardDataList.Count > 0)
        {
            if (left)
            {
                return CardUtils.sequenceMinusOneMap["" + cardDataList[0][0]] + cardDataList[0][1];
            }
            else
            {
                return CardUtils.sequencePlusOneMap["" + cardDataList.Last()[0]] + cardDataList.Last()[1];
            }
        }
        return cardDescription;
    }
}

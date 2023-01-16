using System;

public class LeftHandler : Handler
{
    public static event Action<string, CardData, int> OnLeftHandlerCardAdded;
    protected override void Handle(CardData cardData, int index)
    {
        OnLeftHandlerCardAdded?.Invoke(meldId, cardData, index);
    }
}

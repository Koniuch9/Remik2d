using System;

public class RightHandler : Handler
{
    public static event Action<string, CardData, int> OnRightHandlerCardAdded;
    protected override void Handle(CardData cardData, int index)
    {
        OnRightHandlerCardAdded?.Invoke(meldId, cardData, index);
    }
}

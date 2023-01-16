using System;

public class AllHandler : Handler
{
    public static event Action<string, CardData, int> OnAllHandlerCardAdded;
    protected override void Handle(CardData cardData, int index)
    {
        OnAllHandlerCardAdded?.Invoke(meldId, cardData, index);
    }
}

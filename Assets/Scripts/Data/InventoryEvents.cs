using System;

public static class InventoryEvents
{
    public static event Action<InventoryDropContext> OnItemDropped;

    public static void OnDrop(InventoryDropContext context)
    {
        OnItemDropped?.Invoke(context);
    }

    private static void Reset()
    {
        OnItemDropped = null;
    }
}
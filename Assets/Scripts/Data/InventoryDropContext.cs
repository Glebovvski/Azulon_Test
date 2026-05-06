using UnityEngine.UIElements;

public enum InventoryArea
{
    None,
    World,
    PlayerInventory
}

public readonly struct InventoryDropContext
{
    public readonly IInventoryData Item;
    public readonly VisualElement ItemElement;
    public readonly VisualElement OldSlot;
    public readonly VisualElement NewSlot;
    public readonly InventoryArea From;
    public readonly InventoryArea To;

    public InventoryDropContext(
        IInventoryData item,
        VisualElement itemElement,
        VisualElement oldSlot,
        VisualElement newSlot,
        InventoryArea from,
        InventoryArea to)
    {
        Item = item;
        ItemElement = itemElement;
        OldSlot = oldSlot;
        NewSlot = newSlot;
        From = from;
        To = to;
    }
}
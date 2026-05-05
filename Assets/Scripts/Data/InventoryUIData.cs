using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct InventoryUIData 
{
    public InventoryData InventoryData { get; private set; }
    public VisualElement Slot { get; private set; }
    public bool IsEmpty { get; private set; }
    public int SlotIndex { get; private set; }

    public InventoryUIData(InventoryData _inventoryData, VisualElement _slot, bool _isEmpty, int _index)
    {
        InventoryData = _inventoryData;
        Slot = _slot;
        IsEmpty = _isEmpty;
        SlotIndex = _index;
    }
}

using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    protected const string INVENTORY_GRID_CLASS = "inventory";
    protected const string WORLD_GRID_CLASS = "world";

    [SerializeField] protected InventoryListScriptableData dataList;
    [SerializeField] protected UIDocument uiDoc;
    [SerializeField] protected List<InventoryUIData> inventoryUIData;
    protected VisualElement root;
    protected VisualElement panel;
    protected VisualElement dragPanel;

    public event Action<IInventoryData> OnAddToInventory;
    public event Action<IInventoryData> OnRemoveFromInventory;


    void OnEnable()
    {
        inventoryUIData = new();
        root = uiDoc.rootVisualElement;
        dragPanel = root.Q(className: "drag-layer");
        FillInventory();
    }

    protected abstract void FillInventory();

    protected VisualElement CreateSlot()
    {
        VisualElement slot = new VisualElement();
        slot.pickingMode = PickingMode.Ignore;
        slot.AddToClassList("slot");
        slot.name = "slot";

        return slot;
    }


    protected VisualElement CreateItem(IInventoryData item)
    {
        VisualElement itemEl = new VisualElement();
        itemEl.AddToClassList("item");
        itemEl.name = "item";

        Image image = new Image();
        image.AddToClassList("slot-image");
        image.pickingMode = PickingMode.Ignore;

        Label amount = new Label();
        amount.AddToClassList("slot-amount");
        amount.pickingMode = PickingMode.Ignore;

        itemEl.name = item.Data.name;

        image.Add(amount);
        itemEl.Add(image);

        itemEl.dataSource = item;

        image.SetBinding("sprite", new DataBinding
        {
            dataSourcePath = new PropertyPath("Data.icon")
        });

        amount.SetBinding("text", new DataBinding
        {
            dataSourcePath = new PropertyPath("Amount")
        });

        return itemEl;
    }


    protected virtual void OnDrop(VisualElement data, VisualElement oldSlot, VisualElement newSlot)
    {
        if (data == null || newSlot == null || oldSlot == null)
        {
            Debug.LogError($"Something went wrong on drag {data.userData} {newSlot} {oldSlot}");
            return;
        }

        IInventoryData inventoryData = data.dataSource as IInventoryData;
        if (inventoryData == null)
        {
            Debug.LogError($"No InventoryData: {data.name}");
            return;
        }

        InventoryArea from = GetInventoryArea(oldSlot);
        InventoryArea to = GetInventoryArea(newSlot);

        InventoryDropContext context = new InventoryDropContext(inventoryData, data, oldSlot, newSlot, from, to);
        InventoryEvents.OnDrop(context);
    }

    private InventoryArea GetInventoryArea(VisualElement slot)
    {
        if (slot == null || slot.parent == null)
            return InventoryArea.None;

        VisualElement grid = slot.parent;

        if (grid.ClassListContains(WORLD_GRID_CLASS))
            return InventoryArea.World;

        if (grid.ClassListContains(INVENTORY_GRID_CLASS))
            return InventoryArea.PlayerInventory;

        return InventoryArea.None;
    }
}

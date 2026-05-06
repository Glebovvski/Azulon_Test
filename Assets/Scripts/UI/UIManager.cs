using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    protected const string INVENTORY_GRID_CLASS = "inventory";
    protected const string WORLD_GRID_CLASS = "world";
    [SerializeField] protected InventoryListScriptableData dataList;
    [SerializeField] protected UIDocument uiDoc;
    protected VisualElement root;
    protected VisualElement panel;
    protected VisualElement dragPanel;

    public event Action<IInventoryData> OnAddToInventory;
    public event Action<IInventoryData> OnRemoveFromInventory;

    private const int SlotsPerRow = 8;


    void OnEnable()
    {
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

        amount.text = item.Amount.ToString();

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
        if (to == InventoryArea.PlayerInventory)
        {
            Match3Horizontal(data);
            Match3Vertical(data);
        }
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

    private void Match3Horizontal(VisualElement item, int matchAmount = 3)
    {
        VisualElement inventory = root.Q(className: INVENTORY_GRID_CLASS);
        IInventoryData data = item.dataSource as IInventoryData;
        int key = data.Data.key;
        bool matching = false;
        List<VisualElement> match = new();
        var slots = inventory.Query(className: "slot").ToList();

        for (int i = 0; i < slots.Count; i++)
        {
            var itemInSlot = slots[i].Q(className: "item");
            if (itemInSlot == null)
            {
                matching = false;
                if (match.Count < matchAmount)
                {
                    match.Clear();
                }
                continue;
            }
            var itemData = itemInSlot.dataSource as IInventoryData;
            if (itemData.Data.key == key)
            {
                match.Add(itemInSlot);
                matching = true;
            }
            else
            {
                if (match.Count < matchAmount)
                {
                    match.Clear();
                }
                matching = false;
            }
        }

        if (match.Count < matchAmount)
            return;

        ResolveMatch(match, matchAmount);
    }

    private void Match3Vertical(VisualElement item, int matchAmount = 3)
    {
        VisualElement inventory = root.Q(className: INVENTORY_GRID_CLASS);

        if (inventory == null || item == null)
            return;

        IInventoryData data = item.dataSource as IInventoryData;

        if (data == null || data.Data == null)
            return;

        int key = data.Data.key;

        List<VisualElement> slots = inventory.Query(className: "slot").ToList();

        int rows = Mathf.CeilToInt(slots.Count / (float)SlotsPerRow);

        for (int column = 0; column < SlotsPerRow; column++)
        {
            List<VisualElement> match = new List<VisualElement>();

            for (int row = 0; row < rows; row++)
            {
                int index = row * SlotsPerRow + column;

                if (index >= slots.Count)
                {
                    ResolveMatch(match, matchAmount);
                    match.Clear();
                    break;
                }

                VisualElement slot = slots[index];
                VisualElement itemInSlot = slot.Q(className: "item");

                if (itemInSlot == null)
                {
                    ResolveMatch(match, matchAmount);
                    match.Clear();
                    continue;
                }

                IInventoryData itemData = itemInSlot.dataSource as IInventoryData;

                if (itemData != null && itemData.Data != null && itemData.Data.key == key)
                {
                    match.Add(itemInSlot);
                }
                else
                {
                    ResolveMatch(match, matchAmount);
                    match.Clear();
                }
            }

            ResolveMatch(match, matchAmount);
        }
    }

    private bool ResolveMatch(List<VisualElement> match, int matchAmount)
    {
        if (match == null || match.Count < matchAmount)
            return false;

        int totalAmount = 0;

        for (int i = 0; i < match.Count; i++)
        {
            IInventoryData data = match[i].dataSource as IInventoryData;

            if (data != null)
                totalAmount += data.Amount;
        }

        VisualElement firstItem = match[0];

        Label firstMatchedLabel = firstItem.Q<Label>(className: "slot-amount");

        if (firstMatchedLabel != null)
            firstMatchedLabel.text = totalAmount.ToString();

        for (int i = 1; i < match.Count; i++)
        {
            match[i].dataSource = null;
            match[i].RemoveFromHierarchy();
        }

        return true;
    }
}

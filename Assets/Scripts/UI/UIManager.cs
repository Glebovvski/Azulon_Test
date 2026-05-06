using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIManager : MonoBehaviour
{
    private const int DefaultAmountOfItemsInSlot = 1;

    [SerializeField] protected InventoryListScriptableData dataList;
    [SerializeField] protected UIDocument uiDoc;
    [SerializeField] protected List<InventoryUIData> inventoryUIData;
    protected VisualElement root;
    protected VisualElement panel;
    protected VisualElement dragPanel;


    void OnEnable()
    {
        inventoryUIData = new();
        root = uiDoc.rootVisualElement;
        dragPanel = root.Q(className:"drag-layer");
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

    protected VisualElement CreateItem(InventoryScriptableData item)
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

        image.Add(amount);
        itemEl.Add(image);

        amount.text = $"{DefaultAmountOfItemsInSlot}";
        image.sprite = item.icon;

        itemEl.userData = item;
        return itemEl;
    }


    protected VisualElement CreateItem(InventoryData item)
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

        image.Add(amount);
        itemEl.Add(image);

        amount.text = $"{item.amount}";
        image.sprite = item.data.icon;

        itemEl.userData = item.data;
        return itemEl;
    }
}

using System.Collections.Generic;
using Unity.Properties;
using UnityEditor.UIElements;
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

        itemEl.name = item.data.name;

        image.Add(amount);
        itemEl.Add(image);

        itemEl.dataSource = item;

        image.SetBinding("sprite", new DataBinding
        {
            dataSourcePath = new PropertyPath("data.icon")
        });

        amount.SetBinding("text", new DataBinding
        {
            dataSourcePath = new PropertyPath("amount")
        });

        // amount.text = $"{item.amount}";
        // image.sprite = item.data.icon;

        // itemEl.userData = item.data;
        // itemEl.dataSource = item.data;
        return itemEl;
    }


    protected void OnDrop(VisualElement data, VisualElement newSlot)
    {
        if (data == null || newSlot == null)
        {
            Debug.LogError($"Something went wrong on drag {data.userData} {newSlot}");
            return;
        }
    }
}

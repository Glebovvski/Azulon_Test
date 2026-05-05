using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldUIManager : MonoBehaviour
{
    private const int DefaultAmountOfItemsInSlot = 1;
    [SerializeField] private InventoryListScriptableData worldList;
    [SerializeField] private UIDocument uiDoc;
    private VisualElement root;
    private VisualElement world;

    [SerializeField] private List<InventoryUIData> inventoryUIData;

    void OnEnable()
    {
        inventoryUIData = new();
        root = uiDoc.rootVisualElement;
        FillWorldInventory();
    }

    private void FillWorldInventory()
    {
        world = root.Q(className: "world");
        foreach (var item in worldList.List)
        {
            for (int j = 0; j < item.amount; j++)
            {
                var slot = CreateSlot();
                var inventoryItem = CreateItem(item.data);
                slot.Add(inventoryItem);
                inventoryItem.AddManipulator(new InventoryManipulator(inventoryItem, slot, world, null));
                world.Add(slot);
                inventoryUIData.Add(new InventoryUIData(item, slot, false, inventoryUIData.Count));
            }
        }
    }

    private VisualElement CreateSlot()
    {
        VisualElement slot = new VisualElement();
        slot.pickingMode = PickingMode.Ignore;
        slot.AddToClassList("slot");
        slot.name="slot";

        return slot;
    }

    private VisualElement CreateItem(InventoryScriptableData item)
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
}

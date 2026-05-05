using System;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldUIManager : MonoBehaviour
{
    private const int DefaultAmountOfItemsInSlot = 1;
    [SerializeField] private InventoryListScriptableData worldList;
    [SerializeField] private UIDocument uiDoc;
    private VisualElement root;
    private VisualElement world;

    void OnEnable()
    {
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
                slot.Add(CreateItem(item.data));
                world.Add(slot);
            }
        }
    }

    private VisualElement CreateSlot()
    {
        VisualElement slot = new VisualElement();
        slot.pickingMode = PickingMode.Ignore;
        slot.AddToClassList("slot");
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

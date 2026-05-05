using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private int inventroySize = 64;
    [SerializeField] private InventoryListScriptableData defaultInventoryList;
    [SerializeField] private UIDocument uiDoc;
    private VisualElement root;
    private VisualElement inventory;

    void OnEnable()
    {
        root = uiDoc.rootVisualElement;
        FillPlayerInventory();
    }

    private void FillPlayerInventory()
    {
        inventory = root.Q(className: "inventory");
        for (int i = 0; i < inventroySize; i++)
        {
            var slot = CreateSlot();
            inventory.Add(slot);
        }
        var slots = inventory.Query<VisualElement>(className: "slot").ToList();
        for (int i = 0; i < inventroySize; i++)
        {
            if (i < defaultInventoryList.List.Count)
                slots[i].Add(CreateItem(defaultInventoryList.List[i]));
            else
                break;
        }
    }

    private VisualElement CreateSlot()
    {
        VisualElement slot = new VisualElement();
        slot.pickingMode = PickingMode.Ignore;
        slot.AddToClassList("slot");
        return slot;
    }

    private VisualElement CreateItem(InventoryData item)
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

        itemEl.userData = item;
        return itemEl;
    }
}

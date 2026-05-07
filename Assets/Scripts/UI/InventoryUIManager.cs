using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIManager : UIManager
{
    [SerializeField] private int inventroySize = 64;

    protected override void FillInventory()
    {
        mainPanel = root.Q("PlayerInventory");
        title = mainPanel.Q(className: "title");
        titleLabel = mainPanel.Q(className: "title-label");
        RegisterHoverForMainPanel();
        panel = root.Q(className: INVENTORY_GRID_CLASS);
        for (int i = 0; i < inventroySize; i++)
        {
            var slot = CreateSlot();
            panel.Add(slot);
        }
        var slots = panel.Query<VisualElement>(className: "slot").ToList();
        for (int i = 0; i < inventroySize; i++)
        {
            if (i < dataList.List.Count)
            {
                var inventoryItem = CreateItem(dataList.List[i]);
                inventoryItem.AddManipulator(new InventoryManipulator(inventoryItem, dragPanel, OnDrop));
                slots[i].Add(inventoryItem);
            }
            else
                break;
        }
    }
}

using UnityEngine.UIElements;

public class WorldUIManager : UIManager
{
    protected override void FillInventory()
    {
        mainPanel = root.Q("WorldInventory");
        title = mainPanel.Q(className: "title");
        titleLabel = mainPanel.Q(className: "title-label");
        RegisterHoverForMainPanel();
        panel = root.Q(className: WORLD_GRID_CLASS);
        foreach (var item in dataList.List)
        {
            for (int j = 0; j < item.Amount; j++)
            {
                var slot = CreateSlot();
                var inventoryItem = CreateItem(item);
                slot.Add(inventoryItem);
                inventoryItem.AddManipulator(new InventoryManipulator(inventoryItem, dragPanel, OnDrop));
                panel.Add(slot);
            }
        }
    }
}

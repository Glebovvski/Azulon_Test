using UnityEngine.UIElements;

public class WorldUIManager : UIManager
{
    protected override void FillInventory()
    {
        panel = root.Q(className: "world");
        foreach (var item in dataList.List)
        {
            for (int j = 0; j < item.amount; j++)
            {
                var slot = CreateSlot();
                var inventoryItem = CreateItem(item.data);
                slot.Add(inventoryItem);
                inventoryItem.AddManipulator(new InventoryManipulator(inventoryItem, dragPanel, null));
                panel.Add(slot);
                inventoryUIData.Add(new InventoryUIData(item, slot, false, inventoryUIData.Count));
            }
        }
    }
}

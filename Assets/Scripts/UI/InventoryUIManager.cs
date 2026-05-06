using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIManager : UIManager
{
    [SerializeField] private int inventroySize = 64;

    protected override void FillInventory()
    {
        panel = root.Q(className: "inventory");
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

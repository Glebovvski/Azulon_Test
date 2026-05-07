using UI.Controller;
using UnityEngine.UIElements;

namespace UI.Document
{
    public class WorldUIManager : UIManager
    {
        protected override void FillInventory()
        {
            InitMainPanelElements(UIDocumentConsts.WorldInventoryName);
            panel = root.Q(className: UIDocumentConsts.WorldGridClassName);
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
}
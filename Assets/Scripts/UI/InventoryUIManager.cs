using UI.Controller;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Document
{
    public class InventoryUIManager : UIManager
    {
        [SerializeField] private int inventroySize = 64;

        protected override void FillInventory()
        {
            InitMainPanelElements(UIDocumentConsts.PlayerInventoryName);
            panel = root.Q(className: UIDocumentConsts.InventoryGridClassName);
            for (int i = 0; i < inventroySize; i++)
            {
                var slot = CreateSlot();
                panel.Add(slot);
            }
            var slots = panel.Query<VisualElement>(className: UIDocumentConsts.SlotClassName).ToList();
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
}
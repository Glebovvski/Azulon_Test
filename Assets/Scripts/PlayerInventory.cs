using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InventoryListScriptableData startingInventory;
    public Dictionary<int, int> Inventory { get; private set; }

    void Awake()
    {
        Inventory = new();
        PopulateInventoryWithStartingData();
    }

    private void OnEnable()
    {
        InventoryEvents.OnItemDropped += HandleItemDropped;
    }

    private void PopulateInventoryWithStartingData()
    {
        foreach (var data in startingInventory.List)
        {
            if (Inventory.ContainsKey(data.Data.key))
            {
                AddToInventory(data);
            }
            else
            {
                Inventory.Add(data.Data.key, data.Amount);
            }
        }
    }

    private void HandleItemDropped(InventoryDropContext context)
    {
        if (context.Item == null)
            return;

        if (context.From == InventoryArea.World && context.To == InventoryArea.PlayerInventory)
        {
            AddToInventory(context.Item);
            return;
        }

        if (context.From == InventoryArea.PlayerInventory && context.To == InventoryArea.World)
        {
            RemoveFromInventory(context.Item);
            return;
        }
    }

    private void AddToInventory(IInventoryData data)
    {
        if (Inventory.ContainsKey(data.Data.key))
        {
            Inventory[data.Data.key] += data.Amount;
        }
        else
        {
            Inventory.Add(data.Data.key, data.Amount);
        }
    }

    private void RemoveFromInventory(IInventoryData data)
    {
        if (!Inventory.ContainsKey(data.Data.key))
        {
            Debug.LogError($"Inventory doesn't contain data with key {data.Data.key} {data.Data.name}");
            return;
        }

        Inventory[data.Data.key] -= data.Amount;
        if (Inventory[data.Data.key] == 0)
        {
            Inventory.Remove(data.Data.key);
        }
    }

    private void OnDisable()
    {
        InventoryEvents.OnItemDropped -= HandleItemDropped;
    }
}

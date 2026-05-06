using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private InventoryListScriptableData startingInventory;
    public Dictionary<int, int> Inventory { get; private set; }

    void Awake()
    {
        Inventory = new();
        PopulateInventoryWithStartingData();
    }

    private void PopulateInventoryWithStartingData()
    {
        foreach (var data in startingInventory.List)
        {
            if (Inventory.ContainsKey(data.Data.key))
            {
                AddToInventoty(data);
            }
            else
            {
                Inventory.Add(data.Data.key, data.Amount);
            }
        }
    }

    public void AddToInventoty(IInventoryData data)
    {
        if (Inventory.ContainsKey(data.Data.key))
        {
            Inventory[data.Data.key] += data.Amount;
        }
    }

    public void RemoveFromInventory(IInventoryData data)
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
}

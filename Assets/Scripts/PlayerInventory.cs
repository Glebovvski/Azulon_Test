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
            if (Inventory.ContainsKey(data.data.key))
            {
                AddToInventoty(data);
            }
            else
            {
                Inventory.Add(data.data.key, data.amount);
            }
        }
    }

    public void AddToInventoty(InventoryData data)
    {
        if (Inventory.ContainsKey(data.data.key))
        {
            Inventory[data.data.key] += data.amount;
        }
    }

    public void RemoveFromInventory(InventoryData data)
    {
        if (!Inventory.ContainsKey(data.data.key))
        {
            Debug.LogError($"Inventory doesn't contain data with key {data.data.key} {data.data.name}");
            return;
        }

        Inventory[data.data.key] -= data.amount;
        if (Inventory[data.data.key] == 0)
        {
            Inventory.Remove(data.data.key);
        }
    }
}

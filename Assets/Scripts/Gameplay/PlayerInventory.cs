using System.Collections.Generic;
using System.Linq;
using Data;
using ScriptableData;
using UnityEngine;

namespace GameplayData
{
    public interface IPlayerInventory
    {
        Dictionary<int, int> Inventory { get; }
        InventoryScriptableData GetItem(int key);
        InventoryScriptableData GetItem(string name);
        void HandleItemDropped(InventoryDropContext context);
    }

    public class PlayerInventory : MonoBehaviour, IPlayerInventory
    {
        [SerializeField] private InventoryListScriptableData startingInventory;
        [SerializeField] private InventoryListScriptableData ItemsDB;
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

        public InventoryScriptableData GetItem(int key)
        {
            if (Inventory.ContainsKey(key))
            {
                var item = ItemsDB.List.FirstOrDefault(x => x.Data.key == key);
                return item.Data;
            }
            return null;
        }

        public InventoryScriptableData GetItem(string name)
        {
            int key = -1;
            key = ItemsDB.List.FirstOrDefault(x => x.Data.name.Contains(name)).Data.key;
            if (key < 0)
                return null;
            return GetItem(key);
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

        public void HandleItemDropped(InventoryDropContext context)
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
}
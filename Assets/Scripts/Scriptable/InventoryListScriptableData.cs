using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu(fileName = "Inventory List", menuName = "Inventory/List")]
    public class InventoryListScriptableData : ScriptableObject
    {
        [CreateProperty]
        public List<InventoryData> List;
    }

    [Serializable]
    public class InventoryData : IInventoryData
    {
        [CreateProperty]
        [field: SerializeField] public InventoryScriptableData Data { get; set; }
        [CreateProperty]
        [field: SerializeField] public int Amount { get; set; }
    }

    public interface IInventoryData
    {
        public InventoryScriptableData Data { get; }
        public int Amount { get; }
    }
}
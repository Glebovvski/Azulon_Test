using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

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
    [field:SerializeField] public InventoryScriptableData Data { get; set; }
    [CreateProperty]
    [field:SerializeField] public int Amount { get; set; }

    // public InventoryData(InventoryScriptableData _data, int _amount)
    // {
    //     Data = _data;
    //     Amount = _amount;        
    // }
}

public interface IInventoryData
{
    public InventoryScriptableData Data { get; }
    public int Amount { get; }
}
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
public class InventoryData
{
    [CreateProperty]
    public InventoryScriptableData data;
    [CreateProperty]
    public int amount;
}

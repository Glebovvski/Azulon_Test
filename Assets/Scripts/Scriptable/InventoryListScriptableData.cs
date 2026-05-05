using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory List", menuName = "Inventory/List")]
public class InventoryListScriptableData : ScriptableObject
{
    public List<InventoryData> List;
}

[Serializable]
public class InventoryData
{
    public InventoryScriptableData data;
    public int amount;
}

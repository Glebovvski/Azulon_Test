using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Item")]
public class InventoryScriptableData : ScriptableObject
{
    [CreateProperty]
    public int key;
    [CreateProperty]
    public string name;
    [Multiline]
    [CreateProperty]
    public string description;
    [CreateProperty]
    public Sprite icon;
}

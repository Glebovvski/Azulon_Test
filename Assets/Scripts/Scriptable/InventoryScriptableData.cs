using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory/Item")]
public class InventoryScriptableData : ScriptableObject
{
    public int key;
    public string name;
    [Multiline]
    public string description;
    public Sprite icon;
}

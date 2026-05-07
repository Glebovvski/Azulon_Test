using System;
using System.IO;
using ScriptableData;
using UnityEditor;
using UnityEngine;

namespace InventoryEditor.Tools
{
    public class InventoryJsonImporter : EditorWindow
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private DefaultAsset outputFolder;
        [SerializeField] private int startKey = 0;

        [MenuItem("Tools/Inventory/Import Items From JSON")]
        public static void Open()
        {
            GetWindow<InventoryJsonImporter>("Inventory Importer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Inventory JSON Importer");

            jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);

            outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", outputFolder, typeof(DefaultAsset), false);

            startKey = 0;

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate ScriptableObjects"))
            {
                GenerateAssets();
            }
        }

        private void GenerateAssets()
        {
            if (jsonFile == null)
            {
                Debug.LogError("No JSON");
                return;
            }

            if (outputFolder == null)
            {
                Debug.LogError("No folder");
                return;
            }

            string outputPath = AssetDatabase.GetAssetPath(outputFolder);

            InventoryItemJson[] items = ParseJsonArray(jsonFile.text);

            if (items == null || items.Length == 0)
            {
                Debug.LogError("No items found in JSON");
                return;
            }

            int createdCount = 0;

            for (int i = 0; i < items.Length; i++)
            {
                InventoryItemJson item = items[i];

                if (string.IsNullOrWhiteSpace(item.name))
                {
                    Debug.LogError($"Skipped item at index {i}: missing name.");
                    continue;
                }

                InventoryScriptableData asset = CreateInstance<InventoryScriptableData>();

                asset.key = item.key != 0 ? item.key : startKey + i;
                asset.name = item.name;
                asset.description = item.description;
                asset.icon = FindSpriteForItem(item);

                string safeFileName = MakeSafeFileName(string.IsNullOrWhiteSpace(item.id) ? item.name : item.id);

                string assetPath = $"{outputPath}/{safeFileName}.asset";

                if (File.Exists(assetPath))
                {
                    InventoryScriptableData existingAsset = AssetDatabase.LoadAssetAtPath<InventoryScriptableData>(assetPath);

                    if (existingAsset != null)
                    {
                        existingAsset.key = asset.key;
                        existingAsset.name = asset.name;
                        existingAsset.description = asset.description;
                        existingAsset.icon = asset.icon;

                        EditorUtility.SetDirty(existingAsset);
                        createdCount++;
                        continue;
                    }
                }
                ((UnityEngine.Object)asset).name = item.name;
                AssetDatabase.CreateAsset(asset, assetPath);
                createdCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static InventoryItemJson[] ParseJsonArray(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<InventoryItemJson>();

            string wrappedJson = "{\"items\":" + json + "}";
            InventoryItemJsonWrapper wrapper = JsonUtility.FromJson<InventoryItemJsonWrapper>(wrappedJson);

            return wrapper.items;
        }

        private static Sprite FindSpriteForItem(InventoryItemJson item)
        {
            if (!string.IsNullOrWhiteSpace(item.iconPath))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(item.iconPath);

                if (sprite != null)
                    return sprite;

                Debug.LogWarning($"Sprite not found at iconPath: {item.iconPath}");
            }

            string searchName = !string.IsNullOrWhiteSpace(item.id) ? item.id : item.name;

            string[] guids = AssetDatabase.FindAssets($"{searchName} t:Sprite");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            return null;
        }

        private static string MakeSafeFileName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }

            return value
                .Trim()
                .Replace(" ", "_")
                .ToLowerInvariant();
        }

        [Serializable]
        private class InventoryItemJsonWrapper
        {
            public InventoryItemJson[] items;
        }

        [Serializable]
        private class InventoryItemJson
        {
            public int key;
            public string id;
            public string name;
            public string description;
            public string iconPath;
        }
    }
}
using System.Collections.Generic;
using ScriptableData;
using UnityEditor;
using UnityEngine;

namespace InventoryEditor.Tools
{
    public class InventoryFiller : EditorWindow
    {
        [SerializeField] private InventoryListScriptableData listToFill;
        [SerializeField] private DefaultAsset dataFolder;

        [SerializeField] private bool fillAll = true;

        [SerializeField] private int fillMinAmount = 1;
        [SerializeField] private int fillMaxAmount = 8;

        [SerializeField] private int randomMinItems = 3;
        [SerializeField] private int randomMaxItems = 12;

        [MenuItem("Tools/Inventory/Fill inventory")]
        public static void Open()
        {
            GetWindow<InventoryFiller>("Inventory Filler");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Inventory Filler");

            listToFill = (InventoryListScriptableData)EditorGUILayout.ObjectField("List", listToFill, typeof(InventoryListScriptableData), false);

            dataFolder = (DefaultAsset)EditorGUILayout.ObjectField("Data Folder", dataFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space();

            fillAll = EditorGUILayout.Toggle("Fill all data", fillAll);

            EditorGUILayout.Space();

            fillMinAmount = EditorGUILayout.IntField("Min item amount", fillMinAmount);
            fillMaxAmount = EditorGUILayout.IntField("Max item amount", fillMaxAmount);

            if (!fillAll)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Random Item Count", EditorStyles.boldLabel);

                randomMinItems = EditorGUILayout.IntField("Min items", randomMinItems);
                randomMaxItems = EditorGUILayout.IntField("Max items", randomMaxItems);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Fill Inventory List"))
            {
                FillInventoryList();
            }

            if (GUILayout.Button("Clear List"))
            {
                ClearList();
            }
        }

        private void FillInventoryList()
        {
            if (listToFill == null)
            {
                Debug.LogError("List is null");
                return;
            }

            if (dataFolder == null)
            {
                Debug.LogError("Data folder is null");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(dataFolder);

            NormalizeValues();

            List<InventoryScriptableData> foundItems = LoadInventoryAssets(folderPath);

            if (listToFill.List == null)
                listToFill.List = new List<InventoryData>();

            listToFill.List.Clear();

            if (fillAll)
            {
                FillWithAllItems(foundItems);
            }
            else
            {
                FillWithRandomItems(foundItems);
            }

            EditorUtility.SetDirty(listToFill);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void FillWithAllItems(List<InventoryScriptableData> foundItems)
        {
            foreach (InventoryScriptableData item in foundItems)
            {
                listToFill.List.AddRange(CreateInventoryDataList(item));
            }
        }

        private void FillWithRandomItems(List<InventoryScriptableData> foundItems)
        {
            Shuffle(foundItems);

            int maxPossibleItems = Mathf.Min(randomMaxItems, foundItems.Count);
            int minPossibleItems = Mathf.Min(randomMinItems, maxPossibleItems);

            int itemCount = Random.Range(minPossibleItems, maxPossibleItems + 1);

            for (int i = 0; i < itemCount; i++)
            {
                listToFill.List.Add(CreateInventoryData(foundItems[i]));
            }
        }

        private List<InventoryScriptableData> LoadInventoryAssets(string folderPath)
        {
            List<InventoryScriptableData> items = new List<InventoryScriptableData>();

            string[] guids = AssetDatabase.FindAssets("t:InventoryScriptableData", new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                InventoryScriptableData item = AssetDatabase.LoadAssetAtPath<InventoryScriptableData>(assetPath);

                if (item != null)
                    items.Add(item);
            }

            items.Sort((a, b) =>
            {
                int keyCompare = a.key.CompareTo(b.key);

                if (keyCompare != 0)
                    return keyCompare;

                return string.Compare(a.name, b.name, System.StringComparison.Ordinal);
            });

            return items;
        }

        private InventoryData CreateInventoryData(InventoryScriptableData item)
        {
            return new InventoryData()
            {
                Data = item,
                Amount = Random.Range(fillMinAmount, fillMaxAmount + 1)
            };
        }

        private List<InventoryData> CreateInventoryDataList(InventoryScriptableData item)
        {
            var list = new List<InventoryData>();
            int amount = Random.Range(fillMinAmount, fillMaxAmount + 1);
            for (int i = 0; i < amount; i++)
            {
                list.Add(new InventoryData()
                {
                    Data = item,
                    Amount = 1
                });
            }
            return list;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);

                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        private void NormalizeValues()
        {
            fillMinAmount = Mathf.Max(0, fillMinAmount);
            fillMaxAmount = Mathf.Max(fillMinAmount, fillMaxAmount);

            randomMinItems = Mathf.Max(0, randomMinItems);
            randomMaxItems = Mathf.Max(randomMinItems, randomMaxItems);
        }

        private void ClearList()
        {
            if (listToFill == null)
                return;

            if (listToFill.List == null)
                listToFill.List = new List<InventoryData>();
            listToFill.List.Clear();

            EditorUtility.SetDirty(listToFill);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
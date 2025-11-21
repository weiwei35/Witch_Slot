using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// CSV → SymbolSO 自动导入器
/// </summary>
public class SymbolImporter : EditorWindow
{
    private TextAsset csvFile;
    private string outputPath = "Assets/GameData/Symbol/";

    [MenuItem("Tools/Symbol Importer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SymbolImporter), false, "Symbol Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Symbol CSV Importer", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        outputPath = EditorGUILayout.TextField("Output Folder", outputPath);

        if (GUILayout.Button("Import"))
        {
            if (csvFile == null)
            {
                Debug.LogError("No CSV selected!");
                return;
            }

            ImportCSV(csvFile.text);
        }
    }

    void ImportCSV(string csv)
    {
        string[] lines = csv.Split('\n');

        if (lines.Length <= 1)
        {
            Debug.LogError("CSV empty!");
            return;
        }

        // 按 symbolId 分组
        Dictionary<string, List<SymbolCSVRow>> symbolGroups = new();

        // parse
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            SymbolCSVRow row = new SymbolCSVRow(line);

            if (!symbolGroups.ContainsKey(row.symbolId))
                symbolGroups[row.symbolId] = new List<SymbolCSVRow>();

            symbolGroups[row.symbolId].Add(row);
        }

        foreach (var kvp in symbolGroups)
            CreateOrUpdateSymbolSO(kvp.Value);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Symbol Imported! Total = {symbolGroups.Count}");
    }

    /// <summary>
    /// 行 → SymbolSO
    /// </summary>
    void CreateOrUpdateSymbolSO(List<SymbolCSVRow> rows)
    {
        // 一个 symbolId 对应多个 effect
        SymbolCSVRow row = rows[0];
        string id = row.symbolId;

        string path = $"{outputPath}{row.displayName}.asset";

        SymbolSO so = AssetDatabase.LoadAssetAtPath<SymbolSO>(path);

        if (so == null)
        {
            so = ScriptableObject.CreateInstance<SymbolSO>();
            AssetDatabase.CreateAsset(so, path);
        }

        so.symbolId = row.symbolId;
        so.displayName = row.displayName;

        // ✅ icon
        if (!string.IsNullOrEmpty(row.iconName))
        {
            so.icon = LoadIcon(row.iconName);

            if (!so.icon)
            {
                Debug.LogWarning($"[SymbolImporter] Icon not found: {row.iconName}");
            }
        }
        else
        {
            so.icon = null;
        }

        // ✅ 直接读取 CSV 内容
        so.category = row.category;
        so.triggers = row.triggers;
        so.interval = row.interval;
        // so.durationBattles = 0;
        // so.durationAttacks = 0;
        so.isConsumedAfterTrigger = row.isConsumedAfterTrigger;
        so.description = row.description;
        so.needTrigger = row.needTrigger;

        // ✅ EFFECTS 读取（不再写 durationBattles 等重复字段）
        so.effects = new();
        foreach (var r in rows)
        {
            SymbolEffectConfig cfg = new()
            {
                effectType = r.effectType,
                value = r.value,
                durationBattles = r.durationBattles,
                durationAttacks = r.durationAttacks,
                element = r.element,
                target = r.target,
            };
            so.effects.Add(cfg);
        }

        EditorUtility.SetDirty(so);
    }


    // ✅ 加载 icon（支持 sprite atlas / sliced sprites）
    Sprite LoadIcon(string iconName)
    {
        string folder = "Assets/Art/Symbol/";

        var textureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });

        foreach (string guid in textureGUIDs)
        {
            string texPath = AssetDatabase.GUIDToAssetPath(guid);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(texPath);

            foreach (var a in assets)
            {
                if (a is Sprite sp && sp.name == iconName)
                    return sp;
            }
        }

        Debug.LogWarning($"[SymbolImporter] Icon not found: {iconName}");
        return null;
    }
}

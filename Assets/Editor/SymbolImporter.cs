using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SymbolImporter : EditorWindow
{
    private TextAsset csvFile;
    private string savePath = "Assets/GameData/Symbols/";
    private string spriteSearchFolder = "Assets/Art/Symbol/";

    [MenuItem("Tools/Symbol Importer (Advanced)")]
    public static void ShowWindow()
    {
        GetWindow<SymbolImporter>("Symbol Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("🧩 Symbol 导入工具（覆盖更新 + 图集支持 + Booster 扩展）", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV 文件", csvFile, typeof(TextAsset), false);
        savePath = EditorGUILayout.TextField("SO 保存路径", savePath);
        spriteSearchFolder = EditorGUILayout.TextField("Sprite 搜索文件夹", spriteSearchFolder);

        if (GUILayout.Button("导入 / 更新", GUILayout.Height(30)))
        {
            if (csvFile != null)
                ImportSymbols(csvFile.text, savePath);
            else
                Debug.LogError("❌ 请先选择一个 CSV 文件");
        }
    }

    private void ImportSymbols(string csvText, string folderPath)
    {
        string[] lines = csvText.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogError("❌ CSV 内容为空或格式错误");
            return;
        }

        // 读取已有 Symbol
        string[] existingAssets = Directory.Exists(folderPath)
            ? Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories)
            : new string[0];
        var existingSymbols = new Dictionary<string, BaseSymbolSO>();

        foreach (string assetPath in existingAssets)
        {
            var so = AssetDatabase.LoadAssetAtPath<BaseSymbolSO>(assetPath);
            if (so != null)
            {
                if (existingSymbols.ContainsKey(so.symbolName))
                    Debug.LogWarning($"⚠️ 重名符号：{so.symbolName}（路径：{assetPath}）");
                else
                    existingSymbols.Add(so.symbolName, so);
            }
        }

        int created = 0, updated = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');

            // CSV 基础字段
            string type = cols[0].Trim();
            string name = cols[1].Trim();
            string desc = cols[2].Trim();
            string spriteName = cols[3].Trim();

            // ✅ Sprite 匹配（支持图集）
            Sprite sprite = FindSpriteByNameInFolder(spriteName, spriteSearchFolder);

            existingSymbols.TryGetValue(name, out BaseSymbolSO existingSO);

            if (type.Equals("Normal", System.StringComparison.OrdinalIgnoreCase))
            {
                NormalSymbolSO symbol = existingSO as NormalSymbolSO;
                bool isNew = symbol == null;
                if (isNew)
                {
                    symbol = ScriptableObject.CreateInstance<NormalSymbolSO>();
                    created++;
                }
                else
                {
                    updated++;
                }

                symbol.symbolName = name;
                symbol.symbolDesc = desc;
                symbol.symbolSprite = sprite;

                if (float.TryParse(cols[4], out float amt))
                    symbol.amount = amt;

                SaveOrUpdateSO(symbol, folderPath, name, isNew);
            }
            else if (type.Equals("Booster", System.StringComparison.OrdinalIgnoreCase))
            {
                BoosterSymbolSO booster = existingSO as BoosterSymbolSO;
                bool isNew = booster == null;
                if (isNew)
                {
                    booster = ScriptableObject.CreateInstance<BoosterSymbolSO>();
                    created++;
                }
                else
                {
                    updated++;
                }

                booster.symbolName = name;
                booster.symbolDesc = desc;
                booster.symbolSprite = sprite;

                // 🔹 Booster 属性解析
                if (cols.Length > 5 && System.Enum.TryParse(cols[5].Trim(), out BoosterTriggerTiming timing))
                    booster.triggerTiming = timing;

                if (cols.Length > 6 && System.Enum.TryParse(cols[6].Trim(), out BoosterTargetType target))
                    booster.targetType = target;

                if (cols.Length > 7 && System.Enum.TryParse(cols[7].Trim(), out BoosterEffectType effect))
                    booster.effectType = effect;

                if (cols.Length > 8 && float.TryParse(cols[8], out float val))
                    booster.effectValue = val;

                if (cols.Length > 9 && int.TryParse(cols[9], out int dur))
                    booster.duration = dur;

                // 🟢 新增属性：DurationType, IntervalCount
                if (cols.Length > 10 && System.Enum.TryParse(cols[10].Trim(), out BoosterDurationType durType))
                    booster.durationType = durType;
                else
                    booster.durationType = BoosterDurationType.Immediate; // 默认即时

                if (cols.Length > 11 && int.TryParse(cols[11].Trim(), out int interval))
                    booster.intervalCount = interval;
                else
                    booster.intervalCount = 0;

                SaveOrUpdateSO(booster, folderPath, name, isNew);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Symbol 导入完成：新建 {created} 个，更新 {updated} 个。");
    }

    // ✅ 支持图集 Sprite 匹配
    private Sprite FindSpriteByNameInFolder(string spriteName, string searchFolder)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;

        // 支持 item.png:item_2 格式
        if (spriteName.Contains(":"))
        {
            string[] parts = spriteName.Split(':');
            string sheetBaseName = parts[0];
            string subSpriteName = parts[1];

            string[] sheetGuids = AssetDatabase.FindAssets($"{sheetBaseName} t:Texture2D", new[] { searchFolder });
            if (sheetGuids.Length > 0)
            {
                string sheetPath = AssetDatabase.GUIDToAssetPath(sheetGuids[0]);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(sheetPath).OfType<Sprite>();
                var subSprite = sprites.FirstOrDefault(s => s.name == subSpriteName);
                if (subSprite != null)
                {
                    return subSprite;
                }
            }

            Debug.LogWarning($"⚠️ 未找到图集 {sheetBaseName}:{subSpriteName}");
            return null;
        }

        // 常规 Sprite 搜索
        string[] guids = AssetDatabase.FindAssets($"{spriteName} t:Sprite", new[] { searchFolder });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"⚠️ 未在 {searchFolder} 找到 Sprite：{spriteName}");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var allSprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToList();

        // 如果是图集，则匹配同名子 Sprite
        if (allSprites.Count > 1)
        {
            var sub = allSprites.FirstOrDefault(s => s.name == spriteName);
            if (sub != null)
                return sub;
        }

        return allSprites.FirstOrDefault();
    }

    private void SaveOrUpdateSO(ScriptableObject so, string folder, string name, bool isNew)
    {
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string assetPath = Path.Combine(folder, $"{name}.asset");
        if (isNew)
        {
            AssetDatabase.CreateAsset(so, assetPath);
        }
        else
        {
            EditorUtility.SetDirty(so);
        }
    }
}

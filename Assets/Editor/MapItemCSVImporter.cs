using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MapItemCSVImporter : EditorWindow
{
    private static string csvPath = "Assets/Configs";
    private static string outputPath = "Assets/GameData/MapItems/";

    [MenuItem("Tools/Import ALL CSV")]
    public static void ImportAll()
    {
        ImportEnemies();
        ImportItems();
        ImportChests();
        ImportRoomContent();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✨ CSV → SO 导入完成");
    }

    static string[][] ReadCSV(string file)
    {
        string fullPath = Path.Combine(csvPath, file);
        string[] lines = File.ReadAllLines(fullPath);

        List<string[]> rows = new();
        foreach (var line in lines)
            rows.Add(line.Split(','));

        return rows.ToArray();
    }

    // ---------------- Enemy ----------------
    public static void ImportEnemies()
    {
        var rows = ReadCSV("Enemy.csv");
        for (int i = 1; i < rows.Length; i++)
        {
            var r = rows[i];

            CharacterDataSO so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.ID = r[0];
            so.charactorName = r[1];
            so.maxHP = int.Parse(r[2]);
            so.strength = int.Parse(r[3]);
            so.defense = int.Parse(r[4]);
            so.icon = LoadIcon(r[5]);

            AssetDatabase.CreateAsset(so, outputPath + "Enemy/" + so.ID + ".asset");
        }
        Debug.Log("Imported Enemy SO");
    }

    // ---------------- Item ----------------
    public static void ImportItems()
    {
        var rows = ReadCSV("Item.csv");
        for (int i = 1; i < rows.Length; i++)
        {
            var r = rows[i];

            MapItemSO so = ScriptableObject.CreateInstance<MapItemSO>();
            so.ID = r[0];
            so.Name = r[1];
            so.Type = EnumParser.ParseEnum<EffectType>(r[2]);
            so.Value = int.Parse(r[3]);
            so.Icon = LoadIcon(r[4]);

            AssetDatabase.CreateAsset(so, outputPath + "Item/" + so.ID + ".asset");
        }
        Debug.Log("Imported Item SO");
    }

    // ---------------- Chest ----------------
    public static void ImportChests()
    {
        var rows = ReadCSV("Chest.csv");
        for (int i = 1; i < rows.Length; i++)
        {
            var r = rows[i];

            MapChestSO so = ScriptableObject.CreateInstance<MapChestSO>();
            so.ID = r[0];
            so.Name = r[1];
            so.Icon = LoadIcon(r[2]);

            AssetDatabase.CreateAsset(so, outputPath + "Chest/" + so.ID + ".asset");
        }
        Debug.Log("Imported Chest SO");
    }

    // ---------------- RoomContent ----------------
    public static void ImportRoomContent()
    {
        var enemyDict = LoadDict<CharacterDataSO>("Enemy");
        var itemDict = LoadDict<MapItemSO>("Item");
        var chestDict = LoadDict<MapChestSO>("Chest");

        var rows = ReadCSV("RoomContent.csv");

        Dictionary<string, RoomContentData> roomMap = new();

        for (int i = 1; i < rows.Length; i++)
        {
            var r = rows[i];
            string level = r[0];
            string roomName = r[1];
            string type = r[2];
            string refID = r[3];
            float posIndex = float.Parse(r[4]);

            string key = level + "_" + roomName;

            if (!roomMap.TryGetValue(key, out RoomContentData so))
            {
                so = ScriptableObject.CreateInstance<RoomContentData>();
                so.Level = level;
                so.RoomName = roomName;
                roomMap[key] = so;
            }

            switch (type)
            {
                case "Enemy":
                    so.Enemies.Add(new EnemySpawn
                    {
                        Data = enemyDict[refID],
                        PosIndex = posIndex
                    });
                    break;

                case "Item":
                    so.Items.Add(new ItemSpawn
                    {
                        Data = itemDict[refID],
                        PosIndex = posIndex
                    });
                    break;

                case "Chest":
                    so.Chests.Add(new ChestSpawn
                    {
                        Data = chestDict[refID],
                        PosIndex = posIndex
                    });
                    break;
            }
        }

        // 保存所有 RoomContent SO
        foreach (var kv in roomMap)
        {
            AssetDatabase.CreateAsset(kv.Value, outputPath + "RoomContent/" + kv.Key + ".asset");
        }

        Debug.Log("Imported RoomContent SO");
    }

    static Dictionary<string, T> LoadDict<T>(string subfolder) where T : ScriptableObject
    {
        string folder = outputPath + subfolder;
        var assets = AssetDatabase.LoadAllAssetsAtPath(folder);

        Dictionary<string, T> dict = new();
        foreach (var path in Directory.GetFiles(folder))
        {
            if (path.EndsWith(".asset"))
            {
                T so = AssetDatabase.LoadAssetAtPath<T>(path);
                dict[(string)so.GetType().GetField("ID").GetValue(so)] = so;
            }
        }
        return dict;
    }

    static Sprite LoadIcon(string iconName)
    {
        string folder = "Assets/Art/MapIcons/";

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

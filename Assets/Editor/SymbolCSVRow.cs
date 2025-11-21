using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SymbolCSVRow
{
    public string symbolId;
    public string displayName;
    public string iconName;
    public SymbolCategory category;
    public List<TriggerEvent> triggers;
    public int interval;
    public EffectType effectType;
    public float value;
    public DamageElement element;
    public TargetType target;
    public int durationBattles;
    public int durationAttacks;
    public bool isConsumedAfterTrigger;
    public bool needTrigger;
    public string description;

    public SymbolCSVRow(string csvLine)
    {
        var cols = Split(csvLine);

        int i = 0;
        symbolId = cols[i++].Trim();
        displayName = cols[i++].Trim();
        iconName = cols[i++].Trim();
        category = EnumParser.ParseEnum<SymbolCategory>(cols[i++]);

        triggers = EnumParser.ParseEnumList<TriggerEvent>(cols[i++]).ToList();

        interval = SafeInt(cols[i++]);
        effectType = EnumParser.ParseEnum<EffectType>(cols[i++]);

        value = SafeFloat(cols[i++]);
        element = EnumParser.ParseEnum<DamageElement>(cols[i++]);
        target = EnumParser.ParseEnum<TargetType>(cols[i++]);

        durationBattles = SafeInt(cols[i++]);
        durationAttacks = SafeInt(cols[i++]);

        isConsumedAfterTrigger = SafeBool(cols[i++]);
        needTrigger = SafeBool(cols[i++]);
        description = cols[i++].Trim();
    }

    string[] Split(string line) => line.Split(',');

    int SafeInt(string s) => int.TryParse(s, out var v) ? v : 0;
    float SafeFloat(string s) => float.TryParse(s, out var v) ? v : 0;
    bool SafeBool(string s) => s.Trim().ToUpper() == "TRUE";
}
public static class EnumParser
{
    public static T ParseEnum<T>(string s) where T : struct
    {
        Enum.TryParse<T>(s, true, out var result);
        return result;
    }

    public static IEnumerable<T> ParseEnumList<T>(string s) where T : struct
    {
        if (string.IsNullOrEmpty(s)) yield break;

        var arr = s.Split(';');
        foreach (var a in arr)
        {
            if (Enum.TryParse<T>(a.Trim(), true, out var e))
                yield return e;
        }
    }
}

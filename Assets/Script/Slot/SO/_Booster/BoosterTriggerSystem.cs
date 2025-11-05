using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoosterTriggerSystem
{
    /// <summary>
    /// 根据触发时机执行效果。
    /// </summary>
    public static void TriggerBoosters(
        BoosterTriggerTiming timing,
        List<BoosterSymbolSO> boosters,
        CharacterDataSO playerData,
        List<Enemy> allEnemies)
    {
        if (boosters == null || boosters.Count == 0) return;

        foreach (var booster in boosters)
        {
            if (booster.triggerTiming != timing)
                continue;

            List<CharacterDataSO> targets = new();

            switch (booster.targetType)
            {
                case BoosterTargetType.Player:
                    targets.Add(playerData);
                    break;
                case BoosterTargetType.CurrentEnemy:
                    if (GameManager.Instance.CurrentEnemy != null)
                        targets.Add(GameManager.Instance.CurrentEnemy.GetData());
                    break;
                case BoosterTargetType.AllEnemies:
                    foreach (var e in allEnemies)
                        targets.Add(e.GetData());
                    break;
                case BoosterTargetType.HighestHPEnemy:
                    var hpEnemy = allEnemies.OrderByDescending(e => e.GetData().currentHP).FirstOrDefault();
                    if (hpEnemy) targets.Add(hpEnemy.GetData());
                    break;
                case BoosterTargetType.HighestAttackEnemy:
                    var atkEnemy = allEnemies.OrderByDescending(e => e.GetData().strength).FirstOrDefault();
                    if (atkEnemy) targets.Add(atkEnemy.GetData());
                    break;
            }

            foreach (var t in targets)
            {
                // ✅ 只执行即时型
                if (booster.durationType == BoosterDurationType.Immediate)
                {
                    var effect = new BoosterEffect(booster);
                    effect.Apply(t);
                    Debug.Log($"Booster 立即触发: {booster.symbolName} ({booster.effectType}) 对 {t.characterName}");
                }
            }
        }
    }
}

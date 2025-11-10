public static class CombatSync
{
    public static void Apply(Player player, Enemy enemy, CombatResult result)
    {
        if (result.PlayerWin)
        {
            // ✅ 同步玩家 HP
            player.runtimeData.CurrentHP = result.Runtime.Player.CurrentHP;

            // ✅ 不同步 Defense → 自动恢复

            // ✅ 敌人死亡
            enemy.runtimeData.CurrentHP = 0;
        }
        else
        {
            // 玩家死亡
            player.runtimeData.CurrentHP = 0;
        }
    }
}
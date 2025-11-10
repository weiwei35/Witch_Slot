using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    public GameObject battlePanel;
    public Button startFightButton;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 启动战斗
    /// </summary>
    public void StartCombat(Player player, Enemy enemy)
    {
        battlePanel.SetActive(true);
        var runtime = new CombatRuntime(player.runtimeData, enemy.runtimeData);
        var core = new CombatCore(runtime);
        
        playerHUD.Bind(runtime.Player);
        enemyHUD.Bind(runtime.Enemy);

        startFightButton.onClick.RemoveAllListeners();
        startFightButton.onClick.AddListener(() =>
        {
            // 点击后开始战斗
            startFightButton.gameObject.SetActive(false);
            StartCoroutine(BattleRoutine(player, enemy, core));
        });
        SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnCombatStart);

    }

    private IEnumerator BattleRoutine(Player player, Enemy enemy, CombatCore core)
    {
        float interval = 2f;

        AutoScrollLog.instance.AddLog("战斗开始!");

        while (!core.IsOver)
        {
            yield return new WaitForSeconds(interval);
            int count = core.DoStep();
            
            yield return new WaitForSeconds(count*0.5f);
            // AutoScrollLog.instance.AddLog(log);
        }

        // 战斗结束
        CombatResult result = new CombatResult(
            core.data.Player.CurrentHP > 0,
            core.data
        );

        CombatSync.Apply(player, enemy, result);

        yield return new WaitForSeconds(interval);
        AutoScrollLog.instance.AddLog(result.PlayerWin ? "战斗成功!" : "战斗失败!");

        player.OnBattleEnd();

        // 如果玩家胜利，清除 enemy
        if (result.PlayerWin)
            Destroy(enemy.gameObject);

        EndCombat();
    }
    
    private void EndCombat()
    {
        // ✅ 回收 Buff
        SymbolSystem.Instance.OnBattleEnd();
        battlePanel.SetActive(false);
        
        SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnVictory);
        GameManager.Instance.EndBattle();
    }
}
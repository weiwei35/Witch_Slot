using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private Scrollbar hpBar;
    [SerializeField] private Image icon;

    private CharacterStats currentStats;

    /// <summary>
    /// 由 CharacterFight 初始化时直接调用
    /// </summary>
    public void BindStats(CharacterStats newStats)
    {
        // 解绑旧事件
        if (currentStats != null)
            currentStats.OnValueChanged -= RefreshUI;

        // 绑定新事件
        currentStats = newStats;

        if (currentStats != null)
        {
            currentStats.OnValueChanged += RefreshUI;
            RefreshUI();
        }
        else
        {
            // 清空 UI
            strengthText.text = "-";
            defenseText.text = "-";
            hpBar.size = 0;
            icon.sprite = null;
        }
    }

    private void OnDisable()
    {
        // 防止泄漏
        if (currentStats != null)
            currentStats.OnValueChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (currentStats == null) return;

        strengthText.text = currentStats.Strength.ToString();
        defenseText.text = currentStats.Defense.ToString();
        hpBar.size = currentStats.CurrentHP / currentStats.MaxHP;
        icon.sprite = currentStats.Icon;
    }
}
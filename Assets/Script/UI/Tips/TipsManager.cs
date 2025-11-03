using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 全局唯一的提示系统管理器
/// </summary>
public class TipsManager : MonoBehaviour
{
    public static TipsManager Instance;

    [SerializeField] private GameObject tipsWindow;
    [SerializeField] private TextMeshProUGUI tipContent;

    [SerializeField] private Vector2 offset = new Vector2(20, 20); // 提示偏移量
    [SerializeField] private float padding = 15f; // 边距防止贴边

    private bool isVisible = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 显示 Tip
    /// </summary>
    public void ShowTip(UIDescription info, Vector2 position)
    {
        if(info.info.Name == "") return;
        string content = $"<b>{info.info.Name}</b>\n{info.info.Description}";
        tipContent.text = content;

        tipsWindow.SetActive(true);
        isVisible = true;

        // 自动调整位置到屏幕内
        position = ClampToScreenBorder(position, tipsWindow.GetComponent<RectTransform>());
        tipsWindow.transform.position = position;
    }

    /// <summary>
    /// 隐藏 Tip
    /// </summary>
    public void HideTip()
    {
        tipsWindow.SetActive(false);
        isVisible = false;
    }

    /// <summary>
    /// 防止 Tip 显示在屏幕外
    /// </summary>
    private Vector2 ClampToScreenBorder(Vector2 pos, RectTransform rect)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;

        float halfWidth = rect.rect.width / 2;
        float halfHeight = rect.rect.height / 2;

        float x = Mathf.Clamp(pos.x + offset.x, halfWidth + padding, canvasRect.width - halfWidth - padding);
        float y = Mathf.Clamp(pos.y + offset.y, halfHeight + padding, canvasRect.height - halfHeight - padding);

        return new Vector2(x, y);
    }
}

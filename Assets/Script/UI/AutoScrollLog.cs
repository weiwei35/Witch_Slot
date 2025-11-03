using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScrollLog : MonoBehaviour
{
    public static AutoScrollLog instance;
    [Tooltip("用于显示战斗日志的文本组件")]
    public TMP_Text logText; // 或使用 TMP_Text

    [Tooltip("是否仅在内容超出时自动滚动")]
    public bool onlyScrollIfNeeded = true;

    private ScrollRect scrollRect;

    private void Awake()
    {
        instance = this;
        scrollRect = GetComponent<ScrollRect>();
    }

    /// <summary>
    /// 添加日志内容后更新 UI 并尝试自动滚动
    /// </summary>
    /// <param name="newLog">新的战斗日志</param>
    public void AddLog(string newLog)
    {
        logText.text += newLog + "\n";
        StartCoroutine(ScrollToBottom());
    }

    public void OnEnable()
    {
        logText.text = "";
    }

    private System.Collections.IEnumerator ScrollToBottom()
    {
        // 如果使用 TMP_Text，这一步非常关键，因为文本更新后不会立即刷新布局
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        // 延迟一帧以确保 Content 已重新布局
        yield return null;

        if (onlyScrollIfNeeded && !IsContentOverflowing())
            yield break;

        // 滚动到底部
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private bool IsContentOverflowing()
    {
        // 获取 Content 的 RectTransform
        RectTransform contentRect = scrollRect.content;
        if (contentRect == null) return false;

        // 获取当前 Content 高度
        float contentHeight = contentRect.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        return contentHeight > viewportHeight;
    }
}
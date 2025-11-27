using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : MonoBehaviour
{
    public static TipsManager Instance;

    [SerializeField] private GameObject tipsWindow;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private TextMeshProUGUI tipTextPrefab;

    [SerializeField] private Vector2 offset = new Vector2(20, 20);
    [SerializeField] private float padding = 15f;

    private readonly List<TextMeshProUGUI> spawnedTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 显示多个 Tip
    /// </summary>
    public void ShowTip(UIDescription desc, Vector2 position)
    {
        ClearOldTexts();

        // 动态根据 desc.tips 生成 Text 行
        foreach (var tip in desc.tips)
        {
            var text = Instantiate(tipTextPrefab, contentRoot);
            text.text = $"<b>{tip.Title}</b>\n{tip.Content}";
            spawnedTexts.Add(text);
        }

        if (spawnedTexts.Count == 0)
            return;

        tipsWindow.SetActive(true);

        // UI 自动排版
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        var finalPos = ClampToScreenBorder(position, tipsWindow.GetComponent<RectTransform>());
        tipsWindow.transform.position = finalPos;
    }

    public void HideTip()
    {
        tipsWindow.SetActive(false);
        ClearOldTexts();
    }

    private void ClearOldTexts()
    {
        foreach (var t in spawnedTexts)
            if (t != null) Destroy(t.gameObject);
        spawnedTexts.Clear();
    }

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

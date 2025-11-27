using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TipItem
{
    public string Title;
    public string Content;

    public TipItem(string title, string content)
    {
        Title = title;
        Content = content;
    }
}

/// <summary>
/// 挂载到任意 UI 元素上，提供 Tips 显示所需数据
/// </summary>
public class UIDescription : MonoBehaviour
{
    public List<TipItem> tips = new List<TipItem>();

    /// <summary>
    /// 动态添加一条 Tip
    /// </summary>
    public void AddTip(string title, string content)
    {
        tips.Add(new TipItem(title, content));
    }

    /// <summary>
    /// 移除
    /// </summary>
    public void RemoveTip(string title)
    {
        tips.RemoveAll(t => t.Title == title);
    }

    /// <summary>
    /// 清空
    /// </summary>
    public void ClearTips()
    {
        tips.Clear();
    }
}
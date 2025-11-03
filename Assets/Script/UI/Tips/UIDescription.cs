using System;
using UnityEngine;

[Serializable]
public class NameDescription
{
    public string Name = "名称";
    public string Description = "描述信息";
}

/// <summary>
/// 挂载到任意 UI 元素上，提供 Tips 显示所需数据
/// </summary>
[AddComponentMenu("UI/Tips/NameDescription")]
public class UIDescription : MonoBehaviour
{
    public NameDescription info = new NameDescription();
}
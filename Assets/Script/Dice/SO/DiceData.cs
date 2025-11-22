using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "DiceData", menuName = "GameData/Dice")]
public class DiceData : ScriptableObject
{
    public List<DiceFaceData> faces = new List<DiceFaceData>();
    public SymbolSO symbol;
    public TMP_FontAsset font;
    public Texture texture;
}
[Serializable]
public class DiceFaceData {
    public FaceDir dir;      // 方便调试的名字 (如 "Top", "Front")
    public int value;        // 这个面对应的数字
}
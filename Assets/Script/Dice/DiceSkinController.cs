using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 骰子皮肤控制器
/// 职责：管理分离后的 7 个材质槽，实现整体纹理和独立面图案的控制。
/// </summary>
public class DiceSkinController : MonoBehaviour
{
    public DiceData data;
    public DiceFace[] diceFaces;

    private void Start()
    {
        InitMesh();
        GetComponent<DiceController>().InitData(diceFaces);
    }

    public void InitMesh()
    {
        foreach (var face in diceFaces)
        {
            foreach (var faceData in data.faces)
            {
                if (faceData.dir == face.dir)
                {
                    face.value = faceData.value;
                    face.mesh.material.mainTexture = data.texture;
                    face.numberText.font = data.font;
                }
            }
            face.numberText.text = face.value.ToString();
        }
    }
    public void SetMatOneSide(FaceDir face,Texture tex)
    {
        foreach (var diceFace in diceFaces)
        {
            if (diceFace.dir == face)
            {
                diceFace.mesh.material.mainTexture = tex;
            }
        }
    }

    public void SetFontOneSide(FaceDir face,TMP_FontAsset font)
    {
        foreach (var diceFace in diceFaces)
        {
            if (diceFace.dir == face)
            {
                diceFace.numberText.font = font;
            }
        }
    }

    public void SetFaceNumberOneSide(FaceDir face, int number)
    {
        foreach (var diceFace in diceFaces)
        {
            if (diceFace.dir == face)
            {
                diceFace.value = number;
            }
        }
    }
}
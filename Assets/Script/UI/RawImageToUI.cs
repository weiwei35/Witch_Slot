using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 获取 RawImage 中 3D 物体在 Main Canvas 中的 UI 坐标位置。
/// 采用“两步叠加法”，适用于 RawImage 在 Canvas 中有偏移的情况。
/// </summary>
public class RawImageToUI : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("渲染 3D 场景并输出到 RawImage 的摄像机")]
    public Camera renderCamera;

    [Tooltip("显示摄像机输出的 RawImage UI 组件")]
    public RawImage outputRawImage;
    public Canvas canvas;

    /// <summary>
    /// 获取 3D 物体在 RawImage UI 上的本地坐标位置。
    /// </summary>
    /// <param name="worldObjectTransform">要追踪的 3D 物体的 Transform。</param>
    /// <returns>物体在 RawImage RectTransform 中的本地坐标。</returns>
    public Vector2 GetRawImageUIPosition(Transform worldObjectTransform)
    {
        if (renderCamera == null || outputRawImage == null)
        {
            Debug.LogError("请在 Inspector 中配置 Render Camera 和 Raw Image!");
            return Vector2.zero;
        }

        // 1. 世界坐标 -> 渲染摄像机的视口坐标 (Viewport Point)
        // 视口坐标范围是 (0, 0) [左下角] 到 (1, 1) [右上角]
        Vector3 viewportPoint = renderCamera.WorldToViewportPoint(worldObjectTransform.position);

        // 检查：物体是否在摄像机后方？
        if (viewportPoint.z < 0)
        {
            // 返回一个特殊值或处理逻辑，表示物体不在可视范围内
            return new Vector2(-10000, -10000); 
        }

        // 2. 将视口坐标映射到 RawImage 的本地坐标 (Local Position)
        RectTransform rawImageRect = outputRawImage.rectTransform;
        Rect rawImageSize = rawImageRect.rect;
    
        // a. 计算 RawImage 内部的像素位置 (基于 0-1 视口)
        float pixelX = viewportPoint.x * rawImageSize.width;
        float pixelY = viewportPoint.y * rawImageSize.height;

        // b. 将像素位置转换为相对于 RawImage 中心 (0, 0) 的本地坐标
        // (这是因为 RawImage 的 RectTransform 通常以中心点为原点)
        Vector2 localUIPosition = new Vector2(
            pixelX - rawImageSize.width * rawImageRect.pivot.x, // 考虑 Pivot 偏移
            pixelY - rawImageSize.height * rawImageRect.pivot.y
        );
    
        // 如果 RawImage 的 Pivot 默认在中心 (0.5, 0.5)，可以简化为：
        // Vector2 localUIPosition = new Vector2(pixelX - rawImageSize.width / 2f, pixelY - rawImageSize.height / 2f);

        return localUIPosition;
    }
    public Vector2 GetLocalZeroScreenPos()
    {
        // 获取 target 世界坐标
        Vector3 worldPos = outputRawImage.GetComponent<RectTransform>().position;

        // 转换为 Canvas 的本地坐标
        Vector3 canvasLocalPos = canvas.transform.InverseTransformPoint(worldPos);

        return canvasLocalPos;
    }

}
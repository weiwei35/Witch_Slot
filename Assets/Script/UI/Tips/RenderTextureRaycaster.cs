using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RenderTextureRaycaster : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("必须设置")]
    public Camera renderCamera;  // 那个拍摄3D物体的相机
    public RawImage rawImage;    // 显示RenderTexture的UI组件

    [Header("设置")]
    public LayerMask targetLayer; // 只需要检测的层级

    private RectTransform rectTransform;
    private bool isHovering = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rawImage == null) rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // 如果你想实现“悬停检测”(Hover)，就在这里写
        if (isHovering)
        {
            CastRayFromRenderTexture();
        }
    }

    // 如果你想实现“点击检测”(Click)，用这个接口
    public void OnPointerClick(PointerEventData eventData)
    {
        CastRayFromRenderTexture();
    }

    // 核心逻辑
    private void CastRayFromRenderTexture()
    {
        // 1. 获取鼠标在屏幕上的位置
        Vector2 mousePos = Input.mousePosition;

        // 2. 将屏幕坐标转换为 RawImage 的局部坐标
        // 注意：如果你的 Canvas 是 ScreenSpace-Overlay，cam 参数传 null
        // 如果是 ScreenSpace-Camera，传你的 UI Camera
        Camera uiCamera = null; 
        if (GetComponentInParent<Canvas>().renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = GetComponentInParent<Canvas>().worldCamera;
        }

        Vector2 localPoint;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            mousePos, 
            uiCamera, 
            out localPoint
        );

        if (isInside)
        {
            // 3. 将局部坐标转换为 0-1 的归一化坐标 (Viewport Coordinates)
            // rect.xMin 和 rect.yMin 考虑了 Pivot 的影响，这是最稳健的写法
            float normalizedX = (localPoint.x - rectTransform.rect.xMin) / rectTransform.rect.width;
            float normalizedY = (localPoint.y - rectTransform.rect.yMin) / rectTransform.rect.height;

            // 4. 让渲染相机从这个视口坐标发射射线
            // ViewportPointToRay 接受 0-1 的坐标，(0,0)是左下角，(1,1)是右上角
            Ray ray = renderCamera.ViewportPointToRay(new Vector3(normalizedX, normalizedY, 0));

            RaycastHit hit;
            // 5. 像平时一样进行射线检测
            if (Physics.Raycast(ray, out hit, 1000f, targetLayer))
            {
                //Debug.Log($"<color=green>触碰到了 Render Texture 里的物体: {hit.collider.name}</color>");
                // 这里可以处理你的逻辑，比如高亮物体
                hit.collider.GetComponent<TipsEventTrigger3D>().ShowTips();
            }
            else
            {
                // B. 射线没打到任何东西 -> 隐藏
                TipsManager.Instance.HideTip();
            }
        }
    }

    // 辅助：鼠标进入 UI 区域
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    // 辅助：鼠标离开 UI 区域
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
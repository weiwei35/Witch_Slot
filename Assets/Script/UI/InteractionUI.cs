using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public CanvasGroup keyHintGroup;
    public TMP_Text keyHintText;
    public Vector3 offset = new Vector3(0, 1.2f, 0);

    private Transform followTarget; // 要跟随的对象（交互物体）

    void Update()
    {
        if (followTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + offset);
            transform.position = screenPos;
        }
    }

    public void ShowKeyHint(bool show, Vector3 worldPos = default, Transform target = null)
    {
        if (!show)
        {
            keyHintGroup.alpha = 0f;
            followTarget = null;
            return;
        }

        keyHintGroup.alpha = 1f;
        keyHintText.text = "按 E 交互";

        followTarget = target;
    }
}
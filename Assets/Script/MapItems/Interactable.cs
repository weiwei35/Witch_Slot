using UnityEngine;

public class Interactable : MonoBehaviour
{
    public SpriteRenderer highlightRenderer;

    public virtual void Highlight(bool on)
    {
        if (highlightRenderer != null){
            highlightRenderer.color = on ? new Color(1f, 1f, 0.5f) : Color.white; // 简单高亮
        }
    }

    // 执行交互
    public virtual void Interact(Player player)
    {
        Debug.Log($"{name} 被交互了");
    }
}
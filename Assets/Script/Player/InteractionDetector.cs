using UnityEngine;
using System.Collections.Generic;

public class InteractionDetector : MonoBehaviour
{
    public float detectRadius = 1.2f;
    public LayerMask interactLayer; // 设置为 Interactable Layer
    public InteractionUI ui;

    private List<Interactable> targets = new List<Interactable>();
    private int currentIndex = -1;  
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        DetectTargets();
        HandleSwitchTarget();
        HandleInteractInput();
    }

    // =============================
    // 1. 侦测可交互对象
    // =============================
    private void DetectTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, interactLayer);

        // 没有可交互对象
        if (hits.Length == 0)
        {
            ClearTargets();
            return;
        }

        // 构建新列表
        List<Interactable> newList = new List<Interactable>();
        foreach (var h in hits)
        {
            if (h.TryGetComponent(out Interactable ia))
            {
                newList.Add(ia);
            }
        }

        // 如果列表不同，则更新选项
        if (!IsSameList(newList))
        {
            UpdateTargetList(newList);
        }
    }

    private bool IsSameList(List<Interactable> other)
    {
        if (other.Count != targets.Count) return false;
        foreach (var t in other)
            if (!targets.Contains(t)) return false;
        return true;
    }

    private void UpdateTargetList(List<Interactable> newList)
    {
        // 清除旧高亮
        foreach (var t in targets)
            t.Highlight(false);

        targets = newList;

        if (targets.Count == 0)
        {
            currentIndex = -1;
            ui.ShowKeyHint(false);
            return;
        }

        // 先尝试根据玩家最后移动方向自动选中
        Vector2 dir = player.GetComponent<PlayerMove>().lastMoveDir;
        int best = FindBestTargetInDirection(dir);

        if (best == -1)
        {
            // 如果方向上没有可交互对象 → fallback 最近的
            best = FindClosestTargetIndex();
        }

        currentIndex = best;

        // 高亮 & UI 更新
        targets[currentIndex].Highlight(true);
        // ui.ShowKeyHint(true, targets[currentIndex].transform.position);
        ui.ShowKeyHint(true, player.transform.position,player.transform);
    }
    private int FindClosestTargetIndex()
    {
        Vector2 playerPos = transform.position;
        float bestDist = Mathf.Infinity;
        int best = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            float dist = Vector2.Distance(playerPos, targets[i].transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = i;
            }
        }

        return best;
    }
    private void ClearTargets()
    {
        foreach (var t in targets)
            t.Highlight(false);

        targets.Clear();
        currentIndex = -1;
        ui.ShowKeyHint(false);
    }

    // =============================
    // 2. WASD 切换交互对象
    // =============================
    private void HandleSwitchTarget()
    {
        if (targets.Count == 0) return;

        Vector2 dir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W)) dir = Vector2.up;
        if (Input.GetKeyDown(KeyCode.S)) dir = Vector2.down;
        if (Input.GetKeyDown(KeyCode.A)) dir = Vector2.left;
        if (Input.GetKeyDown(KeyCode.D)) dir = Vector2.right;

        if (dir == Vector2.zero) return;

        int idx = FindBestTargetInDirection(dir);
        if (idx == -1 || idx == currentIndex) return;

        // 取消旧高亮
        if (currentIndex >= 0 && currentIndex < targets.Count)
            targets[currentIndex].Highlight(false);

        // 更新选中对象
        currentIndex = idx;

        // 新高亮
        targets[currentIndex].Highlight(true);
        // ui.ShowKeyHint(true, targets[currentIndex].transform.position);
        ui.ShowKeyHint(true, player.transform.position,player.transform);
    }

    private int FindBestTargetInDirection(Vector2 dir, float maxAngle = 20)
    {
        if (dir == Vector2.zero || targets.Count == 0)
            return -1;

        Vector2 playerPos = transform.position;
        float bestDist = Mathf.Infinity;
        int bestIndex = -1;

        for (int i = 0; i < targets.Count; i++)
        {
            Vector2 to = ((Vector2)targets[i].transform.position - playerPos).normalized;

            float angle = Vector2.Angle(dir, to);
            if (angle > maxAngle) continue;

            float dist = Vector2.Distance(playerPos, targets[i].transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
    
    // =============================
    // 3. 按下互动键
    // =============================
    private void HandleInteractInput()
    {
        if (currentIndex < 0) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            targets[currentIndex].Interact(player);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}

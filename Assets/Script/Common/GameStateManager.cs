using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("当前游戏状态")]
    [SerializeField] private GameState currentState = GameState.Slot;
    public GameState CurrentState => currentState;

    /// <summary>
    /// 当游戏状态变化时触发 (旧状态, 新状态)
    /// </summary>
    public event Action<GameState, GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 修改游戏状态（包含通知）
    /// </summary>
    public void SetState(GameState newState)
    {
        if (newState == currentState) return;

        GameState oldState = currentState;
        currentState = newState;
        Debug.Log($"[GameState] 切换：{oldState} → {newState}");

        OnGameStateChanged?.Invoke(oldState, newState);
    }

    /// <summary>
    /// 是否为当前状态
    /// </summary>
    public bool Is(GameState state) => currentState == state;
}
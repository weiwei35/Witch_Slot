using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using NaughtyAttributes;
using Random = UnityEngine.Random;
using CodeMonkey.Utils;
using DG.Tweening;

public class PlayerMove : MonoBehaviour 
{
	[Header("Movement Settings")]
	public float gridSize = 1f;
	public float moveDuration = 0.2f;
	public float inputCoolDown = 0.1f;
	private Vector3 lastValidPosition;
	private Vector3 targetPosition;
	private float lastMoveTime;
    
	private Queue<Vector3> moveQueue = new Queue<Vector3>();
	private bool useMoveQueue;

	private Vector3 targetPos;
	private Rigidbody2D rb;
	public bool canMove = false;
	public bool waitMove = false;
	bool isMoving = false;
	
	private SpriteRenderer playerSprite;
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		playerSprite = GetComponent<SpriteRenderer>();
		lastValidPosition = transform.position;
		targetPosition = transform.position;
	}

	private void OnDisable()
	{
		DOTween.KillAll();
	}

	private void Update()
	{
		if(!GameStateManager.Instance.Is(GameState.Walking)) return;
		if(waitMove) return;
		// 冷却时间控制
		if (Time.time - lastMoveTime < inputCoolDown) return;
        
		// 队列控制系统
		if (useMoveQueue && moveQueue.Count > 0)
		{
			Vector3 nextMove = moveQueue.Dequeue();
			StartCoroutine(MoveToGrid(nextMove));
		}
		if (canMove && !isMoving)
		{
			// 键盘输入检测
			if (Input.GetKey(KeyCode.A))
			{
				RegisterMove(-Vector3.right);
				playerSprite.flipX = true;
			}
			else if (Input.GetKey(KeyCode.D))
			{
				RegisterMove(Vector3.right);
				playerSprite.flipX = false;
			}
			else if (Input.GetKey(KeyCode.W))
			{
				RegisterMove(Vector3.up);
			}
			else if (Input.GetKey(KeyCode.S))
			{
				RegisterMove(-Vector3.up);
			}
		}
	}

	// 统一注册移动请求
	private void RegisterMove(Vector3 direction)
	{
		if (isMoving) 
		{
			if(useMoveQueue) moveQueue.Enqueue(direction);
			return;
		}
        
		StartCoroutine(MoveToGrid(direction));
	}
	// 网格精确移动协程
	private IEnumerator MoveToGrid(Vector3 direction)
	{
		isMoving = true;
		lastMoveTime = Time.time;
        
		// === 核心改进：网格中心化计算 ===
		// 1. 计算当前所在的网格中心点位置
		Vector3 currentCenter = GetCurrentGridCenter();
        
		// 2. 计算目标网格的中心位置
		Vector3 targetCenter = currentCenter + direction * gridSize;
        
		// 3. 更新位置记录
		targetPosition = targetCenter;
		lastValidPosition = targetPosition;
        
		// === 使用刚体进行物理移动 ===
		yield return rb.DOMove(targetPosition, moveDuration)
			.SetEase(Ease.OutQuad)
			.SetUpdate(UpdateType.Fixed)
			.WaitForCompletion();
        
		// === 最终位置校正 ===
		// 强制对齐到网格中心（避免浮点误差）
		SnapToGridCenter();
        
		isMoving = false;
	}
    
	// 获取当前位置所属网格的中心
	private Vector3 GetCurrentGridCenter()
	{
		// 计算当前所在的网格索引
		int gridX = Mathf.RoundToInt(transform.position.x / gridSize - 0.5f);
		int gridY = Mathf.RoundToInt(transform.position.y / gridSize - 0.5f);
        
		// 计算网格中心坐标
		float centerX = (gridX + 0.5f) * gridSize;
		float centerY = (gridY + 0.5f) * gridSize;
        
		return new Vector3(centerX, centerY, 0);
	}
    
	// 将角色对齐到最近的网格中心
	private void SnapToGridCenter()
	{
		Vector3 gridCenter = GetCurrentGridCenter();
		rb.MovePosition(gridCenter);
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		// 只在移动时检测碰撞
		if (!isMoving) return;

		if (other.CompareTag("TouchableItem"))
		{
			rb.DOComplete();
			// 清空已排队的移动
			moveQueue.Clear();
			isMoving = false;
		}
		if (other.CompareTag("Enemy") || other.CompareTag("MapWall"))
		{
			rb.DOComplete();
			rb.MovePosition(lastValidPosition); // 回退到上次有效位置
			
			// 清空已排队的移动
			moveQueue.Clear();
			isMoving = false;
        
			// 可选：添加碰撞反馈效果
			ShakeCamera(0.05f, 0.15f);
		}

	}

	// 可选：碰撞反馈效果
	private void ShakeCamera(float duration, float strength)
	{
		Camera.main.DOShakePosition(duration, strength);
	}
}

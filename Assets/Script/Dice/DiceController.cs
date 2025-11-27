using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("active");
    [HideInInspector]
    public SymbolSO runtimeSymbolData;
    public SymbolSO runtimeSymbolData_extra;
    [Header("设置")]
    public float throwForce = 10f;      // 向上抛的力
    public float rollTorque = 20f;      // 旋转的力
    public Rigidbody rb;

    // 在 Inspector 中配置这6个面
    [HideInInspector]
    public DiceFace[] diceFaces; 

    private bool canTrigger = false;
    private int result = 0;
    private Animator _anim;
    bool animFinished;
    public bool IsAnimationFinished => animFinished;

    bool isLanded = false;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void InitData(DiceFace[] dice)
    {
        diceFaces = dice;
    }
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public IEnumerator RollDice()
    {
        _anim.enabled = false;
        // 1. 随机初始旋转，保证每次起始状态不同
        transform.rotation = Random.rotation;

        // 2. 施加力和扭矩 (向上抛 + 随机旋转)
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * rollTorque, ForceMode.Impulse);

        // 3. 等待物理引擎处理
        yield return new WaitForSeconds(0.5f);

        // 4. 检测是否停止 (速度极小且角速度极小)
        while (rb.velocity.sqrMagnitude > 0.05f || rb.angularVelocity.sqrMagnitude > 0.05f)
        {
            yield return null;
        }

        // 5. 计算结果
        result = CalculateResult();
        canTrigger = true;
        _anim.enabled = true;
    }

    int CalculateResult()
    {
        // 核心逻辑：我们要找哪一个面的局部方向(Local Direction)，
        // 在旋转后，最接近世界坐标的“上方”(Vector3.up)。
        
        float maxDot = -1f;
        int bestValue = 0;

        foreach (var face in diceFaces)
        {
            // 将局部方向转换为世界方向
            Vector3 worldDir = transform.TransformDirection(face.localDir);
            
            // 计算与世界UP的点积 (1表示完全同向)
            float dot = Vector3.Dot(worldDir, Vector3.up);

            if (dot > maxDot)
            {
                maxDot = dot;
                bestValue = face.value;
            }
        }

        return bestValue;
    }

    public void SetActiveAnimation(bool active)
    {
        animFinished = false;
        _anim.SetTrigger(Active);

        // 在 Animator event 调用：AnimationFinished()
    }
    public void AnimationFinished()
    {
        animFinished = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canTrigger) return;
        if(isLanded) return;
        int diceType = 0;
        if (other.CompareTag("AttackArea"))
        {
            diceType = 1;
        }

        if (other.CompareTag("DefenceArea"))
        {
            diceType = 2;
        }
        DiceManager.instance.OnOnDiceEnded(this,result,diceType);
        Debug.Log("<color=yellow>骰子结果: " + result + "</color>");
        canTrigger = false;
        isLanded = true;
    }
}
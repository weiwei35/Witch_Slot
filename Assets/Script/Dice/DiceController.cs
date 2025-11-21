using UnityEngine;
using System.Collections;

public class DiceController : MonoBehaviour
{
    private static readonly int Active = Animator.StringToHash("active");
    public SymbolSO symbolData;
    public SymbolSO runtimeSymbolData;
    [Header("设置")]
    public float throwForce = 10f;      // 向上抛的力
    public float rollTorque = 20f;      // 旋转的力
    public Rigidbody rb;

    private bool isRolling = false;

    // 这是一个结构体，用来记录每个面的方向和对应的数字
    [System.Serializable]
    public struct DiceFace {
        public string name;      // 方便调试的名字 (如 "Top", "Front")
        public Vector3 localDir; // 面的局部朝向 (如 Vector3.up)
        public int value;        // 这个面对应的数字
    }

    // 在 Inspector 中配置这6个面
    public DiceFace[] diceFaces; 

    private Animator _anim;
    bool animFinished = false;
    public bool IsAnimationFinished => animFinished;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    void Start()
    {
        runtimeSymbolData = Instantiate(symbolData);
        DiceManager.instance.dictList.Add(this);
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        // 如果你没有手动配置，这里会给一套默认值（基于标准Unity Cube）
        if (diceFaces == null || diceFaces.Length == 0) {
            SetupDefaultFaces();
        }
    }

    public void ResetDice()
    {
        runtimeSymbolData = Instantiate(symbolData);
    }

    void Update()
    {
        // 按下空格键投掷
        // if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        // {
        //     StartCoroutine(RollDice());
        // }
    }

    public IEnumerator RollDice()
    {
        isRolling = true;

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
        int result = CalculateResult();
        DiceManager.instance.OnOnDiceEnded(this,result);
        Debug.Log("<color=yellow>骰子结果: " + result + "</color>");

        isRolling = false;
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

    // 设置标准默认值 (你需要根据你的贴图修改这里，或者在Inspector里改)
    void SetupDefaultFaces()
    {
        diceFaces = new DiceFace[] {
            new DiceFace { name = "Top (+Y)", localDir = Vector3.up, value = 1 },
            new DiceFace { name = "Bottom (-Y)", localDir = Vector3.down, value = 6 },
            new DiceFace { name = "Front (+Z)", localDir = Vector3.forward, value = 3 },
            new DiceFace { name = "Back (-Z)", localDir = Vector3.back, value = 4 },
            new DiceFace { name = "Right (+X)", localDir = Vector3.right, value = 2 },
            new DiceFace { name = "Left (-X)", localDir = Vector3.left, value = 5 }
        };
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
}
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    private void Awake()
    {
        instance = this;
    }

    public LayerMask overlapLayerMask;
    public float checkRadius;
    public List<WeaponController> weaponList;
    public RawImageToUI attackRawImage;
    public RawImageToUI defenceRawImage;
    public GameObject attackDiceParent;
    public GameObject defenceDiceParent;
    public DiceFlyAnim diceFlyPrefab;
    public SymbolSO attackSymbol;
    public SymbolSO defenceSymbol;
    private List<DiceController> diceList = new List<DiceController>();
    private Dictionary<DiceController,int> diceResults_attack = new Dictionary<DiceController, int>();
    private Dictionary<DiceController,int> diceResults_defence = new Dictionary<DiceController, int>();
    private int endedDiceCount;
    public event Action<DiceController,int,int> OnDiceEnded;
    private bool isRolling;

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChanged += CheckSlotState;
        OnDiceEnded += EndOneDice;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= CheckSlotState;
        OnDiceEnded -= EndOneDice;
    }
    private void CheckSlotState(GameState arg1, GameState state)
    {
        if (state == GameState.Slot)
        {
            StartCoroutine(InitDiceGroup());
        }
    }

    private void ResetDiceGroup()
    {
        endedDiceCount = 0;
        diceResults_attack.Clear();
        diceResults_defence.Clear();
        SymbolSystem.Instance.ResetSymbols();
        foreach (var dice in diceList)
        {
            Destroy(dice.gameObject);
        }
        diceList.Clear();
    }
    //根据武器携带的骰子初始化骰子库
    IEnumerator InitDiceGroup()
    {
        ResetDiceGroup();
        List<DiceFlyAnim> diceFlyObj = new List<DiceFlyAnim>();
        foreach (var weapon in weaponList)
        {
            for (int i = 0; i < weapon.diceList.Count; i++)
            {
                DiceController currentDice = null;
                if (weapon.diceList[i].diceType == DiceType.Attack)
                {
                    Vector3 spawnPos = GetNonOverlapPoint();//骰子出生位置
                    var dice = Instantiate(weapon.diceList[i].dice, attackDiceParent.transform);
                    dice.GetComponent<DiceSkinController>().data = weapon.diceList[i].diceData;
                    dice.runtimeSymbolData = Instantiate(attackSymbol);
                    dice.GetComponent<TipsEventTrigger3D>().SetTips(dice.runtimeSymbolData);
                    dice.transform.localPosition = spawnPos;
                    dice.gameObject.SetActive(false);
                    var fly = Instantiate(diceFlyPrefab,weapon.transform.position,Quaternion.identity,attackRawImage.canvas.transform);
                    Vector3 endPos = attackRawImage.GetLocalZeroScreenPos() +
                                     attackRawImage.GetRawImageUIPosition(dice.transform);
                    fly.target = endPos;
                    diceList.Add(dice);
                    diceFlyObj.Add(fly);
                    currentDice = dice;
                }
                else if (weapon.diceList[i].diceType == DiceType.Defence)
                {
                    Vector3 spawnPos = GetNonOverlapPoint();
                    var dice = Instantiate(weapon.diceList[i].dice, defenceDiceParent.transform);
                    dice.GetComponent<DiceSkinController>().data = weapon.diceList[i].diceData;
                    dice.runtimeSymbolData = Instantiate(defenceSymbol);
                    dice.GetComponent<TipsEventTrigger3D>().SetTips(dice.runtimeSymbolData);
                    dice.transform.localPosition = spawnPos;
                    dice.gameObject.SetActive(false);
                    var fly = Instantiate(diceFlyPrefab,weapon.transform.position,Quaternion.identity,defenceRawImage.canvas.transform);
                    Vector3 endPos = defenceRawImage.GetLocalZeroScreenPos() +
                                     defenceRawImage.GetRawImageUIPosition(dice.transform);
                    fly.target = endPos;
                    diceList.Add(dice);
                    diceFlyObj.Add(fly);
                    currentDice = dice;
                }

                if (weapon.diceList[i].symbol != null)
                {
                    //骰子有附加属性
                    if (currentDice != null)
                    {
                        currentDice.runtimeSymbolData_extra = Instantiate(weapon.diceList[i].symbol);
                        currentDice.GetComponent<TipsEventTrigger3D>().SetTips(currentDice.runtimeSymbolData_extra);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < diceFlyObj.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(diceFlyObj[i].DiceFly(diceList[i].gameObject));
            // diceList[i].gameObject.SetActive(true);
            // Destroy(diceFlyObj[i].gameObject);
        }
    }
    Vector3 GetNonOverlapPoint()
    {
        Vector3 pos;
        int tryCount = 0;

        do
        {
            pos = new Vector3(
                Random.Range(-2f, 2f),
                1f,
                Random.Range(-4f, 4f)
            );

            tryCount++;
            if (tryCount > 50)
            {
                Debug.LogWarning("超过50次仍未找到可用点，强制放置。");
                break;
            }

        } while (Physics.CheckSphere(pos, checkRadius, overlapLayerMask));

        return pos;
    }
    void Update()
    {
        // 按下空格键投掷
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling &&GameStateManager.Instance.Is(GameState.Slot))
        {
            isRolling = true;
            foreach (var dice in diceList)
            {
                StartCoroutine(dice.RollDice());
            }
        }
    }
    private void EndOneDice(DiceController diceController,int diceAmount,int diceType = 0)
    {
        endedDiceCount++;
        switch (diceType)
        {
            case 1 :
                if (!diceResults_attack.TryAdd(diceController, diceAmount))
                {
                    diceResults_attack[diceController] += diceAmount;
                }

                break;
            case 2:
                if (!diceResults_defence.TryAdd(diceController, diceAmount))
                {
                    diceResults_defence[diceController] += diceAmount;
                }

                break;
        }
        
        if (endedDiceCount >= diceList.Count)
        {
            GetDiceGroupResult();
        }
    }

    private void GetDiceGroupResult()
    {
        List<SymbolSO> resultSymbols_endSlot = new List<SymbolSO>();
        //所有骰子得到结果
        foreach (var dice in diceResults_attack)
        {
            Debug.Log("<color=yellow>符文: "+dice.Key.runtimeSymbolData.displayName+"<color=yellow>结果: " + dice.Value + "</color>");
            dice.Key.runtimeSymbolData.effects[0].value = dice.Value;
            dice.Key.runtimeSymbolData.description += dice.Value+"点";
            dice.Key.GetComponent<TipsEventTrigger3D>().ChangeTips(dice.Key.runtimeSymbolData);
            
            var inst = new SymbolInstance(dice.Key.runtimeSymbolData, dice.Value, dice.Value);
            SymbolSystem.Instance.uiLookup.Add(inst,dice.Key);
            resultSymbols_endSlot.Add(dice.Key.runtimeSymbolData);
            
            //如果有特殊符文，需要额外生效
            if (dice.Key.runtimeSymbolData_extra != null)
            {
                dice.Key.runtimeSymbolData_extra.effects[0].value = dice.Value;
                dice.Key.runtimeSymbolData_extra.description += dice.Value + "点";
                dice.Key.GetComponent<TipsEventTrigger3D>().ChangeTips(dice.Key.runtimeSymbolData_extra);
                var inst_extra = new SymbolInstance(dice.Key.runtimeSymbolData_extra, dice.Value, dice.Value);
                SymbolSystem.Instance.uiLookup.Add(inst_extra, dice.Key);
                resultSymbols_endSlot.Add(dice.Key.runtimeSymbolData_extra);
            }
        }
        foreach (var dice in diceResults_defence)
        {
            Debug.Log("<color=yellow>符文: "+dice.Key.runtimeSymbolData.displayName+"<color=yellow>结果: " + dice.Value + "</color>");
            dice.Key.runtimeSymbolData.effects[0].value = dice.Value;
            dice.Key.runtimeSymbolData.description += dice.Value+"点";
            dice.Key.GetComponent<TipsEventTrigger3D>().ChangeTips(dice.Key.runtimeSymbolData);
            
            var inst = new SymbolInstance(dice.Key.runtimeSymbolData, dice.Value, dice.Value);
            SymbolSystem.Instance.uiLookup.Add(inst,dice.Key);
            resultSymbols_endSlot.Add(dice.Key.runtimeSymbolData);
            
            //如果有特殊符文，需要额外生效
            if (dice.Key.runtimeSymbolData_extra != null)
            {
                dice.Key.runtimeSymbolData_extra.effects[0].value = dice.Value;
                dice.Key.runtimeSymbolData_extra.description += dice.Value + "点";
                dice.Key.GetComponent<TipsEventTrigger3D>().ChangeTips(dice.Key.runtimeSymbolData_extra);
                var inst_extra = new SymbolInstance(dice.Key.runtimeSymbolData_extra, dice.Value, dice.Value);
                SymbolSystem.Instance.uiLookup.Add(inst_extra, dice.Key);
                resultSymbols_endSlot.Add(dice.Key.runtimeSymbolData_extra);
            }
        }
        SymbolSystem.Instance.ApplySlotResult(resultSymbols_endSlot);
        SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnSlotResolved);
        
        isRolling = false;
    }

    public void OnOnDiceEnded(DiceController arg1, int arg2, int arg3)
    {
        OnDiceEnded?.Invoke(arg1, arg2, arg3);
    }
}

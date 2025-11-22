using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<DiceController> dictList = new List<DiceController>();
    private Dictionary<DiceController,int> diceResults = new Dictionary<DiceController, int>();
    private int endedDiceCount;
    public event Action<DiceController,int> OnDiceEnded;
    private bool isRolling;

    private void Start()
    {
        OnDiceEnded += EndOneDice;
    }

    private void OnDestroy()
    {
        OnDiceEnded -= EndOneDice;
    }
    void Update()
    {
        // 按下空格键投掷
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling &&GameStateManager.Instance.Is(GameState.Slot))
        {
            isRolling = true;
            endedDiceCount = 0;
            diceResults.Clear();
            SymbolSystem.Instance.ResetSymbols();
            foreach (var dice in dictList)
            {
                dice.ResetDice();
                StartCoroutine(dice.RollDice());
            }
        }
    }
    private void EndOneDice(DiceController diceController,int diceAmount)
    {
        endedDiceCount++;
        diceResults.Add(diceController,diceAmount);
        if (endedDiceCount >= dictList.Count)
        {
            List<SymbolSO> resultSymbols_endSlot = new List<SymbolSO>();
            //所有骰子得到结果
            foreach (var dice in diceResults)
            {
                Debug.Log("<color=yellow>符文: "+dice.Key.runtimeSymbolData.displayName+"<color=yellow>结果: " + dice.Value + "</color>");
                dice.Key.runtimeSymbolData.effects[0].value = dice.Value;
                dice.Key.runtimeSymbolData.description += dice.Value+"点";
                
                var inst = new SymbolInstance(dice.Key.runtimeSymbolData, dice.Value, dice.Value);
                SymbolSystem.Instance.uiLookup.Add(inst,dice.Key);
                resultSymbols_endSlot.Add(dice.Key.runtimeSymbolData);
            }
            
            SymbolSystem.Instance.ApplySlotResult(resultSymbols_endSlot);
            SymbolSystem.Instance.NotifyEvent(TriggerEvent.OnSlotResolved);
            
            isRolling = false;
        }
    }

    public void OnOnDiceEnded(DiceController arg1, int arg2)
    {
        OnDiceEnded?.Invoke(arg1, arg2);
    }
}

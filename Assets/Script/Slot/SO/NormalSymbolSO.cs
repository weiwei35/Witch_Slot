using UnityEngine;

[CreateAssetMenu(menuName = "Game/Symbol/Normal Symbol", fileName = "NormalSymbol")]
public class NormalSymbolSO : BaseSymbolSO
{
    [Header("数值符号属性")]
    public float amount;
}
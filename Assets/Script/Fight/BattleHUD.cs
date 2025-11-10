using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text strengthText;
    public TMP_Text defenseText;
    public Image iconImage;
    public Scrollbar hpSlider;

    private CharacterRuntimeData boundData;

    public void Bind(CharacterRuntimeData data)
    {
        if (boundData != null)
            boundData.OnValueChanged -= Refresh;

        boundData = data;

        if (boundData != null)
        {
            boundData.OnValueChanged += Refresh;
            Refresh();
        }
        else
        {
            Clear();
        }
    }

    void Refresh()
    {
        if (boundData == null) return;

        nameText.text = boundData.Name;
        strengthText.text = boundData.Strength.ToString();
        defenseText.text = boundData.Defense.ToString();
        hpSlider.size = boundData.CurrentHP / boundData.MaxHP;
        if (iconImage) iconImage.sprite = boundData.Icon;
    }

    void Clear()
    {
        nameText.text = "-";
        strengthText.text = "-";
        defenseText.text = "-";
        hpSlider.size = 0;
        if (iconImage) iconImage.sprite = null;
    }

    private void OnDisable()
    {
        if (boundData != null)
            boundData.OnValueChanged -= Refresh;
    }
}
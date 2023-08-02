using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战斗技能按键
/// </summary>
public class BattleSkillUISlot : BasePanel
{
    [SerializeField] Text skillNameText;
    [SerializeField] Text ppText;
    [SerializeField] Image skillSlotTypeImage;
    [SerializeField] Image effectivenessImage;
    public void SetData(string skillName, string pp, ref Sprite typeSprite, Sprite effectivenessSprite)
    {
        OnOpen();
        skillNameText.text = skillName;
        ppText.text = pp;
        skillSlotTypeImage.sprite = typeSprite;
        effectivenessImage.sprite = effectivenessSprite;
    }

    public void Refresh(string pp)
    {
        ppText.text = pp;
    }
}
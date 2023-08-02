using UnityEngine;
using UnityEngine.UI;
public class CircleUI : BasePanel
{
    [SerializeField] Image circleHpBar;
    [SerializeField] Image pokemonSprite;
    public override void SetData(Pokemon pokemon)
    {
        OnOpen();

        //Hp圆条
        float percent = pokemon.HPPercent;
        circleHpBar.fillAmount = percent;
        if(pokemon.Status != null)
        {
            ConditionID conditionID = pokemon.Status.ConditionID;
            switch(conditionID)
            {
                case ConditionID.psn: circleHpBar.color = MyData.hpPurple;     break;
                case ConditionID.hyp: circleHpBar.color = MyData.hpDeepPurple; break;
                case ConditionID.brn: circleHpBar.color = MyData.hpORed;       break;
                case ConditionID.frz: circleHpBar.color = MyData.iceBlue;      break;
                case ConditionID.par: circleHpBar.color = MyData.parYellow;    break;
            }
        }
        else
        {
            circleHpBar.color = percent > 0.5f? MyData.hp_green : 0.3f < percent? MyData.hp_orange : MyData.hp_red;
        }

        //mini图
        pokemonSprite.sprite = ResM.Instance.LoadSprite(string.Concat(MyData.miniSprite, pokemon.Base.ID.ToString(), pokemon.Shiny? "s" : null));
    }
}
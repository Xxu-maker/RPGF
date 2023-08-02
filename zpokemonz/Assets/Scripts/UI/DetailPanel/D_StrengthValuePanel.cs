using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 2属性
/// </summary>
public class D_StrengthValuePanel : BasePanel
{
    [SerializeField] Text[] baseText;
    [SerializeField] Text[] ivText;//个体值
    [SerializeField] Text[] efText;//努力值
    [SerializeField] HPBar hpBar;
    [SerializeField] Text natureText;
    [SerializeField] List<CanvasGroup> dynamaxImages;
    public override void SetData(Pokemon pokemon)
    {
        OnOpen();

        //基础数值
        baseText[0].text = pokemon.PAttack.ToString();
        baseText[1].text = pokemon.PDefence.ToString();
        baseText[2].text = pokemon.SAttack.ToString();
        baseText[3].text = pokemon.SDefence.ToString();
        baseText[4].text = pokemon.Speed.ToString();

        //性格
        natureText.text = pokemon.Nature.Name;
        //性格加成变色
        foreach(Text text in baseText)
        {
            if(text.color != Color.black)
            {
                text.color = Color.black;
            }
        }

        int n = pokemon.Nature.Up;
        if(n != 6)
        {
            baseText[n].color = Color.red;
            baseText[pokemon.Nature.Down].color = Color.blue;
        }

        //个体值和努力值
        for(int i = 0; i < 6; ++i)
        {
            ivText[i].text = pokemon.IV[i].ToString();
            efText[i].text = pokemon.BasePoints[i].ToString();
        }

        //Hp
        int c = pokemon.HP; int m = pokemon.MaxHP;
        baseText[5].text = string.Concat(c.ToString(), "/", m.ToString());
        hpBar.SetHP((float)c / m);

        SetDynamaxBar(Random.Range(0, 10));
    }

    /// <summary>
    /// 设置极巨化等级条
    /// </summary>
    /// <param name="value"></param>
    public void SetDynamaxBar(int value)
    {
        value--;
        for(int i = 0; i < 10; ++i)
        {
            if(i > value)
            {
                dynamaxImages[i].alpha = 0;
            }
            else
            {
                dynamaxImages[i].alpha = 1;
            }
        }
    }
}
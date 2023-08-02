using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 1简介
/// </summary>
public class D_IntroductionPanel : BasePanel
{
    [SerializeField] Text nickName;
    [SerializeField] Image type1;
    [SerializeField] Image type2;
    [SerializeField] Text trainerName;
    [SerializeField] Text uid;
    [SerializeField] Text ability;
    [SerializeField] Text expTip;
    [SerializeField] Image expBar;
    [SerializeField] Transform trans;
    private Sprite[] typeSprites;
    public override void SetData(Pokemon pokemon)
    {
        OnOpen();
        nickName.text = pokemon.NickName;
        trainerName.text = "斗也";

        int level = pokemon.Level;
        GrowthRate growthRate = pokemon.Base.GrowthRate;
        //经验
        int currentLevelExp = ExpArray.GetExpForLevelAndGrowthRate(level    , growthRate);
        int nextLevelExp    = ExpArray.GetExpForLevelAndGrowthRate(level + 1, growthRate);

        if(level != 100)
        {
            expBar.transform.localScale = new Vector3
            (
                Mathf.Clamp01((float)(pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp)),
                1f, 1f
            );

            expTip.text = string.Concat("还差", (nextLevelExp - pokemon.Exp).ToString(), "点经验");
        }
        else
        {
            expTip.text = "已经满级";
        }


        if(typeSprites == null)
        {
            typeSprites = ResM.Instance.LoadAllSprites("Type");
        }

        //特性
        if(pokemon.Ability != null)
        {
            ability.text = pokemon.Ability.Name;
        }
        else
        {
            ability.text = "还没有加";
        }

        //属性图
        type1.sprite = typeSprites[(int)pokemon.Base.Type1];

        if(pokemon.Base.Type2 != PokemonType.None)
        {
            type2.color = Color.white;
            type2.sprite = typeSprites[(int)pokemon.Base.Type2];
        }
        else
        {
            type2.color = MyData.hyaline;
        }
    }
}
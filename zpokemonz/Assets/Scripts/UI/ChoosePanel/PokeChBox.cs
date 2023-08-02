using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 宝可梦选择用单元格
/// </summary>
public class PokeChBox : BasePanel
{
    [Header("画布组")]
    [SerializeField] CanvasGroup hpBarAndOther;//血条及血条数值 异常状态 属性 性别
    [SerializeField] CanvasGroup messageButtonCG;//详细按键(功能没做)

    [Header("Base")]
    [SerializeField] Image iconImage;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image sexImage;
    [SerializeField] Image type1;
    [SerializeField] Image type2;
    [SerializeField] Image conditionImage;
    [SerializeField] Image itemImage;
    [SerializeField] Text hpText;
    [SerializeField] Text maxHpText;
    [SerializeField] HPBar hpBar;

    [Header("努力值相关")]
    [SerializeField] Text basePointName;
    [SerializeField] Text singleBasePointValueText;
    [SerializeField] Text totalBasePointValueText;
    [SerializeField] CanvasGroup basePointPanelCG;

    [Header("特性")]
    [SerializeField] Text abilityName;
    [SerializeField] CanvasGroup abilityCG;
    Pokemon pokemon;

    /// <summary>
    /// 设置选择盒子数据
    /// </summary>
    /// <param name="pokemon">同位置宝可梦</param>
    /// <param name="open">是否开启点击</param>
    public void SetData(Pokemon _pokemon, bool open = true)
    {
        pokemon = _pokemon;
        if(open)
        {
            OnOpen();
        }
        else
        {
            OnCover();
        }
        iconImage.sprite = ResM.Instance.LoadSprite(string.Concat(MyData.miniSprite, pokemon.Base.ID.ToString(), pokemon.Shiny? "s" : null));
        nameText.text = pokemon.NickName;
        levelText.text = pokemon.Level.ToString();

        sexImage.color = MyData.hyaline;
        type1.sprite = ResM.Instance.LoadSprite(string.Concat("TMini/", ((int)pokemon.Base.Type1).ToString()));
        type2.sprite = ResM.Instance.LoadSprite(string.Concat("TMini/", ((int)pokemon.Base.Type2).ToString()));
        if(type2.sprite == null)
        {
            type2.color = MyData.hyaline;
        }

        if(pokemon.ItemBase == null)
        {
            itemImage.color = MyData.hyaline;
        }
        else
        {
            itemImage.color = Color.white;
            itemImage.sprite = pokemon.ItemBase.ItemSprite;
        }

        if(pokemon.Status == null)
        {
            conditionImage.color = MyData.hyaline;
        }
        else
        {
            conditionImage.color = Color.white;
            conditionImage.sprite = ResM.Instance.Load<Sprite>(string.Concat("Status/", pokemon.Status.ConditionID.ToString()));
        }

        hpText.text = pokemon.HP.ToString();
        maxHpText.text = pokemon.MaxHP.ToString();
        hpBar.SetHP(pokemon.HPPercent);

        ShowOrHide(messageButtonCG, true);
        basePointPanelCG.alpha = 0;
    }

    private int itemBasePointType;
    /// <summary>
    /// 努力值道具使用面板
    /// </summary>
    public void SetBasePointData(Pokemon pokemon, bool open, BasePointType type)
    {
        this.pokemon = pokemon;
        if(open)
        {
            OnOpen();
        }
        else
        {
            OnCover();
        }
        basePointName.text = type.ToString();
        totalBasePointValueText.text = pokemon.TotalBasePointsValue().ToString();

        itemImage.color = MyData.hyaline;
        //type1.color = MyData.hyaline;
        //type2.color = MyData.hyaline;
        iconImage.sprite = ResM.Instance.LoadSprite(string.Concat(MyData.miniSprite, pokemon.Base.ID.ToString(), pokemon.Shiny? "s" : null));;
        singleBasePointValueText.text = pokemon.BasePoints[(int)type].ToString();
        nameText.text = pokemon.NickName;
        levelText.text = pokemon.Level.ToString();
        hpBarAndOther.alpha = 0;
        ShowOrHide(messageButtonCG, false);
        basePointPanelCG.alpha = 1;
    }

    /// <summary>
    /// 修改特性面板
    /// </summary>
    public void SetAbilityData(Pokemon pokemon)
    {
        abilityName.text = pokemon.Ability == null? "还没做好" : pokemon.Ability.Name;
        //if()特性只有一个，就不用按

        itemImage.color = MyData.hyaline;
        //type1.color = MyData.hyaline;
        //type2.color = MyData.hyaline;
        hpBarAndOther.alpha = 0;
        ShowOrHide(messageButtonCG, false);
        basePointPanelCG.alpha = 0;
        abilityCG.alpha = 1;
    }

    public void ShowMessagePanel()
    {
        //print("1");
        //进入详细信息
    }

    public void CarryOnItem(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        OnOpen();

        //mini图
        iconImage.sprite = ResM.Instance.LoadSprite(string.Concat(MyData.miniSprite, pokemon.Base.ID.ToString(), pokemon.Shiny? "s" : null));

        nameText.text = pokemon.NickName;
        levelText.text = pokemon.Level.ToString();

        sexImage.color = MyData.hyaline;
        //加载属性方块图
        type1.sprite = ResM.Instance.LoadSprite(string.Concat("TMini/", ((int)pokemon.Base.Type1).ToString()));
        type2.sprite = ResM.Instance.LoadSprite(string.Concat("TMini/", ((int)pokemon.Base.Type2).ToString()));
        if(type2.sprite == null)
        {
            type2.color = MyData.hyaline;
        }

        if(pokemon.ItemBase == null)
        {
            itemImage.color = MyData.hyaline;
        }
        else
        {
            itemImage.color = Color.white;
            itemImage.sprite = pokemon.ItemBase.ItemSprite;
        }

        conditionImage.color = MyData.hyaline;
        hpBarAndOther.alpha = 0;

        ShowOrHide(messageButtonCG, true);
        basePointPanelCG.alpha = 0;
    }
    /*/// <summary>
    /// 没有弄好 回血动画
    /// </summary>
    /// <param name="hp"></param>
    public void HpBarAnim(int hp)
    {
        float percent = (float)(pokemon.HP + hp) / pokemon.MaxHP;
        if(percent >= 1)
        {
            percent = 1f;
        }
        StartCoroutine(hpBar.SetHpUpSmooth(percent));
        SetData(pokemon);
    }*/
    /// <summary>
    /// 刷新格子
    /// </summary>
    public void Refresh(bool basePoint)
    {
        if(basePoint)
        {
            int x = pokemon.BasePoints[itemBasePointType];
            if(x < 252 && !pokemon.BasePointsWasMax)
            {
                OnOpen();
                totalBasePointValueText.text = pokemon.TotalBasePointsValue().ToString();
            }
            else
            {
                OnCover();
                totalBasePointValueText.text = "510";
            }
            singleBasePointValueText.text = x.ToString();
        }
        else
        {
            //SetData(pokemon, true);
            /*if(!isCover)
            {
                OnOpen();
            }
            else
            {
                OnCover();
            }*/
            OnCover();
            levelText.text = pokemon.Level.ToString();
            hpText.text = pokemon.HP.ToString();
            maxHpText.text = pokemon.MaxHP.ToString();
            hpBar.SetHP(pokemon.HPPercent);
            if(pokemon.Status == null)
            {
                conditionImage.color = MyData.hyaline;
            }
        }
    }

    public void OnCover()
    {
        Canvas.blocksRaycasts = false;
        Canvas.interactable = false;
    }
}
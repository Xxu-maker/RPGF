using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战斗数据栏
/// </summary>
public class BattleHud : BasePanel
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Text maxHPn;
    [SerializeField] Text currentHPn;
    [SerializeField] Image statusImage;
    [SerializeField] CanvasGroup statusCG;
    Pokemon _pokemon;

    public override void SetData(Pokemon pokemon)
    {
        if(_pokemon != null)
        {
            _pokemon.OnHPChanged     -= UpdateHPAsync  ;
            _pokemon.OnMaxHPChanged  -= UpdateMaxHPText;
            _pokemon.OnStatusChanged -= SetStatusImage ;
        }

        if(Canvas.alpha != 1)
        {
            Canvas.alpha = 1;
        }

        _pokemon = pokemon;
        nameText.text = pokemon.NickName;
        SetLevel();
        SetHp();
        SetExp();
        SetStatusImage();

        _pokemon.OnHPChanged     += UpdateHPAsync  ;
        _pokemon.OnMaxHPChanged  += UpdateMaxHPText;
        _pokemon.OnStatusChanged += SetStatusImage ;
    }

    private void SetStatusImage()
    {
        if(_pokemon.Status == null)
        {
            statusCG.alpha = 0;
        }
        else
        {
            statusCG.alpha = 1;
            statusImage.sprite = ResM.Instance.Load<Sprite>(string.Concat("Status/", _pokemon.Status.ConditionID.ToString()));
        }
    }

    public void SetLevel()
    {
        levelText.text = _pokemon.Level.ToString();
    }

    public void SetHp()
    {
        hpBar.SetHP(_pokemon.HPPercent);

        maxHPn.text = _pokemon.MaxHP.ToString();
        currentHPn.text = _pokemon.HP.ToString();
    }

    public void UpdateHPAsync()
    {
        hpBar.SetHPSmooth(_pokemon.HPPercent);

        if(maxHPn != null)
        {
            currentHPn.text = _pokemon.HP.ToString();
        }
    }

    public void UpdateMaxHPText() => maxHPn.text = _pokemon.MaxHP.ToString();

    /// <summary>
    /// 持有宝可梦数量提示
    /// </summary>
    /// <param name="team"></param>
    public void UpdateTeamPokemonNumber(Pokemon[] team)
    {
        //
    }
#region 经验条
    public void SetExp()
    {
        if(expBar == null) { return; }

        expBar.transform.localScale = new Vector3(GetNormalizedExp(), 1f, 1f);
    }

    public async UniTask SetExpSmooth(bool reset = false)
    {
        if(expBar == null) { return; }

        Vector2 newExp = Vector2.up;//(0,1)
        if(reset)
        {
            //升级声音
            expBar.transform.localScale = newExp;
        }

        float curExp = expBar.transform.localScale.x;
        float normalizedExp = GetNormalizedExp();
        float nt = normalizedExp * Time.deltaTime;
        while (curExp < normalizedExp)
        {
            curExp += nt;
            newExp.x = curExp;
            expBar.transform.localScale = newExp;
            await UniTask.Yield();
        }
        newExp.x = normalizedExp;
        expBar.transform.localScale = newExp;
    }

    float GetNormalizedExp()
    {
        int level = _pokemon.Level;

        if(level == 100)
        {
            return 1f;
        }

        GrowthRate growthRate = _pokemon.Base.GrowthRate;
        int currentLevelExp = ExpArray.GetExpForLevelAndGrowthRate(level,     growthRate);
        int nextLevelExp    = ExpArray.GetExpForLevelAndGrowthRate(level + 1, growthRate);

        return Mathf.Clamp01((float)(_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp));
    }
#endregion
    public void OnHide() => Canvas.alpha = 0;
}
using UnityEngine;
using UnityEngine.UI;
public class BoxBase : BasePanel
{
    [SerializeField] Toggle toggle;//多选用的toggle
    [SerializeField] CanvasGroup tcg;//toggle的画布组
    [SerializeField] PCOnDrag slot;
    public bool havePokemon = false;
    [Header("宝可梦信息")]
    [SerializeField] Image pokemonImage;
    [SerializeField] CanvasGroup lockCG;
    [SerializeField] CanvasGroup item;
    [SerializeField] Image itemImage;
    [SerializeField] CanvasGroup shiny;
    [SerializeField] CanvasGroup sign;
    [SerializeField] CanvasGroup sex;
    [SerializeField] Image sexImage;
    [SerializeField] Text level;
    public override void SetData(Pokemon pokemon)
    {
        if(pokemon.Base == null)
        {
            havePokemon = false;
            slot.Canvas.alpha = 0;
            return;
        }

        lockCG.alpha = pokemon.Lock? 1 : 0;
        sign.alpha = 0;

        havePokemon = true;
        slot.Canvas.alpha = 1;
        sex.alpha = 0;
        item.alpha = 0;
        shiny.alpha = pokemon.Shiny? 1 : 0;
        level.text = pokemon.Level.ToString();
        pokemonImage.sprite = ResM.Instance.LoadSprite(string.Concat(MyData.miniSprite, pokemon.Base.ID.ToString(), pokemon.Shiny? "s" : null));
    }

    public void RefreshLockAndSign(bool lockPKM)
    {
        lockCG.alpha = lockPKM? 1 : 0;
        //sign.alpha = 0;
    }

    /// <summary>
    /// 当前位置
    /// </summary>
    /// <returns></returns>
    public int ID()
    {
        OffSelectMode();
        return slot.ID;
    }

    /// <summary>
    /// 检查toggle是否开启
    /// </summary>
    /// <returns></returns>
    public bool CheckToggle() => toggle.isOn;

    public void ReadySelectMode()
    {
        slot.Canvas.blocksRaycasts = false;
        tcg.blocksRaycasts = havePokemon;
    }

    public void OffSelectMode()
    {
        toggle.isOn = false;
        tcg.blocksRaycasts = false;
        slot.Canvas.blocksRaycasts = true;
    }

    public void CancelModeToggle()
    {
        if(havePokemon)
        {
            toggle.isOn = false;
        }
    }

    public void OnToggle()  => toggle.isOn = havePokemon;
    public void OffToggle() => toggle.isOn = false;
}
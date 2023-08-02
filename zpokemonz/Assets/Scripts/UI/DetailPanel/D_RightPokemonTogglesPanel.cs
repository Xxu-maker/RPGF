using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 详细面板右侧宝可梦切换toggle
/// </summary>
public class D_RightPokemonTogglesPanel : BasePanel
{
    [SerializeField] Image[] pokemonIconImage;//右侧宝可梦mini图
    [SerializeField] CanvasGroup[] rightToggleSlotsCG;//右侧单个toggle画布组列表
    [SerializeField] Toggle rightFirstToggle;//第一个toggle

    public void SetData(ref Pokemon[] pokemons)
    {
        OnOpen();
        Reset();

        for(int i = 0; i < 6; ++i)
        {
            if(pokemons[i].Base == null)
            {
                OnCloseSingleToggle(i);
                for(int u = i; u < 6; ++u)
                {
                    OnCloseSingleToggle(u);
                }
                break;
            }

            //mini图
            SetSingleData
            (
                ResM.Instance.LoadSprite
                (
                    string.Concat(MyData.miniSprite, pokemons[i].Base.ID.ToString(), pokemons[i].Shiny? "s" : null)
                ),
                i
            );
        }
    }

    /// <summary>
    /// 设置单个toggle显示
    /// </summary>
    private void SetSingleData(Sprite sprite, int index)
    {
        ShowOrHide(rightToggleSlotsCG[index], true);
        pokemonIconImage[index].sprite = sprite;
    }

    /// <summary>
    /// 关闭单个toggle
    /// </summary>
    private void OnCloseSingleToggle(int index)
    {
        ShowOrHide(rightToggleSlotsCG[index], false);
    }

    private void Reset() => rightFirstToggle.isOn = true;
}
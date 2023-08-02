using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 饲育屋面板
/// </summary>
public class FosterPanel : BasePanel
{
    [SerializeField] CanvasGroup[] miniCG;
    [SerializeField] Image[] miniImage;
    [SerializeField] Toggle[] bagToggle;
    [SerializeField] CanvasGroup[] fosterCG;
    [SerializeField] Image[] fosterBox;
    [SerializeField] Toggle[] fosterToggle;
    [SerializeField] PokemonTeam playerTeam;

    /// <summary>
    /// foster面板
    /// </summary>
    /// <param name="bagPokemons">背包里的宝可梦</param>
    /// <param name="fosterPokemons">寄存的宝可梦</param>
    public void SetData()
    {
        Pokemon[] bagPokemons    = playerTeam.Pokemons;
        Pokemon[] fosterPokemons = playerTeam.Foster  ;
        OnOpen();

        //背包的
        int length = bagPokemons.Length;
        for(int i = 0; i < length; ++i)
        {
            if(bagPokemons[i].Base == null)
            {
                ShowOrHide(miniCG[i], false);
            }
            else
            {
                ShowOrHide(miniCG[i], true);
                miniImage[i].sprite =
                    ResM.Instance.LoadSprite
                    (
                        string.Concat
                        (
                            MyData.miniSprite, bagPokemons[i].Base.ID.ToString(), bagPokemons[i].Shiny? "s" : null
                        )
                    );
            }
        }

        //寄存的
        length = fosterPokemons.Length;
        for(int i = 0; i < length; ++i)
        {
            if(fosterPokemons[i].Base == null)
            {
                ShowOrHide(fosterCG[i], false);
            }
            else
            {
                ShowOrHide(fosterCG[i], true);
                fosterBox[i].sprite =
                    ResM.Instance.LoadSprite
                    (
                        string.Concat
                        (
                            MyData.miniSprite, fosterPokemons[i].Base.ID.ToString(), fosterPokemons[i].Shiny? "s" : null
                        )
                    );
            }
        }
    }

    /// <summary>
    /// 处理Toggle显示
    /// </summary>
    /// <param name="isBag"></param>
    public void CloseToggle()
    {
        foreach(Toggle toggle in bagToggle)
        {
            toggle.isOn = false;
        }

        foreach(Toggle toggle in fosterToggle)
        {
            toggle.isOn = false;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        CloseToggle();
        UIManager.Instance.ResumeControl();
    }

    public bool NeedChange(int x) => bagToggle[x].isOn;

    public Toggle[] Toggles(bool isBag) => isBag? bagToggle : fosterToggle;

    public void PutInBag() => PutInBagOrFosterButton(true);

    public void PutInFoster() => PutInBagOrFosterButton(false);


    /// <summary>
    /// 放入背包还是放入寄存处
    /// </summary>
    /// <param name="putInBag">true为寄存处放入背包</param>
    public void PutInBagOrFosterButton(bool putInBag)
    {
        Toggle[] toggles = Toggles(!putInBag);
        //17号移到0号
        int box1 = 17; int box2 = 0;
        int find = 0;

        if(!putInBag)//放到牧场
        {
            box1 = 0; box2 = 17;
            find = 17;
        }

        //空位
        int n = playerTeam.FindVacancyByNumber(find);

        int togglesLength = toggles.Length;
        //查看哪些需要交换，并进行交换
        for(int i = 0; i < togglesLength; ++i)
        {
            if(toggles[i].isOn)
            {
                if(n == 61) { break; }
                toggles[i].isOn = false;
                if(playerTeam.PlayerTeamPokemonsCount() != 1 || !putInBag)//只剩一只就不交换了
                {
                    playerTeam.SwapPokemon(box1, i, box2, n);
                    n = playerTeam.FindVacancyByNumber(find);
                }
            }
        }

        playerTeam.SortTeam();
        SetData();
    }
}
using UnityEngine;
using UnityEngine.UI;
public class TeamCirclePanel : BasePanel
{
    [SerializeField] CanvasGroup slotsCG;
    [SerializeField] PokemonTeam _pokemonTeam;
    [SerializeField] Toggle mgrToggle;
    [SerializeField] CircleUI[] circleSlots;
    [SerializeField] PokemonOnDrag[] onDragSlots;

    public void Start()
    {
        mgrToggle.onValueChanged.AddListener((bool isOn) => ShowCircleUI(isOn));

        foreach(PokemonOnDrag slot in onDragSlots)
        {
            slot.SetDelegate(GameManager.Instance.Player.SetFollowPokemon, SwitchPokemonPos);
        }
    }

    public void ShowCircleUI(bool isOn)
    {
        if(isOn)
        {
            OpenCircleSlotsPanel();
        }
        else
        {
            ShowOrHide(slotsCG, isOn);
        }
    }

    /// <summary>
    /// 打开右侧圆形面板
    /// </summary>
    private void OpenCircleSlotsPanel()
    {
        ShowOrHide(slotsCG, true);
        SetTeamData();
    }

    private void SetTeamData()
    {
        Pokemon[] pokemons = _pokemonTeam.Pokemons;
        int length = circleSlots.Length;
        for(int i = 0; i < length; ++i)
        {
            if(pokemons[i].Base != null)
            {
                circleSlots[i].SetData(pokemons[i]);
            }
            else
            {
                circleSlots[i].OnClose();
            }
            //看情况隐藏濒死宝可梦
        }
    }

    /// <summary>
    /// 队伍中交换
    /// </summary>
    public void SwitchPokemonPos(int id, int cid)
    {
        _pokemonTeam.SwapPokemonAndRefresh(0, id, 0, cid, circleSlots[id].SetData, circleSlots[cid].SetData);
    }

    public void UpdateData()
    {
        if(slotsCG.alpha == 1)
        {
            SetTeamData();
        }
    }

    private bool isItOpen = false;
    /// <summary>
    /// PC开启时隐藏
    /// </summary>
    public void Hide()
    {
        if(slotsCG.alpha == 1)
        {
            isItOpen = true;
        }
        OnClose();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void Resume()
    {
        if(isItOpen)
        {
            SetTeamData();
            ShowOrHide(slotsCG, true);
            isItOpen = false;
        }
        OnOpen();
    }
}
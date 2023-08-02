using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokeDesPanel : BasePanel
{
    [Header("退出键画布组")]
    [SerializeField] CanvasGroup exitButtonCG;
    [SerializeField] CanvasGroup backPCButtonCG;

    [Header("右侧宝可梦toggle")]
    [SerializeField] D_RightPokemonTogglesPanel rightPokemonTogglesPanel;

    [Header("宝可梦动画位置")]
    [SerializeField] Transform animatorShowTrans;
    [SerializeField] PokemonAnimator animator;

    [Header("左侧面板")]
    [SerializeField] List<BasePanel> pkmMessagePanels;

    [SerializeField] Text _baseName;
    [SerializeField] Text level;

    private Stack<BasePanel> panelStack = new Stack<BasePanel>();
    Pokemon currentPokemon;
    PokemonTeam pokemonTeam;
    Pokemon[] pokemons;

    private void Start()
    {
        pokemonTeam = GameManager.Instance.PlayerTeam;
        pokemons = pokemonTeam.Pokemons;
        currentPokemon = pokemons[0];
    }

    /// <summary>
    /// 切换面板Toggle
    /// </summary>
    public void SwitchPanel(int n)
    {
        //用栈管理当前Panel
        if(panelStack.Count != 0)
        {
            panelStack.Pop().OnClose();
        }
        pkmMessagePanels[n].SetData(currentPokemon);
        panelStack.Push(pkmMessagePanels[n]);
    }

    private int currentShowPokemonPos = 6;
    /// <summary>
    /// 切换宝可梦Toggle
    /// </summary>
    public void SwitchPokemon(int n)//切换展示宝可梦面板信息
    {
        if(n == currentShowPokemonPos) { return; }

        currentShowPokemonPos = n;
        MemoryPokemon(pokemons[n]);
    }

    public override void OnOpen()
    {
        base.OnOpen();

        ShowOrHide(exitButtonCG, true);

        currentShowPokemonPos = 0;
        MemoryPokemon(pokemons[0]);

        rightPokemonTogglesPanel.SetData(ref pokemons);

        SwitchPanel(0);
    }

    /// <summary>
    /// 从PC打开详细面板
    /// </summary>
    public void OpenFromPC()
    {
        base.OnOpen();
        ShowOrHide(backPCButtonCG, true);
        rightPokemonTogglesPanel.OnClose();
        MemoryPokemon(pokemonTeam.FindPokemon);
    }

    /// <summary>
    /// 切换宝可梦时, 仅更新宝可梦数据
    /// </summary>
    /// <param name="pokemon"></param>
    public void MemoryPokemon(Pokemon pokemon)
    {
        currentPokemon = pokemon;
        _baseName.text = pokemon.NickName;
        level.text = "Lv." + pokemon.Level.ToString();
        if(panelStack.Count == 0)
        {
            panelStack.Push(pkmMessagePanels[0]);
        }
        panelStack.Peek().SetData(currentPokemon);
        animator.SetAnimation(currentPokemon, false, false, false, animatorShowTrans.position);
    }

    /// <summary>
    /// 普通退出界面
    /// </summary>
    public void ExitPanel()
    {
        OnClose();
        ShowOrHide(exitButtonCG, false);
        UIManager.Instance.BackCtrlPanel();
    }

    /// <summary>
    /// 退出从PC打开的界面
    /// </summary>
    public void ExitPanelAndBackPCPanel()
    {
        OnClose();
        ShowOrHide(backPCButtonCG, false);
        UIManager.Instance.PCPanel.Show();
    }

    public override void OnClose()
    {
        base.OnClose();
        animator.ForcedStopAnimator();
    }
}
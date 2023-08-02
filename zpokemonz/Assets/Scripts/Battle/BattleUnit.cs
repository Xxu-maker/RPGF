using UnityEngine;
/// <summary>
/// 战斗单位
/// </summary>
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;
    [SerializeField] PokemonAnimator stage;
    [SerializeField] SpriteRenderer cloneSR;
    [SerializeField] Transform cloneSRTrans;
    private Pokemon _pokemon;

    public bool IsPlayerUnit => isPlayerUnit;
    public BattleHud Hud => hud;
    public PokemonAnimator AnimStage => stage;
    public Pokemon Pokemon => _pokemon;

    public void SetData(Pokemon pokemon, bool mega)
    {
        _pokemon = pokemon;

        hud.SetData(pokemon);

        ChangeSprite(mega, false);
    }

    /// <summary>
    /// 更换stage的宝可梦动画
    /// </summary>
    public void ChangeSprite(bool mega, bool gig)
    {
        stage.SetAnimation(_pokemon, isPlayerUnit, mega, gig, transform.position);
    }

    /// <summary>
    /// 停止宝可梦动画
    /// </summary>
    public void ResetSR() => stage.ResetAnimator();

    /// <summary>
    /// 关闭HUD
    /// </summary>
    public void ClearHud() => hud.OnHide();
}
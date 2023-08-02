using System.Collections;
using UnityEngine;
public class EvoPanel : BasePanel
{
    //******************没弄好
    [SerializeField] PokemonAnimator anim;
    [SerializeField] Transform showPos;
    public IEnumerator EvolutionAnim(Pokemon pokemon)
    {
        OnOpen();
        anim.SetAnimation(pokemon, false, false, false, showPos.position);
        yield return new WaitForSeconds(1f);
        pokemon.Evolution();
        anim.ResetAnimator();
        yield return new WaitForSeconds(1f);
        anim.SetAnimation(pokemon, false, false, false, showPos.position);
        yield return new WaitForSeconds(1f);
        anim.ResetAnimator();
    }
}
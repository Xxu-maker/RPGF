using UnityEngine;
public class TestPokemon : MonoBehaviour
{
    public Material material;
    public Material defaultMaterial;
    public PokemonAnimator back;
    public PokemonAnimator front;
    public Transform b;
    public Transform f;
    public Pokemon testPokemon;
    public bool Gigantamax;
    void Start()
    {
        back.SetAnimation(testPokemon, true, testPokemon.Mega, Gigantamax, b.position);

        front.SetAnimation(testPokemon, false, testPokemon.Mega, Gigantamax, f.position);
        //Invoke("InvokeTest", 5f);
        //Invoke("O",1f);
    }
    //public void O()
    //{
    //    front.SetAnimation(testPokemon, false, testPokemon.Mega, Gigantamax, f.position);
    //}
    public void InvokeTest()
    {
        back.SkillMaterial(material);
    }
}
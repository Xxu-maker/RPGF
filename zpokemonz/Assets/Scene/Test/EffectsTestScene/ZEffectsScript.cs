using UnityEngine;
[ExecuteInEditMode]
public class ZEffectsScript : MonoBehaviour
{
    public Material mat;
    public string variable;
    public AnimationCurve anm;
    public float Mul = 1;
    public float Speed = 1;
    public bool e = false;

	private void Update()
    {
        if(mat != null)
        {
            mat.SetFloat(variable, anm.Evaluate(Time.time * Speed)*Mul);
        }
    }
}
using UnityEngine;
/// <summary>
/// 草的粒子
/// </summary>
public class GrassParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem grassParticle;
    private bool isOn;
    private void OnTriggerEnter2D()
    {
        if(!isOn)
        {
            isOn = true;
            if(grassParticle != null)
            {
                grassParticle.Play();
            }
        }
    }

    private void OnTriggerExit2D()
    {
        isOn = false;
    }
}
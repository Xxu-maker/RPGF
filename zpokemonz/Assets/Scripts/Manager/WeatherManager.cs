using UnityEngine;
public class WeatherManager : SingletonMono<WeatherManager>
{
    [SerializeField] ParticleSystem mapleLeaves;
    [SerializeField] ParticleSystem sakura;
    [SerializeField] ParticleSystem rain;
    [SerializeField] GameObject snow;
    private int n;
    public void ChangeWeather(int r)
    {
        n = r;
        switch(r)
        {
            case 1: rain.Play(); break;
            case 2: snow.SetActive(true); break;
            case 3: mapleLeaves.Play(); break;
            case 4: sakura.Play(); break;
        }
    }
    public void CloseWeather()
    {
        switch(n)
        {
            case 1: rain.Stop(); break;
            case 2: snow.SetActive(false); break;
            case 3: mapleLeaves.Stop(); break;
            case 4: sakura.Stop(); break;
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class LightManager : MonoBehaviour
{
    [Header("光照")]
    [SerializeField] Light2D Light2DCtrl;
    [Header("时间")]
    [SerializeField] Text timeText;
    [SerializeField] bool isTest;
    void Awake()
    {
        timeText.text = string.Concat(System.DateTime.Now.ToString("yyyy-MM-dd  HH:mm"),
        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.
        GetDayName(System.DateTime.Now.DayOfWeek).ToString());

        if(!isTest)
        {

        float intensityNum = 1f;
        int hh = System.DateTime.Now.Hour;
        if     (hh >= 0  && hh <= 6 )
        {
            Light2DCtrl.color = beforeDawn;
        }
        else if(hh >  6  && hh <= 8 )
        {
            Light2DCtrl.color = earlyMorning;
        }
        else if(hh >  8  && hh <= 17)
        {
            Light2DCtrl.color = Color.white;
        }
        else if(hh >  17 && hh <= 19)
        {
            intensityNum = 1.3f;
            Light2DCtrl.color = evening;
        }
        else if(hh >  19 && hh <= 23)
        {
            intensityNum = 1.7f;
            Light2DCtrl.color = night;
        }
        Light2DCtrl.intensity = intensityNum;

        }
        else
        {
            Light2DCtrl.color = Color.white;
            Light2DCtrl.intensity = 1;
        }
    }
    private void Start()
    {
        int w = UnityEngine.Random.Range(1, 20);
        switch(w)
        {
            case 1: Light2DCtrl.color = raining; Light2DCtrl.intensity = 0.8f; break;

            case 2: Light2DCtrl.color = snowing; Light2DCtrl.intensity = 0.8f; break;
            //case 3: fog.SetActive(true);
            //ColorUtility.TryParseHtmlString("#C1BA98", out nowtimeColor); break;
            //otherLight.SetActive(false);
        }
        WeatherManager.Instance.ChangeWeather(w);
    }
    private int index_test;
    public void ChangeWeather_Test()
    {
        WeatherManager.Instance.CloseWeather();
        switch(index_test)
        {
            case 0: float intensityNum = 1f;
                    int hh = System.DateTime.Now.Hour;
                    if     (hh >= 0  && hh <= 6 )
                    {
                        Light2DCtrl.color = beforeDawn;
                    }
                    else if(hh >  6  && hh <= 8 )
                    {
                        Light2DCtrl.color = earlyMorning;
                    }
                    else if(hh >  8  && hh <= 17)
                    {
                        Light2DCtrl.color = Color.white;
                    }
                    else if(hh >  17 && hh <= 19)
                    {
                        intensityNum = 1.3f;
                        Light2DCtrl.color = evening;
                    }
                    else if(hh >  19 && hh <= 23)
                    {
                        intensityNum = 1.7f;
                        Light2DCtrl.color = night;
                    }
                    Light2DCtrl.intensity = intensityNum; index_test++;
            break;

            case 1: Light2DCtrl.color = raining; Light2DCtrl.intensity = 0.8f; WeatherManager.Instance.ChangeWeather(index_test); index_test++; break;
            case 2: Light2DCtrl.color = snowing; Light2DCtrl.intensity = 0.8f; WeatherManager.Instance.ChangeWeather(index_test); index_test++; break;
            case 3: Light2DCtrl.color = Color.white; Light2DCtrl.intensity = 1f  ; WeatherManager.Instance.ChangeWeather(index_test); index_test = 0; break;
        }
    }
    //光照颜色
    public readonly Color evening      = new Color(0.93f, 0.64f, 0.28f);
    public readonly Color night        = new Color(0.19f, 0.34f, 0.47f);
    public readonly Color beforeDawn   = new Color(0.18f, 0.35f, 0.64f);
    public readonly Color earlyMorning = new Color(0.99f, 0.95f, 0.68f);
    public readonly Color raining      = new Color(0.78f, 0.78f, 0.78f);
    public readonly Color snowing      = new Color(0.44f, 0.66f, 0.97f);
}
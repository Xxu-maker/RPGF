using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 左上角地图提示
/// </summary>
public class MapTip : BasePanel
{
    [SerializeField] RectTransform trans;
    [SerializeField] Image tipBoxBackgroundImage;
    [SerializeField] Text mapNameText;
    private Coroutine mainDisplay;
    private WaitForSeconds waitForThree = new WaitForSeconds(3f);
    private void Start()
    {
        DisPlayMapTip("真新镇");
    }

    /// <summary>
    /// 左上地图名提示
    /// </summary>
    /// <param name="townName"></param>
    public void DisPlayMapTip(string townName)
    {
        if (mapNameText.text != townName)
        {
            if (mainDisplay != null)
            {
                StopCoroutine(mainDisplay);
            }
            //box.sprite = sprite;
            mapNameText.text = townName;
            mainDisplay = StartCoroutine(Display());
        }
    }

    /// <summary>
    /// tip动画
    /// </summary>
    private IEnumerator Display()
    {
        Vector3 vec = trans.anchoredPosition;
        float s = vec.y + 120f;
        Canvas.alpha = 1;
        float i = 5;
        int y = (int)(s / i);
        for(int x = 0; x < y; ++x)
        {
            vec.y -= i;
            trans.anchoredPosition = vec;
            yield return null;
        }
        vec.y = -120f;
        trans.anchoredPosition = vec;
        yield return waitForThree;

        for(byte x = 0; x < 32; ++x)
        {
            vec.y += i;
            trans.anchoredPosition = vec;
            yield return null;
        }
        Canvas.alpha = 0;
    }
}
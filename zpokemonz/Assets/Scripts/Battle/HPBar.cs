using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class HPBar : MonoBehaviour
{
    [SerializeField] Transform _trans;
    [SerializeField] Image bar;
    public void SetHP(float percent)
    {
        _trans.localScale = new Vector3(percent, 1f, 1f);
        //颜色
        bar.color = percent > 0.5f? MyData.hp_green : (percent < 0.3f? MyData.hp_red : MyData.hp_orange);
    }

    public async void SetHPSmooth(float percent)//平滑血条
    {
        float curHp = _trans.localScale.x;
        Vector3 newHp = new Vector3(curHp, 1f, 1f);
        if(curHp > percent)
        {
            float changeAmt = (curHp - percent) * Time.deltaTime;
            while (curHp > percent)// > Mathf.Epsilon
            {
                curHp -= changeAmt;
                newHp.x = curHp;
                _trans.localScale = newHp;
                await UniTask.Yield();
            }
        }
        else
        {
            float changeAmt = (percent - curHp) * Time.deltaTime;
            //回血声音
            while (curHp < percent)
            {
                curHp += changeAmt;
                newHp.x = curHp;
                _trans.localScale = newHp;
                await UniTask.Yield();
            }
        }
        newHp.x = percent;
        _trans.localScale = newHp;
        //颜色
        bar.color = percent > 0.5f? MyData.hp_green : (percent < 0.3f? MyData.hp_red : MyData.hp_orange);
    }
}
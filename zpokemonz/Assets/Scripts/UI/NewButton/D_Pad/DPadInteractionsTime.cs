using System;
using UnityEngine;
/// <summary>
/// 检查DPad按键时间(按下时转身,按住超0.1秒移动)
/// </summary>
public class DPadInteractionsTime : MonoBehaviour
{
    [NonSerialized]
    public bool outTime;
    private float tapTime;

	/// <summary>
	/// 检查是否处于tap时间
	/// </summary>
	/// <returns></returns>
    public bool IsTheTapTimeExceeded()
    {
        if(outTime) { return true; }

        if(Time.time - tapTime > 0.15f)
        {
            //超过tap计算时间
            return outTime = true;
        }
        else
        {
            return false;
        }
    }

#region action绑定
    /// <summary>
    /// 记录时间
    /// </summary>
    public void RecordDownTime()
    {
        if(!outTime)
        {
            tapTime = Time.time;
        }
    }

    /// <summary>
    /// 重置时间
    /// </summary>
    public void ResetTime()
    {
        if(outTime)
        {
            tapTime = 0f;
            outTime = false;
        }
    }
#endregion
}
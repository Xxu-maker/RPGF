using UnityEngine;
/// <summary>
/// 存档继承
/// </summary>
public class ZSavable : MonoBehaviour
{
    /// <summary>
    /// Save 存档数据
    /// </summary>
    /// <returns></returns>
    public virtual object CaptureState() { return null; }

    /// <summary>
    /// Load 读取数据
    /// </summary>
    /// <param name="state"></param>
    public virtual void RestoreState(object state) { }
}
/*public interface ISavable
{
    /// <summary>
    /// Save 存档数据
    /// </summary>
    /// <returns></returns>
    object CaptureState();
    /// <summary>
    /// Load 读取数据
    /// </summary>
    /// <param name="state"></param>
    void RestoreState(object state);
}*/

[System.Serializable]
/// <summary>
/// 存档信息(时间和存档名)
/// </summary>
public class GameMessage
{
    public string name;
    public string time;

    public GameMessage(string name, string time)
    {
        this.name = name;
        this.time = time;
    }
}
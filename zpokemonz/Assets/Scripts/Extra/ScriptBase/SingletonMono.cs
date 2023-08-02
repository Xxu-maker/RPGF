using UnityEngine;
/// <summary>
/// 继承MonoBehavior的单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T instance;
    public  static T Instance => instance;

    /// <summary>
    /// 基类Awake
    /// </summary>
    protected virtual void Awake()
    {
        if(instance != null)
        {
            print("重复");
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    public static bool IsInitialized => instance != null;//已生成

    protected virtual void OnDestroy()//销毁
    {
        if(instance == null)
        {
            instance = null;
        }
    }
}
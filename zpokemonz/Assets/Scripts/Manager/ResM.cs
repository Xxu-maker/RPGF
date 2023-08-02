using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class ResM : SingletonMono<ResM>
{
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)//如果对象是一个GameObject类型的，实例化后再返回。
        {
            return GameObject.Instantiate(res);
        }
        else //else情况示例：TextAsset、AudioClip
        {
            return res;
        }
    }

    public IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if(r.asset is GameObject)
        {
            //实例化一下再传给方法
            callback(GameObject.Instantiate(r.asset) as T);
        }
        else
        {
            //直接传给方法
            callback(r.asset as T);
        }
    }

    /// <summary>
    /// LoadAll T
    /// </summary>
    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    //如果明确知道要读图片就用这个吧

    public Sprite LoadSprite(string path)
    {
        return Resources.Load<Sprite>(path);
    }

    public Sprite[] LoadAllSprites(string path)
    {
        return Resources.LoadAll<Sprite>(path);
    }
}
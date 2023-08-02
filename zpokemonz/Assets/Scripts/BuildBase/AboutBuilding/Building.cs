using System.Collections.Generic;
using UnityEngine;
//建筑由多个层瓦片组成, 非碰撞区域, 例如顶部需要分离.
public class Building : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites;
    [SerializeField] GameObject gridSpritesObject;

    /// <summary>
    /// 开启Grid检测
    /// </summary>
    public void OpenBuildableDetection()
    {
        gridSpritesObject.SetActive(true);
    }

    /// <summary>
    /// 关掉Grid检测
    /// </summary>
    public void CloseBuildableDetection()
    {
        gridSpritesObject.SetActive(false);
    }

    /// <summary>
    /// 检查是否在可以建造的网格内
    /// </summary>
    /// <returns></returns>
    public bool IsItInBuildableGridRange()
    {
        foreach(SpriteRenderer sprite in sprites)
        {
            if(sprite.color == MyData.girdRed)
            {
                return false;
            }
        }
        //可放置就关掉检测
        CloseBuildableDetection();
        return true;
    }
}
using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建服装道具")]
public class CostumesItem : ItemBase
{
    [SerializeField] string path;
    [SerializeField] CostumesType costumesType;
    [SerializeField] Vector3 fixedPosition;
    /// <summary>
    /// 加载地址
    /// </summary>
    public string Path => path;
    public CostumesType CostumesType => costumesType;
}

//                        帽子，头发，上衣，  裤子， 鞋子,  饰品
public enum CostumesType{Cap, Hair, Blouse, Pant, Shoes, Jewelry};
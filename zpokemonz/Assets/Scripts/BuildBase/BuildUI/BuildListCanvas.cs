using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于建造Canvas和Scroll的设置和管理
/// </summary>
public class BuildListCanvas : BasePanel
{
    [SerializeField] List<BuildUIOnDrag> buildingUISlots;

    public void SetSlots(List<BuildingBase> buildings)
    {
        /*int count = buildings.Count;
        for(int i = 0; i < count; ++i)
        {
            buildingUISlots[i].SetData(buildings[i]);
        }*/
    }
}
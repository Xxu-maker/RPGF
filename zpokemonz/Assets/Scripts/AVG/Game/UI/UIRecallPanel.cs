using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecallPanel : MonoBehaviour,LoopScrollDataSource,LoopScrollPrefabSource
{
    public LoopVerticalScrollRect loopVerticalScrollRect;
    
    [Header("Prefab")]
    public GameObject DialogItemPrefab;
    public GameObject cloudItemPrefab;
    
    
    public void ProvideData(Transform transform, int idx)
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public void ReturnObject(Transform trans)
    {
        throw new System.NotImplementedException();
    }
}

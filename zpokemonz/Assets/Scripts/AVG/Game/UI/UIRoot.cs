using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoSingleton<UIRoot>
{
    public GameObject Canvas;

    public Camera UICamera;

    public Transform Trans_NovelsPoint;
    
    //重置剧情UI
    public void ResetAVGUI()
    {
        for (int i = 0; i < Trans_NovelsPoint.childCount; i++){
           Destroy(Trans_NovelsPoint.GetChild(i).gameObject);
        }
        
        //Todo 初始化剧情UI模块
        GameHelper.Alloc<GameObject>("Prefabs/UI/Novels/UINovelsPanel").transform.RestTransform(Trans_NovelsPoint);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTestzzz : MonoBehaviour
{
    [SerializeField] GameObject xPrefab;
    [SerializeField] GameObject yPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask groundMask;
    private bool start;
    private Vector3Int startVec;
    private Vector3Int endVec;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3Int vec = Vector3Int.RoundToInt(RaycastGround().Value);
            if(!start)
            {
                startVec = vec;
                //Instantiate(xPrefab, RaycastGround().Value, Quaternion.identity);
                //print($"start{vec}");
            }
            else
            {
                endVec = vec;
                //print($"end{vec}");
                Distance();
            }
            start = !start;
        }
    }
    public List<Vector3> currentRoad;
    public List<Vector3> x;
    public List<Vector3> y;
    public void Distance()
    {
        bool leftFirst = startVec.x - endVec.x < 0;
        bool downFirst = startVec.y - endVec.y < 0;
        //x从左往右
        if(leftFirst)
        {
            //oddNumber奇数
            bool oddNumber1 = Mathf.Abs(startVec.x) % 2 == 1;
            bool oddNumber2 = Mathf.Abs(endVec.x) % 2   == 1;
            //终点
            int end = oddNumber2? endVec.x + 1 : endVec.x;
            //for(int i = (oddNumber1? startVec.x - 1 : startVec.x); i < end; i += 2)
            for(int i = startVec.x; i < end; i += 2)
            {
                currentRoad.Add(new Vector3(i, startVec.y, 0));
                //x.Add(new Vector3(i, startVec.y, 0));
            }

            //y从下往上
            if(downFirst)
            {
                oddNumber1 = Mathf.Abs(startVec.y) % 2 == 1;
                oddNumber2 = Mathf.Abs(endVec.y) % 2   == 1;
                end = oddNumber2? endVec.y + 1 : endVec.y;
                //float xNum = x[x.Count - 1].x;
                float xNum = currentRoad[currentRoad.Count - 1].x;
                for(int i = startVec.y + 2; i < end; i += 2)
                {
                    currentRoad.Add(new Vector3(xNum, i, 0));
                    //y.Add(new Vector3(xNum, i, 0));
                }
            }
            foreach(Vector3 vec in currentRoad)
            {
                Instantiate(xPrefab, vec, Quaternion.identity);
                //print(vec);
            }
            /*foreach(Vector3 vec in x)
            {
                Instantiate(xPrefab, vec, Quaternion.identity);
                //print(vec);
            }
            foreach(Vector3 vec in y)
            {
                Instantiate(yPrefab, vec, Quaternion.identity);
                //print(vec);
            }*/
        }
    }

    /// <summary>
    /// 检查是否可放置
    /// </summary>
    /// <returns></returns>
    private Vector3? RaycastGround()
    {
        RaycastHit hit;//从光线投射获取信息
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }
}
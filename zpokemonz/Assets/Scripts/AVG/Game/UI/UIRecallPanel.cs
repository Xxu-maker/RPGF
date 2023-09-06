using System.Collections;
using System.Collections.Generic;
using Novels;
using UnityEngine;
using UnityEngine.UI;

public class UIRecallPanel : SingletonMono<UIRecallPanel>,LoopScrollPrefabSource, LoopScrollMultiDataSource
{
    public LoopVerticalScrollRectMulti loopVerticalScrollRect;
    
    [Header("Prefab")]
    public GameObject DialogItemPrefab;
    public GameObject SelectItemPrefab;
    
    private List<RecallConfig> RecallConfigItemDataList;
    
    #region LoopVerticalScrollRect
    
    private readonly Stack<Transform> _poolDialogItem = new();
    private readonly Stack<Transform> _poolSelectItem = new();
    
    public void Awake()
    {
        base.Awake();
        RecallConfigItemDataList = SaveManager.Instance.GetRecallConfigList;
    }
    
    public void ProvideData(Transform transform, int idx)
    {
        var data = RecallConfigItemDataList[idx];
        if (data.type == 0)
        {
            var item = transform.GetComponent<DialogItemR>();
            item.SetProvider(RecallConfigItemDataList[idx]);
        }
        else if (data.type == 1)
        {
            var item = transform.GetComponent<SelectItemR>();
            item.SetProvider(RecallConfigItemDataList[idx]);
        }
    }

    public GameObject GetObject(int index)
    {
        var data = RecallConfigItemDataList[index];
        if (data.type == (int)EContentType.Dialog)
        {
            if (_poolDialogItem.Count == 0)
            {
                return Instantiate(DialogItemPrefab);
            }

            Transform item = _poolDialogItem.Pop();
            item.gameObject.SetActive(true);
            return item.gameObject;
        }
        else if (data.type == (int)EContentType.Select)
        {
            if (_poolSelectItem.Count == 0)
            {
                return Instantiate(SelectItemPrefab);
            }

            Transform item = _poolSelectItem.Pop();
            item.gameObject.SetActive(true);
            return item.gameObject;
        }

        return null;


    }

    public void ReturnObject(Transform trans)
    {
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        if (trans.GetComponent<DialogItemR>() != null)
        {
            _poolDialogItem.Push(trans);
        }
        
        if (trans.GetComponent<SelectItemR>() != null)
        {
            _poolSelectItem.Push(trans);
        }
        
    }
    #endregion
    
    public void OnOpen()
    {
        UINovelsPanel.Instance.buttonIsolateGroup[5].gameObject.SetActive(true);
        UIRoot.Instance.Trans_RecallPoint.gameObject.SetActive(true);
        var oldSize = RecallConfigItemDataList?.Count ?? -1;
        RecallConfigItemDataList = SaveManager.Instance.GetRecallConfigList;
        InitScrollRect(true, oldSize);
    }

    public void OnClose()
    {
        UIRoot.Instance.Trans_RecallPoint.gameObject.SetActive(false);
        UINovelsPanel.Instance.buttonIsolateGroup[5].gameObject.SetActive(false);
       
    }
    
    private void InitScrollRect(bool smoothScroll = false, int oldSize = -1)
    {
        loopVerticalScrollRect.ClearCells();
        loopVerticalScrollRect.dataSource = this;
        loopVerticalScrollRect.prefabSource = this;
        loopVerticalScrollRect.totalCount = RecallConfigItemDataList.Count ;

        if (smoothScroll)
        {
            var count = RecallConfigItemDataList.Count - oldSize;
            loopVerticalScrollRect.RefillCells(count);
            RefreshScrollRectPosition();
        }
        else
        {
            loopVerticalScrollRect.RefillCells(RecallConfigItemDataList.Count-1);
            loopVerticalScrollRect.verticalNormalizedPosition = 1;
            
        }
        
    }
    
    private void RefreshScrollRectPosition()
    {
        //Todo 滑到最新的回忆
        loopVerticalScrollRect.ScrollToCellWithinTime(RecallConfigItemDataList.Count-1, 0.5f, () =>
        {
            // FIX LoopVerticalScrollRect: 修复滑动内容超过 viewport 底部问题
            var contentHeight = loopVerticalScrollRect.content.rect.height;
            var viewportHeight = loopVerticalScrollRect.GetComponent<RectTransform>().rect.height;
            if (contentHeight < viewportHeight)
            {
                loopVerticalScrollRect.MockUpdateScroll(new Vector2(0, contentHeight - viewportHeight));
            }
        });
    }
}

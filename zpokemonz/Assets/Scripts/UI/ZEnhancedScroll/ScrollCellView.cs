using System;
using UnityEngine;
using UnityEngine.UI;
namespace ZUI.ZScroller
{
    /// <summary>
    /// CellView都应该派生自它
    /// </summary>
    public class ScrollCellView : MonoBehaviour
    {
        /// <summary>
        /// gameObject.SetActive(true);
        /// </summary>
        public void Active()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// gameObject.SetActive(false);
        /// </summary>
        public void Recycle()
        {
            gameObject.SetActive(false);
        }

        [SerializeField] LayoutElement layout;
        public LayoutElement Layout => layout;

        /// <summary>
        /// 单元格索引
        /// </summary>
        [NonSerialized]
        public int cellIndex;

        /// <summary>
        /// 刷新单元格道具持有数
        /// </summary>
        public virtual void RefreshHoldNum(int holdNum) { }

        //[SerializeField] CanvasGroup canvas;
        /// <summary>
        /// 画布组开
        /// </summary>
        /*public virtual void OnOpen()//打开
        {
            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
            canvas.interactable = true;
        }
        /// <summary>
        /// 画布组关
        /// </summary>
        public virtual void OnClose()//退出
        {
            canvas.alpha = 0;
            canvas.blocksRaycasts = false;
            canvas.interactable = false;
        }*/
    }
}
using UnityEngine;
using UnityEngine.UI;
namespace ZUI.ZScroller
{
    public class Scroller : MonoBehaviour
    {
        #region Get
        [SerializeField] ScrollRect _scrollRect;
        [SerializeField] VerticalLayoutGroup _layoutGroup;
        [SerializeField] RectTransform _scrollRectTransform;
        [SerializeField] RectTransform _container;//content
        /// <summary>
        /// 用于偏移第一个可见单元格视图的布局元素
        /// </summary>
        [SerializeField] LayoutElement _firstPadder;
        /// <summary>
        /// 用于保持单元格视图正确尺寸的布局元素
        /// </summary>
        [SerializeField] LayoutElement _lastPadder;
        [SerializeField] CanvasGroup _firstPadderCG;
        [SerializeField] CanvasGroup _lastPadderCG;
        [SerializeField] ScrollCellView[] readyCellViews;
        //[SerializeField] RectTransform _recycledCellViewContainer;
        [SerializeField] float cellSize;

        /*/// <summary>
        /// 在Scroll位置之前向前看的空间量。正值(>=0)
        /// 这允许单元格在第一个可见单元格之前被加载，即使它们还没有显示。
        /// </summary>
        private float _lookAheadBefore = 100f;

        /// <summary>
        /// 在最后一个可见单元格后向前看的空间量。正值(>=0)
        /// 这允许单元格在最后一个可见单元格之前被加载，即使它们还没有显示。
        /// </summary>
        private float _lookAheadAfter = 100f;*/
        #endregion
        #region Public Load Private
        private IScrollDelegate _delegate;
        /// <summary>
        /// Delegate
        /// </summary>
        public IScrollDelegate Delegate
        {
            get { return _delegate; }
            set { _delegate = value;}
        }

        /// <summary>
        /// 这是Scroll中的单元格的数量
        /// </summary>
        private int NumberOfCells => _delegate.GetNumberOfCells();

        /// <summary>
        /// 显示在Scroll可见区域的第一个数据索引
        /// </summary>
        public int StartDataIndex => _activeCellViewsStartIndex % NumberOfCells;

        /// <summary>
        /// 显示在Scroll可见区域的最后一个数据索引
        /// </summary>
        public int EndDataIndex => _activeCellViewsEndIndex % NumberOfCells;

        //public bool IsScrolling { get; private set; }
        #endregion
        #region Private
        /// <summary>
        /// 从第一个单元格之后的间隔
        /// </summary>
        private float spacing;

        /// <summary>
        /// padding顶
        /// </summary>
        private int paddingTop;

        /// <summary>
        /// paddingTop加Bottom
        /// </summary>
        private int paddingTAB;

        /// <summary>
        /// 当前正在显示的单元格列表
        /// </summary>
        private SmallList<ScrollCellView> _activeCellViews = new SmallList<ScrollCellView>();

        /// <summary>
        /// 已被回收的单元列表
        /// </summary>
        private SmallList<ScrollCellView> _recycledCellViews = new SmallList<ScrollCellView>();

        /// <summary>
        /// 剩余的单元格 浏览之前的每个活动单元格，如果它不再属于该范围，则对其进行回收
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
        private SmallList<int> remainingCellIndices = new SmallList<int>();

        /// <summary>
        /// 单元格偏移量的内部列表。每个单元格的偏移量都是之前的偏移量的累积
        /// Reload时创建的，以加快处理速度
        /// </summary>
        private SmallList<float> _cellViewOffsetArray = new SmallList<float>();

        /// <summary>
        /// 正在显示的第一个单元格索引
        /// </summary>
        private int _activeCellViewsStartIndex;

        /// <summary>
        /// 正在显示的最后一个单元格索引
        /// </summary>
        private int _activeCellViewsEndIndex;

        /// <summary>
        /// 活动单元格content的大小减去Scroll的可见部分
        /// </summary>
        private float ScrollSize => Mathf.Max
        (
            _container.rect.height - _scrollRectTransform.rect.height, 0
        );

        /// <summary>
        /// Scroll的位置
        /// </summary>
        private float _scrollPosition;//[SerializeField]

        /// <summary>
        /// Scroll开始的位置
        /// </summary>
        private float ScrollPosition
        {
            get
            {
                return _scrollPosition;
            }
            set
            {
                // 确保位置是在当前单元格范围内
                value = Mathf.Clamp(value, 0, ScrollSize);

                // 只有当值发生变化时
                if (_scrollPosition != value)
                {
                    _scrollPosition = value;
                    //设置垂直位置
                    _scrollRect.verticalNormalizedPosition = 1f - (_scrollPosition / ScrollSize);
                }
            }
        }
        #endregion
        #region Awake
        void Awake()
        {
            _container.localRotation = Quaternion.identity;

            spacing = _layoutGroup.spacing;
            paddingTop = _layoutGroup.padding.top;
            paddingTAB = paddingTop + _layoutGroup.padding.bottom;

            int x = readyCellViews.Length;
            for(int i = 0; i < x; i++)
            {
                _recycledCellViews.Add(readyCellViews[i]);
            }
        }
        #endregion
        #region 接口
        /// <summary>
        /// 刷新单个单元格
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="hold"></param>
        public void RefreshCellView(int cellIndex, int hold)
        {
            //首位cellIndex
            int first = _activeCellViews[0].cellIndex;
            //首位到扣除的距离
            int lack = first - cellIndex;
            //0-距离拿到Index
            _activeCellViews[0 - lack].RefreshHoldNum(hold);
        }

        /// <summary>
        /// 用完CellIndex位置的道具后刷新list
        /// </summary>
        /// <param name="cellIndex"></param>
        public void RefreshList(int cellIndex)
        {
            _RecycleAllCells();
            _Resize(true);
        }

        /// <summary>
        /// 这将重置内部尺寸列表并刷新单元格
        /// </summary>
        public void ReloadData()
        {
            // 回收所有的活动单元，得到新的单元
            _RecycleAllCells();

            // 调整大小
            _Resize(false);

            // 设置垂直位置
            //_scrollRect.verticalNormalizedPosition = 1f;

            _RefreshActive();
        }

        public void OnAddListener()
        {
            _scrollRect.onValueChanged.AddListener(_ScrollRect_OnValueChanged);
        }

        public void OnRemoveListener()
        {
            _scrollRect.onValueChanged.RemoveListener(_ScrollRect_OnValueChanged);
        }
        #endregion
        #region Main
        /// <summary>
        /// 当Scroll改变数值时处理
        /// </summary>
        private void _ScrollRect_OnValueChanged(Vector2 val)
        {
            //最低限速
            float speed = _scrollRect.velocity.y;
            if(speed > 0)
            {
                //print("Down");
                if(speed < 300f)
                {
                    _scrollRect.StopMovement();
                }
            }
            else
            {
                //print("UP");
                if(speed > -300f)
                {
                    _scrollRect.StopMovement();
                }
            }

            // 设置内部滚动位置
            _scrollPosition = (1f - val.y) * ScrollSize;
            _scrollPosition = Mathf.Clamp(_scrollPosition, 0, ScrollSize);

            _RefreshActive();
        }
        #endregion
        #region 添加和回收
        /// <summary>
        /// 创建一个单元格视图，存在则回收一个
        /// </summary>
        /// <param name="cellPrefab">prefab</param>
        /// <returns></returns>
        public ScrollCellView GetCellView(ScrollCellView cellPrefab)
        {
            // 看看是否有回收的单元格
            ScrollCellView cellView = _recycledCellViews.RemoveAt(0);
            if (cellView == null)
            {
                //print("实例化");
                // 没有找到可回收的单元，就创建一个新的
                // 并放到_container
                GameObject go = Instantiate(cellPrefab.gameObject);
                cellView = go.GetComponent<ScrollCellView>();
                cellView.transform.SetParent(_container);
                //cellView.transform.localPosition = Vector3.zero;
                //cellView.transform.localRotation = Quaternion.identity;
            }
            else
            {
                //print("激活");
                // 从一个被回收的单元格中重新激活单元格
                cellView.Active();
            }
            return cellView;
        }

        /// <summary>
        /// 回收一个单元格
        /// </summary>
        /// <param name="cellView"></param>
        private void _RecycleCell(ScrollCellView cellView)
        {
            //print("回收");
            _activeCellViews.Remove(cellView);//移除activeList

            _recycledCellViews.Add(cellView);//回到recList

            //移动到回收处
            //cellView.transform.SetParent(_recycledCellViewContainer);

            // 停用单元格视图 , 这比把它移到一个新的父节点上更有效
            cellView.Recycle();

            // 重置单元格
            cellView.cellIndex = 0;
        }

        /// <summary>
        /// 创建偏移量列表
        /// 它还设置了循环触发器和位置，并初始化了单元格视图。
        /// </summary>
        /// <param name="keepPosition">如果为真，Scroll将尝试回到它在调整大小之前的位置</param>
        private void _Resize(bool keepPosition)
        {
            // 缓存原始位置
            float originalScrollPosition = _scrollPosition;

            // 计算每个单元格视图的偏移量
            _CalculateCellViewOffsets();

            // 根据单元格的数量和大小，设置content大小
            _container.sizeDelta =
                new Vector2
                (
                    _container.sizeDelta.x,
                    _cellViewOffsetArray.Last() + paddingTAB
                );

            // 创建可见的单元格
            _ResetVisibleCellViews();

            // 如果需要保持原来的位置
            if (keepPosition)
            {
                ScrollPosition = originalScrollPosition;
            }
            else
            {
                ScrollPosition = 0;
            }
        }

        /// <summary>
        /// 设置可见单元，必要时添加和回收
        /// </summary>
        private void _ResetVisibleCellViews()
        {
            int startIndex;
            int endIndex;

            // 计算可见单元格的范围
            _CalculateCurrentActiveCellRange(out startIndex, out endIndex);

            // 浏览之前的每个活动单元格，如果它不再属于该范围，则对其进行回收。
            int i = 0;
            remainingCellIndices.Clear();
            while (i < _activeCellViews.Count)
            {
                if (_activeCellViews[i].cellIndex < startIndex || _activeCellViews[i].cellIndex > endIndex)
                {
                    _RecycleCell(_activeCellViews[i]);
                }
                else
                {
                    // this cell index falls in the new range, so we add its
                    // index to the reusable list
                    // 这个单元格的索引属于新的范围，所以我们把它的索引到可重复使用的列表中
                    remainingCellIndices.Add(_activeCellViews[i].cellIndex);
                    i++;
                }
            }

            if (remainingCellIndices.Count == 0)//之前没有剩余的活动单元格
            {
                // 这个列表要么是全新的，要么我们跳到了列表中一个完全不同的部分
                // 只需添加所有新的单元格视图
                for (i = startIndex; i <= endIndex; i++)
                {
                    _AddCellView(i, false);
                }
            }
            else//有剩余的活动单元格
            {
                //首先添加之前列表中的单元格，向后移动，这样新的单元格就会被添加到前面
                for (i = endIndex; i >= startIndex; i--)
                {
                    if (i < remainingCellIndices.First())
                    {
                        _AddCellView(i, true);
                    }
                }

                // 接下来添加在前一个列表之后的单元格，继续向前并在列表的最后添加
                for (i = startIndex; i <= endIndex; i++)
                {
                    if (i > remainingCellIndices.Last())
                    {
                        _AddCellView(i, false);
                    }
                }
            }

            // 更新起点和终点索引
            _activeCellViewsStartIndex = startIndex;
            _activeCellViewsEndIndex = endIndex;

            //调整填充元素,正确偏移单元格
            _SetPadders();
        }

        /// <summary>
        /// 计算每个单元格的偏移量，累积以前单元格的值
        /// </summary>
        private void _CalculateCellViewOffsets()
        {
            //背包里物品个数
            int count = NumberOfCells;
            //偏移量List长度
            int offsetCount = _cellViewOffsetArray.Count;
            //如果背包里个数比List长
            if(count > offsetCount)
            {
                float offset = cellSize;
                float oas = cellSize + spacing;
                if(offsetCount == 0)
                {
                    for(int i = 0; i < count; i++)
                    {
                        _cellViewOffsetArray.Add(offset);
                        offset += oas;
                    }
                }
                else
                {
                    offset = _cellViewOffsetArray.Last();
                    for(int i = offsetCount; i < count; i++)
                    {
                        offset += oas;
                        _cellViewOffsetArray.Add(offset);
                    }
                }
            }
            else if(count < offsetCount)
            {
                //数据数量小于offset的数量
                _cellViewOffsetArray.AdjustToACount(count);
            }
        }

        /// <summary>
        /// 创建一个单元格，或者循环使用
        /// </summary>
        /// <param name="cellIndex">单元格Index</param>
        /// <param name="first">true将该单元格添加到开头 false为结尾</param>
        private void _AddCellView(int cellIndex, bool first)
        {
            if (NumberOfCells == 0)
            {
                return;
            }
            //print("GET");

            // 从委托拿一个单元格
            ScrollCellView cellView = _delegate.GetCellView(this, cellIndex);

            // 设置单元格的属性
            cellView.cellIndex = cellIndex;

            // 将单元格视图添加到活动容器中
            cellView.transform.SetParent(_container, false);
            cellView.transform.localScale = Vector3.one;

            // 设置布局元素的大小
            cellView.Layout.minHeight = 100f;
            //(cellIndex == 0? 100f : 110f);// - (cellIndex > 0 ? _layoutGroup.spacing : 0);

            // 将该单元格添加到活动列表中
            if (first)
            {
                _activeCellViews.AddStart(cellView);
                // 设置单元格视图在容器中的层次结构位置
                cellView.transform.SetSiblingIndex(1);
            }
            else
            {
                _activeCellViews.Add(cellView);
                // 设置单元格视图在容器中的层次结构位置
                cellView.transform.SetSiblingIndex(_container.childCount - 2);
            }
        }

        /// <summary>
        /// 回收所有单元格
        /// </summary>
        private void _RecycleAllCells()
        {
            while (_activeCellViews.Count > 0)
            {
                _RecycleCell(_activeCellViews[0]);
            }
            _activeCellViewsStartIndex = 0;
            _activeCellViewsEndIndex = 0;
        }
        #endregion
        #region Tool
        /// <summary>
        /// 这个函数调整两个padders，控制第一个单元格的偏移和每个单元格的整体尺寸
        /// </summary>
        private void _SetPadders()
        {
            if (NumberOfCells == 0)
            {
                return;
            }

            // 计算垫子的大小 设置第一个垫子并切换其可见性
            _firstPadder.minHeight = _cellViewOffsetArray[_activeCellViewsStartIndex] - (_activeCellViewsStartIndex == 0? 100f : 110f);
            ShowOrHide(ref _firstPadderCG, _firstPadder.minHeight > 0);

            // 计算垫子的大小 设置最后一个垫子并切换其可见性
            _lastPadder.minHeight = _cellViewOffsetArray.Last() - _cellViewOffsetArray[_activeCellViewsEndIndex];
            ShowOrHide(ref _lastPadderCG, _lastPadder.minHeight > 0);
        }

        /// <summary>
        /// 如果滚动就会被调用，更新活动单元格的列表
        /// </summary>
        private void _RefreshActive()
        {
            int startIndex;
            int endIndex;

            // 得到可见的单元范围
            _CalculateCurrentActiveCellRange(out startIndex, out endIndex);

            // 如果索引没有改变则返回
            if (startIndex == _activeCellViewsStartIndex && endIndex == _activeCellViewsEndIndex)
            {
                return;
            }

            // 重现可见的单元
            _ResetVisibleCellViews();
        }

        /// <summary>
        /// 决定了哪些单元可以被看到
        /// </summary>
        /// <param name="startIndex">第一个可见单元格的索引</param>
        /// <param name="endIndex">最后一个可见单元格的索引</param>
        private void _CalculateCurrentActiveCellRange(out int startIndex, out int endIndex)
        {
            startIndex = 0;
            endIndex = 0;

            // 获得滚动器的位置
            float startPosition = _scrollPosition;// - _lookAheadBefore;
            float endPosition = _scrollPosition + _scrollRectTransform.rect.height;// + _lookAheadAfter;

            // 根据位置来计算索引
            startIndex = GetCellViewIndexAtPosition(startPosition);
            endIndex = GetCellViewIndexAtPosition(endPosition);
        }

        /// <summary>
        /// 获取指定位置上的单元格索引
        /// </summary>
        /// <param name="position">从滚动开始的像素偏移量</param>
        /// <returns></returns>
        public int GetCellViewIndexAtPosition(float position)
        {
            // 在列表的整个范围内调用重载方法
            return _GetCellIndexAtPosition(position, 0, _cellViewOffsetArray.Count - 1);
        }

        /// <summary>
        /// 根据子集范围，获取给定位置的单元格索引
        /// 使用递归二进制排序来快速找到索引
        /// </summary>
        /// <param name="position">从Scroll开始的像素偏移量</param>
        /// <param name="startIndex">范围的第一个索引</param>
        /// <param name="endIndex">范围的最后一个索引</param>
        /// <returns></returns>
        private int _GetCellIndexAtPosition(float position, int startIndex, int endIndex)
        {
            // 如果范围无效，返回起始索引
            if (startIndex >= endIndex)
            {
                return startIndex;
            }

            // 确定我们二进制搜索的中间点
            int middleIndex = (startIndex + endIndex) / 2;

            // 如果中间索引大于位置，则搜索二叉树的后半部分，否则搜索前一半
            int pad = paddingTop;
            if ((_cellViewOffsetArray[middleIndex] + pad) >= (position + (pad == 0 ? 0 : 1.00001f)))
            {
                return _GetCellIndexAtPosition(position, startIndex, middleIndex);
            }
            else
            {
                return _GetCellIndexAtPosition(position, middleIndex + 1, endIndex);
            }
        }

        private void ShowOrHide(ref CanvasGroup canvas, bool open)
        {
            if(open)
            {
                canvas.alpha = 1;
                canvas.interactable = true;
                canvas.blocksRaycasts = true;
            }
            else
            {
                canvas.alpha = 0;
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
            }
        }
        #endregion

        //暂时不需要
        #region LateUpdate控制最大速度
		/*/// <summary>
        /// 滚动器可以达到的最大速度。
		/// 这对于消除用户积极的滚动。它还可以用来减轻
		/// Unity在ScrollRect中拖动时增加的巨大惯性速度。
		/// 和在列表边缘附近的循环（参见loopWhileDragging）。
        /// </summary>
		public float maxVelocity;

        /// <summary>
        /// Fired at the end of the frame.
        /// </summary>
        void LateUpdate()
        {
			// 如果maxVelocity不为零，我们可以根据滚动方向设置速度上限
			if (maxVelocity > 0)
			{
				Velocity = INewVector.GetNewVector2
                (
                    Velocity.x, Mathf.Clamp(Mathf.Abs(Velocity.y), 0, maxVelocity) * Mathf.Sign(Velocity.y)
                );
			}
        }*/
        #endregion
        #region Clear
        /*/// <summary>
        /// Removes all cells, both active and recycled from the scroller.
        /// This will call garbage collection.
        /// </summary>
        public void ClearAll()
        {
            ClearActive();
            ClearRecycled();
        }

        /// <summary>
        /// Removes all the active cell views. This should only be used if you want
        /// to get rid of cells because of settings set by Unity that cannot be
        /// changed at runtime. This will call garbage collection.
        /// </summary>
        public void ClearActive()
        {
            for (var i = 0; i < _activeCellViews.Count; i++)
            {
                DestroyImmediate(_activeCellViews[i].gameObject);
            }
            _activeCellViews.Clear();
        }

        /// <summary>
        /// Removes all the recycled cell views. This should only be used after you
        /// load in a completely different set of cell views that will not use the
        /// recycled views. This will call garbage collection.
        /// </summary>
        public void ClearRecycled()
        {
            for (var i = 0; i < _recycledCellViews.Count; i++)
            {
                DestroyImmediate(_recycledCellViews[i].gameObject);
            }
            _recycledCellViews.Clear();
        }*/
        #endregion
        #region Other
        /*/// <summary>
        /// 根据dataIndex获取从滚动器开始的滚动位置
        /// </summary>
        /// <param name="dataIndex">要查找的dataIndex</param>
        /// <param name="up">true为上 false为下</param>
        /// <returns></returns>
        public float GetScrollPositionForDataIndex(int dataIndex, bool up)
        {
            return GetScrollPositionForCellViewIndex(dataIndex, up);
        }*/

        /*/// <summary>
        /// 这将调用每个活动单元格的RefreshCellView方法。
        /// 如果你在你的单元格中覆盖了RefreshCellView方法
        /// 那么你就可以更新用户界面而不需要重新加载数据。
        /// 注意：这不会改变单元格的大小，你需要调用ReloadData来实现。
        /// </summary>
        public void RefreshActiveCellViews()
        {
            int c = _activeCellViews.Count;
            for (int i = 0; i < c; i++)
            {
                _activeCellViews[i].RefreshCellView();
            }
        }

        /// <summary>
        /// 根据cellViewIndex获取从Scroll开始的滚动位置
        /// </summary>
        /// <param name="cellViewIndex"></param>
        /// <param name="up">true为上 false为下</param>
        /// <returns></returns>
        public float GetScrollPositionForCellViewIndex(int cellViewIndex, bool up)
        {
            if (NumberOfCells == 0)
            {
                return 0;
            }
            if (cellViewIndex < 0)
            {
                cellViewIndex = 0;
            }

            if (cellViewIndex == 0 && up)
            {
                return 0;
            }
            else
            {
                if (cellViewIndex < _cellViewOffsetArray.Count)
                {
                    // the index is in the range of cell view offsets
                    // 索引在单元格视图的偏移范围内

                    if (up)
                    {
                        // return the previous cell view's offset + the spacing between cell views
                        // 返回前一个单元格视图的偏移量+单元格视图之间的间距
                        return _cellViewOffsetArray[cellViewIndex - 1] + spacing + padding.top;
                    }
                    else
                    {
                        // return the offset of the cell view (offset is after the cell)
                        // 返回单元格视图的偏移量（偏移量在单元格之后）。
                        return _cellViewOffsetArray[cellViewIndex] + padding.top;
                    }
                }
                else
                {
                    // get the start position of the last cell (the offset of the second to last cell)
                    // 获取最后一个单元格的起始位置（倒数第二个单元格的偏移量）
                    return _cellViewOffsetArray[_cellViewOffsetArray.Count - 2];
                }
            }
        }*/
        #endregion
    }
}
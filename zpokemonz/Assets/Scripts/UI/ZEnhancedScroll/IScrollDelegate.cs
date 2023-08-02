namespace ZUI.ZScroller
{
    /// <summary>
    /// Scroll回调脚本继承接口
    /// </summary>
    public interface IScrollDelegate
    {
        /// <summary>
        /// 数据列表个数
        /// </summary>
        /// <returns></returns>
        int GetNumberOfCells();

        /// <summary>
        /// 获取dataIndex应使用的CellView。应该向Scroller请求一个新的单元格
        /// 以便它能够正确地回收旧单元格。
        /// </summary>
        ScrollCellView GetCellView(Scroller scroller, int cellIndex);
    }
}
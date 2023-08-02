using UnityEngine;
/// <summary>
/// 这是一个超级轻量级的数组的实现
/// 它的行为就像一个列表，在需要时自动分配新的内存，但不释放给垃圾回收
/// </summary>
public class SmallList<T>
{
    /// <summary>
    /// 数据
    /// </summary>
    protected T[] data;

    /// <summary>
    /// 列表中元素的数量
    /// </summary>
    public int Count = 0;

    /// <summary>
    /// 对列表项的索引访问
    /// </summary>
    public T this[int i]
    {
        get
        {
            if (data is null)
            {
                return default(T);
            }
            else
            {
                return data[i];
            }
        }
        set { data[i] = value; }
    }

    /// <summary>
    /// 当需要更多的内存时，调整大小。
    /// </summary>
    private void ResizeArray()
    {
        T[] newData;
        if (data != null)
        {
            newData = new T[Mathf.Max(data.Length << 1, 64)];
        }
        else
        {
            newData = new T[64];
        }
        if (data != null && Count > 0)
        {
            data.CopyTo(newData, 0);
        }
        data = newData;
    }

    /// <summary>
    /// 没有将内存释放给垃圾回收
    /// 列表的大小被设置为零
    /// </summary>
    public void Clear()
    {
        Count = 0;
    }

    /// <summary>
    /// 返回列表中的第一个元素
    /// </summary>
    public T First()
    {
        if (data == null || Count == 0)
        {
            return default(T);
        }
        return data[0];
    }

    /// <summary>
    /// 返回列表中的最后一个元素
    /// </summary>
    public T Last()
    {
        if (data == null || Count == 0)
        {
            return default(T);
        }
        return data[Count - 1];
    }

    /// <summary>
    /// 在数组中添加一个新的元素，如果有必要的话，创建更多的内存
    /// </summary>
    public void Add(T item)
    {
        if (data == null || Count == data.Length)
        {
            ResizeArray();
        }
        data[Count] = item;
        Count++;
    }

    /// <summary>
    /// 在数组的开始添加一个新元素，如果有必要的话，创建更多的内存
    /// </summary>
    public void AddStart(T item)
    {
        Insert(item, 0);
    }

    /// <summary>
    /// 在指定的索引处向数组中插入一个新元素，如果有必要的话，创建更多的内存
    /// </summary>
    public void Insert(T item, int index)
    {
        if (data == null || Count == data.Length)
        {
            ResizeArray();
        }
        for (var i = Count; i > index; i--)
        {
            data[i] = data[i - 1];
        }
        data[index] = item;
        Count++;
    }

    /// <summary>
    /// 删除第一个元素
    /// </summary>
    public T RemoveStart()
    {
        return RemoveAt(0);
    }

    /// <summary>
    /// 索引删除
    /// </summary>
    public T RemoveAt(int index)
    {
        if (data != null && Count != 0)
        {
            T val = data[index];
            for (var i = index; i < Count - 1; i++)
            {
                data[i] = data[i + 1];
            }
            Count--;
            data[Count] = default(T);
            return val;
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// 从数据中删除一个项目
    /// </summary>
    public T Remove(T item)
    {
        if (data != null && Count != 0)
        {
            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(item))
                {
                    return RemoveAt(i);
                }
            }
        }
        return default(T);
    }

    /// <summary>
    /// 删除末尾元素
    /// </summary>
    public T RemoveEnd()
    {
        if (data != null && Count != 0)
        {
            Count--;
            T val = data[Count];
            data[Count] = default(T);
            return val;
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// 调整到Value长度(同值调整)
    /// </summary>
    public void AdjustToACount(int value)
    {
        Count = value;
    }

    /// <summary>
    /// 判断数据是否包含该项目
    /// </summary>
    public bool Contains(T item)
    {
        if (data == null)
        {
            return false;
        }
        for (var i = 0; i < Count; i++)
        {
            if (data[i].Equals(item))
            {
                return true;
            }
        }
        return false;
    }
}
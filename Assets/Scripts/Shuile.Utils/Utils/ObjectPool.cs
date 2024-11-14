using System;
using System.Collections.Generic;

public interface IPooledObject
{
    void GetFromPool();
    void ReleaseFromPool();
    void DestroyFromPool();
}

public class ClassedObjectPool<T> : ObjectPool<T> where T : class, IPooledObject
{
    public ClassedObjectPool(Func<T> createFunc, int capacity = 8) : base(createFunc, capacity: capacity)
    {
        getFunc = static x => x.GetFromPool();
        releaseFunc = static x => x.ReleaseFromPool();
        destroyFunc = static x => x.DestroyFromPool();
    }
}

/// <summary>
///     Better than <see cref="UnityEngine.Pool.ObjectPool{T}" />
/// </summary>
/// <typeparam name="T">Element Type</typeparam>
public class ObjectPool<T> where T : class
{
    protected readonly Func<T> createFunc;
    protected Action<T> destroyFunc;
    protected Action<T> getFunc;
    protected Action<T> releaseFunc;

    // index 0 ~ CountActive - 1 are active, CountActive ~ Count - 1 are inactive
    private readonly List<T> objects;

    public ObjectPool(
        Func<T> createFunc,
        Action<T> getFunc = null,
        Action<T> releaseFunc = null,
        Action<T> destroyFunc = null,
        int capacity = 8)
    {
        this.createFunc = createFunc;
        this.getFunc = getFunc;
        this.releaseFunc = releaseFunc;
        this.destroyFunc = destroyFunc;
        objects = new List<T>(capacity);
    }

    public int Count => objects.Count;
    public int CountActive { get; private set; }
    public int CountInactive => Count - CountActive;
    public int Capacity => objects.Capacity;

    /// <summary>
    ///     获取一个元素
    /// </summary>
    /// <returns>回收一个元素</returns>
    public T Get()
    {
        T obj;
        if (CountInactive == 0) // 没有了，开一个
        {
            obj = createFunc();
            objects.Add(obj);
        }
        else // 用之前剩下的
        {
            obj = objects[CountActive];
        }

        CountActive++;

        getFunc?.Invoke(obj);
        return obj;
    }

    /// <summary>
    ///     释放指定元素
    /// </summary>
    /// <param name="element">待释放的元素</param>
    public void Release(T element)
    {
        var index = objects.IndexOf(element);
        if (index == -1 || index >= CountActive) // -1意味着不是这个对象池管理的，index >= CountActive意味着已经被release了
        {
            return;
        }

        releaseFunc?.Invoke(element);
        if (--CountActive == 0) // 没了，不用交换了
        {
            return;
        }

        // 把最后一个活跃的元素移动到前面，防止空洞
        objects[index] = objects[CountActive];
        objects[CountActive] = element;
    }

    /// <summary>
    ///     枚举所有激活的物体(可以调用Release)
    /// </summary>
    public IEnumerable<T> EnumerateActive()
    {
        for (var i = CountActive - 1; i >= 0; i--)
        {
            yield return objects[i];
        }
    }

    public IEnumerable<PoolObject> EnumerateAll()
    {
        for (var i = Count - 1; i >= 0; i--)
        {
            yield return new PoolObject
            {
                Obj = objects[i],
                IsActive = i < CountActive,
                Pool = this
            };
        }
    }

    public struct PoolObject
    {
        public T Obj;
        public bool IsActive;
        public ObjectPool<T> Pool;
    }

    /// <summary>
    ///     销毁所有未使用的元素
    /// </summary>
    public void DestroyUnused()
    {
        for (var i = Count - 1; i >= CountActive; i--)
        {
            destroyFunc(objects[i]);
            objects[i] = null;
        }
    }

    /// <summary>
    ///     销毁所有元素
    /// </summary>
    public void DestroyAll()
    {
        for (var i = Count - 1; i >= 0; i--)
        {
            destroyFunc(objects[i]);
            objects[i] = null;
        }
    }
}

public static class ObjectPoolExtension
{
    public static void ReleaseAll<T>(this ObjectPool<T> pool) where T : class
    {
        foreach (var element in pool.EnumerateActive())
        {
            pool.Release(element);
        }
    }
}

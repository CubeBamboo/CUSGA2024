using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSharpLazySingletons<T> where T : new()
{
    private static readonly object _lock = new object();
    protected static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
                lock (_lock)
                {
                    instance ??= new T();
                }

            return instance;
        }
        set { instance = value; }
    }
}

public class CSharpHungrySingletons<T> where T : new()
{
    public static T Instance { get; } = new T();
}

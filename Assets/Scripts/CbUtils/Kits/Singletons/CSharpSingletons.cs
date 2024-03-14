using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CSharpSingletons<T> where T : new()
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
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }

            return instance;
        }
        set { instance = value; }
    }
}

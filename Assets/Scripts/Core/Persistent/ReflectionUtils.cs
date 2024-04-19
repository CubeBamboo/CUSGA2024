using System;
using System.Reflection;

using UnityEngine;

namespace Shuile.Persistent
{
    public delegate bool PropertyWalkAction(object instance, PropertyInfo property, string path);

    public static class ReflectionUtils
    {
        public static void WalkProperty(object obj, PropertyWalkAction call, string parent = "")
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var prop in properties)
            {
                var path = parent + (parent == string.Empty ? "" : ".") + prop.Name;
                if (call(obj, prop, path))
                    WalkProperty(obj != null ? prop.GetValue(obj) : null, call, path);
            }
        }

        public static object CreateObject(Type type)
        {
            var ctor = type.GetConstructor(Array.Empty<Type>());  // ¡„≤Œππ‘Ï∆˜
            if (ctor == null)
                throw new InvalidOperationException($"The type {type.FullName} without a parameterless constructor");

            return ctor.Invoke(null);
        }
    }
}
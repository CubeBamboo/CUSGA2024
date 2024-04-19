using Cysharp.Threading.Tasks;

using System;
using System.Reflection;

using UnityEngine;

namespace Shuile.Persistent
{
    public class PlayerPrefsAccessor<T> : IAccessor<T> where T : PersistentData<T>, new()
    {
        private readonly string prefix;

        public PlayerPrefsAccessor(string location)
        {
            prefix = location + ":" ?? string.Empty;
        }

        public bool IsRandomRWSupported => true;

        public UniTask<T> LoadAsync()
        {
            var result = PersistentData<T>.Default;
            ReflectionUtils.WalkProperty(result, Load, string.Empty);
            return UniTask.FromResult(result);

            bool Load(object instance, PropertyInfo property, string path)
            {
                path = prefix + path;
                if (!PlayerPrefs.HasKey(path))
                    return false;

                if (property.PropertyType == typeof(string))
                    property.SetValue(instance, PlayerPrefs.GetString(path));
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(instance, PlayerPrefs.GetInt(path) != 0);
                else if (property.PropertyType == typeof(char))
                    property.SetValue(instance, (char)PlayerPrefs.GetInt(path));
                else if (property.PropertyType == typeof(byte))
                    property.SetValue(instance, (byte)PlayerPrefs.GetInt(path));
                else if (property.PropertyType == typeof(short))
                    property.SetValue(instance, (short)PlayerPrefs.GetInt(path));
                else if (property.PropertyType == typeof(ushort))
                    property.SetValue(instance, (ushort)PlayerPrefs.GetInt(path));
                else if (property.PropertyType == typeof(int))
                    property.SetValue(instance, (int)PlayerPrefs.GetInt(path));
                else if (property.PropertyType == typeof(uint))
                    property.SetValue(instance, uint.Parse(PlayerPrefs.GetString(path)));
                else if (property.PropertyType == typeof(long))
                    property.SetValue(instance, long.Parse(PlayerPrefs.GetString(path)));
                else if (property.PropertyType == typeof(ulong))
                    property.SetValue(instance, ulong.Parse(PlayerPrefs.GetString(path)));
                else if (property.PropertyType == typeof(float))
                    property.SetValue(instance, PlayerPrefs.GetFloat(path));
                else if (property.PropertyType == typeof(double))
                    property.SetValue(instance, double.Parse(PlayerPrefs.GetString(path)));
                else if (property.PropertyType == typeof(DateTime))
                    property.SetValue(instance, DateTime.Parse(PlayerPrefs.GetString(path)));
                else
                {
                    if (PlayerPrefs.GetInt(path) == 0)
                    {
                        if (instance != null)
                            property.SetValue(instance, null);
                    }
                    else  // property not null
                    {
                        if (instance == null)
                            throw new NullReferenceException($"Deserializer walked into an impossible reached area, please report to developer");

                        if (property.GetValue(instance) == null)
                            property.SetValue(instance, ReflectionUtils.CreateObject(property.PropertyType));
                        return true;
                    }
                }
                return false;
            }
        }

        public UniTask SaveAsync(T data)
        {
            return SaveAsync("", data);
        }

        public UniTask SaveAsync<TProp>(string path, TProp propertyValue)
        {
            var pathWithPrefix = prefix + path;
            int dotIndex = 0;
            while ((dotIndex = pathWithPrefix.IndexOf('.', dotIndex + 1)) != -1)
                PlayerPrefs.SetInt(pathWithPrefix[0..dotIndex], 1);

            if (SaveColumn(pathWithPrefix, propertyValue))
                ReflectionUtils.WalkProperty(propertyValue, SaveObjectColumn, path);

            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        private bool SaveObjectColumn(object instance, PropertyInfo property, string path)
            => SaveColumn<object>(prefix + path, property.GetValue(instance));

        private bool SaveColumn<TProp>(string path, TProp propertyValue)
        {
            if (propertyValue is string str)
                PlayerPrefs.SetString(path, str);
            else if (propertyValue is bool @bool)
                PlayerPrefs.SetInt(path, @bool ? 1 : 0);
            else if (propertyValue is char @char)
                PlayerPrefs.SetInt(path, @char);
            else if (propertyValue is byte @byte)
                PlayerPrefs.SetInt(path, @byte);
            else if (propertyValue is short @short)
                PlayerPrefs.SetInt(path, @short);
            else if (propertyValue is ushort @ushort)
                PlayerPrefs.SetInt(path, @ushort);
            else if (propertyValue is int @int)
                PlayerPrefs.SetInt(path, @int);
            else if (propertyValue is uint @uint)
                PlayerPrefs.SetString(path, @uint.ToString());
            else if (propertyValue is long @long)
                PlayerPrefs.SetString(path, @long.ToString());
            else if (propertyValue is ulong @ulong)
                PlayerPrefs.SetString(path, @ulong.ToString());
            else if (propertyValue is float @float)
                PlayerPrefs.SetFloat(path, @float);
            else if (propertyValue is double @double)
                PlayerPrefs.SetString(path, @double.ToString());
            else if (propertyValue is DateTime dt)
                PlayerPrefs.SetString(path, dt.ToString());
            else
            {
                PlayerPrefs.SetInt(path, propertyValue == null ? 0 : 1);
                return propertyValue != null;
            }
            return false;
        }
    }
}
using System.Collections.Generic;

namespace CbUtils
{
    // only for debug, clear all outer references before build
    public class DebugProperty : CSharpLazySingletons<DebugProperty>
    {
        private readonly Dictionary<string, float> floatProps = new();
        private readonly Dictionary<string, int> intProps = new();
        private readonly Dictionary<string, string> stringProps = new();

        public void SetInt(string name, int value)
        {
            if (!intProps.ContainsKey(name))
            {
                intProps.Add(name, value);
            }
            else
            {
                intProps[name] = value;
            }
        }

        public int GetInt(string name)
        {
            if (!intProps.ContainsKey(name))
            {
                intProps.Add(name, default);
            }

            return intProps[name];
        }

        public void SetFloat(string name, float value)
        {
            if (!floatProps.ContainsKey(name))
            {
                floatProps.Add(name, value);
            }
            else
            {
                floatProps[name] = value;
            }
        }

        public float GetFloat(string name)
        {
            if (!floatProps.ContainsKey(name))
            {
                floatProps.Add(name, default);
            }

            return floatProps[name];
        }

        public void SetString(string name, string value)
        {
            if (!stringProps.ContainsKey(name))
            {
                stringProps.Add(name, value);
            }
            else
            {
                stringProps[name] = value;
            }
        }

        public string GetString(string name)
        {
            if (!stringProps.ContainsKey(name))
            {
                stringProps.Add(name, default);
            }

            return stringProps[name];
        }
    }
}

using System;
using System.Reflection;

namespace Myth
{
    public class ReflectionHelper
    {
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            var bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                              BindingFlags.NonPublic;
            return type.GetField(fieldName, bindingAttr)?.GetValue(instance);
        }
    }
}
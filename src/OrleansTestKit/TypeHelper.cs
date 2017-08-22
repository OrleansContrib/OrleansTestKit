using System;
using System.Reflection;

namespace Orleans.TestKit
{
    public static class TypeHelper
    {
        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="baseType">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck != null && toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return true;
                }

                toCheck = toCheck?.BaseType;
            }

            return false;
        }

        public static PropertyInfo GetProperty(Type t, string name)
        {
            var info = t.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);

            if (info == null && t.BaseType != null)
                return GetProperty(t.BaseType, name);

            return info;
        }
    }
}
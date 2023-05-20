namespace Orleans.TestKit;

public static class TypeHelper
{
    /// <summary>
    ///     Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types
    ///     without any type parameters).
    /// </summary>
    /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
    /// <param name="baseType">The base type class for which the check is made.</param>
    /// <returns>Result of the check.</returns>
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
    {
        if (toCheck == null)
        {
            throw new ArgumentNullException(nameof(toCheck));
        }

        if (baseType == null)
        {
            throw new ArgumentNullException(nameof(baseType));
        }

        while (toCheck != typeof(object))
        {
            var cur = toCheck != null && toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (baseType == cur)
            {
                return true;
            }

            toCheck = toCheck?.BaseType!;
        }

        return false;
    }
}

using System;
using System.Collections.Generic;

namespace SUF.Common.GeneralPurpose
{
    internal static class TypeExtentions
    {
        public static IEnumerable<Type> GetParents(this Type childType)
        {
            var current = childType;
            while ((current = current.BaseType) != null)
            {
                yield return current;
            }
        }
    }
}
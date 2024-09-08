using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Furkan.Common
{
    public static class ObjectExtensions
    {
        // TODO: Why does NotNullWhen(true) not work
        /// Safe <c>is</c> operator with Unity lifetime check
        public static bool Is<T>(this Object self, [MaybeNull] out T target) where T : Object
        {
            target = (bool)self ? self as T : null;
            return (bool)target;
        }

        /// Safe <c>is not</c> operator with Unity lifetime check
        public static bool IsNot<T>(this Object self, [MaybeNull] out T target) where T : Object =>
            !self.Is(out target);

        /// Safe <c>is</c> operator with Unity lifetime check
        public static bool Is<T, TInterface>(this TInterface self, [MaybeNull] out T target) where T : Object
        {
            target = (bool)(self as Object) ? self as T : null;
            return (bool)target;
        }

        /// Safe <c>is not</c> operator with Unity lifetime check
        public static bool IsNot<T, TInterface>(this TInterface self, [MaybeNull] out T target) where T : Object =>
            !self.Is(out target);
    }
}

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Furkan.Common
{
    public static class ObjectExtensions
    {
        // TODO: Why does NotNullWhen(true) not work

        /// <summary>
        /// Safe <c>is</c> operator with Unity lifetime check
        /// </summary>
        public static bool Is<TObject>(this Object self, [MaybeNull] out TObject target) where TObject : Object
        {
            target = (bool)self ? self as TObject : null;
            return (bool)target;
        }

        /// <summary>
        /// Safe <c>is not</c> operator with Unity lifetime check
        /// </summary>
        public static bool IsNot<TObject>(this Object self, [MaybeNull] out TObject target) where TObject : Object =>
            !self.Is(out target);

        /// <summary>
        /// Safe <c>is</c> operator with Unity lifetime check
        /// </summary>
        public static bool Is<TInterface, TObject>(this TInterface self, [MaybeNull] out TObject target)
            where TInterface : class
            where TObject : Object
        {
            target = self.IsAlive() ? self as TObject : null;
            return (bool)target;
        }

        /// <summary>
        /// Safe <c>is not</c> operator with Unity lifetime check
        /// </summary>
        public static bool IsNot<TInterface, TObject>(this TInterface self, [MaybeNull] out TObject target)
            where TInterface : class
            where TObject : Object =>
            !self.Is(out target);

        /// <summary>
        /// Unity lifetime check for interfaces (<c>self != null</c>)
        /// </summary>
        public static bool IsAlive<TInterface>(this TInterface self) where TInterface : class =>
            (bool)(self as Object);

        /// <summary>
        /// Unity lifetime check for interfaces (<c>self == null</c>)
        /// </summary>
        public static bool IsNull<TInterface>(this TInterface self) where TInterface : class =>
            !IsAlive(self);
    }
}

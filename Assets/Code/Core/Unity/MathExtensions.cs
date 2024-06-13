using UnityEngine;

namespace Tulip.Core.Unity
{
    public static class MathExtensions
    {
        /// Exponential decay function from Freya Holmér.
        public static float ExpDecay(this float from, float to, float decay, float dt) =>
            to + ((from - to) * Mathf.Exp(-decay * dt));

        /// Exponential decay function from Freya Holmér.
        public static Vector2 ExpDecay(this Vector2 from, Vector2 to, float decay, float dt) =>
            to + ((from - to) * Mathf.Exp(-decay * dt));

        /// Exponential decay function from Freya Holmér.
        public static Vector3 ExpDecay(this Vector3 from, Vector3 to, float decay, float dt) =>
            to + ((from - to) * Mathf.Exp(-decay * dt));
    }
}

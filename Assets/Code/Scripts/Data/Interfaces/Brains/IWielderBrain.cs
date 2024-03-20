using UnityEngine;

namespace Tulip.Data
{
    public interface IWielderBrain
    {
        public Vector3 AimPosition { get; }
        public bool WantsToUse { get; }
    }
}

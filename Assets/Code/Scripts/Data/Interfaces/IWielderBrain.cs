using UnityEngine;

namespace Tulip.Data
{
    public interface IWielderBrain : ICharacterBrain
    {
        public Vector3 FocusPosition { get; }
        public bool IsUseInProgress { get; }
    }
}

using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    public abstract class WielderBrain : CharacterBrain, IWielderBrain
    {
        public abstract Vector3 FocusPosition { get; protected set; }
        public bool IsUseInProgress { get; protected set; }
    }
}

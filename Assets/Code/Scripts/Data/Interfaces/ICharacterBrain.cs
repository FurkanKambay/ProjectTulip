using System;

namespace Tulip.Data
{
    public interface ICharacterBrain
    {
        public event Action<float> OnMoveLateral;
        public event Action OnJump;
        public event Action OnJumpReleased;
    }
}

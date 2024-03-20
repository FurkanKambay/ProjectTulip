using System;

namespace Tulip.Data
{
    public interface IJumperBrain
    {
        public event Action OnJump;
        public event Action OnJumpReleased;
    }
}

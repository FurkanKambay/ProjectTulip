using System;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    public abstract class CharacterBrain : MonoBehaviour, ICharacterBrain
    {
        public event Action<float> OnMoveLateral;
        public event Action OnJump;
        public event Action OnJumpReleased;

        protected virtual void RaiseOnMoveLateral(float value) => OnMoveLateral?.Invoke(value);
        protected virtual void RaiseOnJump() => OnJump?.Invoke();
        protected virtual void RaiseOnJumpReleased() => OnJumpReleased?.Invoke();
    }
}

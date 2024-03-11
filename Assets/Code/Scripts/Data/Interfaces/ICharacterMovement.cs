using UnityEngine;

namespace Tulip.Data
{
    public interface ICharacterMovement
    {
        Vector2 DesiredVelocity { get; }
        Vector2 Velocity { get; }
    }
}

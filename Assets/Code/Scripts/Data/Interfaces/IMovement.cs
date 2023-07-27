using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IMovement
    {
        Vector2 Input { get; set; }
        Vector2 DesiredVelocity { get; }
        Vector2 Velocity { get; }
    }
}

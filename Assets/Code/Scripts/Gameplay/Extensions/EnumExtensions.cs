using UnityEngine;

namespace Tulip.Gameplay.Extensions
{
    public static class EnumExtensions
    {
        public static Vector2 ToVector2(this ItemSwingDirection direction) => direction switch
        {
            ItemSwingDirection.Left => Vector2.left,
            ItemSwingDirection.Right => Vector2.right,
            ItemSwingDirection.Down => Vector2.down,
            ItemSwingDirection.Up => Vector2.up,
            _ => Vector2.up
        };
    }
}

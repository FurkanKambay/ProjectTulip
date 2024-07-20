using System;
using UnityEngine;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        event Action<ItemStack> OnReady;
        event Action<ItemStack, Vector3> OnSwing;

        ItemStack CurrentStack { get; }
        Vector2 AimDirection { get; }
    }
}

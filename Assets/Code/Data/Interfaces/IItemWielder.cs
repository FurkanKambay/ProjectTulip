using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        event Action<ItemStack> OnReady;
        event Action<ItemStack, Vector3> OnSwing;

        ItemStack CurrentStack { get; }
    }
}

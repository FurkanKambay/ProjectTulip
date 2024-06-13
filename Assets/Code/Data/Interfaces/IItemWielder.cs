using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        event Action<Usable> OnCharge;
        event Action<Usable, Vector3> OnSwing;
        event Action<Usable> OnReady;

        Item CurrentItem { get; }
    }
}

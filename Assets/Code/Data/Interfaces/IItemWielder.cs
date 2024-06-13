using System;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        event Action<Usable> OnReady;
        event Action<Usable, Vector3> OnSwing;

        Item CurrentItem { get; }
    }
}

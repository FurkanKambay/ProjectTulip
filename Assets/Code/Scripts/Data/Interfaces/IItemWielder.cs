using System;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        event Action<Usable> OnCharge;
        event Action<Usable, ItemSwingDirection> OnSwing;
        event Action<Usable> OnReady;

        Item CurrentItem { get; }
    }
}

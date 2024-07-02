using System;

namespace Tulip.Data
{
    public interface IInventory
    {
        event Action OnModify;

        int Capacity { get; }

        ItemStack[] Items { get; }
        ItemStack this[int index] { get; }
    }
}

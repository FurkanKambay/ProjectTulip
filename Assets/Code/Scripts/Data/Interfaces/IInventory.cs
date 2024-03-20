using System;

namespace Tulip.Data
{
    public interface IInventory
    {
        ItemStack[] Items { get; }
        ItemStack this[int index] { get; }

        ItemStack HotbarSelected { get; }
        int HotbarSelectedIndex { get; }

        event Action OnModifyHotbar;
        event Action<int> OnChangeHotbarSelection;
    }
}

using System;

namespace Game.Data.Interfaces
{
    public interface IInventory
    {
        ItemStack[] Items { get; }
        ItemStack this[int index] { get; set; }

        ItemStack HotbarSelected { get; }
        int HotbarSelectedIndex { get; }

        event Action OnModifyHotbar;
        event Action<int> OnChangeHotbarSelection;
    }
}

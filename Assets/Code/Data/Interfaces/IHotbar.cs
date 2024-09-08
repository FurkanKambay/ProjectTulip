using System;

namespace Tulip.Data
{
    public interface IHotbar
    {
        event Action OnModify;
        event Action<int> OnChangeSelection;

        int Size { get; }
        int SelectedIndex { get; }
        ItemStack SelectedStack { get; }

        ItemStack[] Items { get; }
        ItemStack this[int index] { get; }
    }
}

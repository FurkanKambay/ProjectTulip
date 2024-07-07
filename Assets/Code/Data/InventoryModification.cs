namespace Tulip.Data
{
    public readonly struct InventoryModification
    {
        public static readonly InventoryModification Empty = new();

        public bool WouldRemove => ToRemove is { IsValid: true };
        public bool WouldAdd => ToAdd is { IsValid: true };
        public bool WouldModify => WouldAdd || WouldRemove;

        public readonly ItemStack ToRemove;
        public readonly ItemStack ToAdd;

        public InventoryModification(ItemStack toRemove = default, ItemStack toAdd = default)
        {
            ToRemove = toRemove;
            ToAdd = toAdd;
        }
    }
}

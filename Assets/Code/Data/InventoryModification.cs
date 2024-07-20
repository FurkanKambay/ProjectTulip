namespace Tulip.Data
{
    public readonly struct InventoryModification
    {
        public bool IsValid => Stack.IsValid;
        public bool WouldAdd => IsValid && kind == InventoryModificationKind.Add;
        public bool WouldRemove => IsValid && kind == InventoryModificationKind.Remove;

        public readonly ItemStack Stack;
        private readonly InventoryModificationKind kind;

        public static InventoryModification ToAdd(ItemStack stack) => new(stack, InventoryModificationKind.Add);
        public static InventoryModification ToRemove(ItemStack stack) => new(stack, InventoryModificationKind.Remove);

        private InventoryModification(ItemStack stack, InventoryModificationKind kind)
        {
            Stack = stack;
            this.kind = kind;
        }

        private enum InventoryModificationKind
        {
            Add,
            Remove
        }
    }
}

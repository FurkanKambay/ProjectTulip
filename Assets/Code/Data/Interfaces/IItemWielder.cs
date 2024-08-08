using UnityEngine;

namespace Tulip.Data
{
    public interface IItemWielder
    {
        public delegate void ItemReadyEvent(ItemStack stack);
        public delegate void ItemSwingEvent(ItemStack stack, Vector3 aimPoint);

        event ItemReadyEvent OnReady;
        event ItemSwingEvent OnSwingStart;
        event ItemSwingEvent OnSwingPerform;

        ItemStack CurrentStack { get; }
        Vector2 AimDirection { get; }
    }
}

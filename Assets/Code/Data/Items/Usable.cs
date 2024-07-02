using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(fileName = "Usable Item", menuName = "Items/Usable Item")]
    public class Usable : Item
    {
        public virtual float Cooldown => cooldown;
        public virtual float SwingTime => swingTime;
        public virtual bool IsInstantSwing => isInstantSwing;

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;
        [SerializeField, Min(1)] protected float swingTime = 10f;
        [SerializeField] protected bool isInstantSwing;
    }
}

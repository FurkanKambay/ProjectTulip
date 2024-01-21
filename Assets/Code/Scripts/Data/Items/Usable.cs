using Game.Data.Interfaces;
using UnityEngine;

namespace Game.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(fileName = "Usable Item", menuName = "Items/Usable Item")]
    public class Usable : Item, IUsable
    {
        public float Cooldown => cooldown;

        [Header("Usable Data")]
        [SerializeField, Min(0)] float cooldown = .5f;
    }
}

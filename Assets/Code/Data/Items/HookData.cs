using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Hook", order = 3)]
    public class HookData : UsableData
    {
        public float Range => range;
        public float RopeLaunchSpeed => ropeLaunchSpeed;
        public float PullStrength => pullStrength;

        [Header("Hook Data")]
        [SerializeField, Min(0)] float range;
        [SerializeField, Min(0)] float ropeLaunchSpeed;
        [SerializeField, Min(0)] float pullStrength;
    }
}

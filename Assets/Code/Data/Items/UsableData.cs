using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Usable")]
    public class UsableData : ItemData
    {
        public float Cooldown => cooldown;
        public ItemSwingConfig SwingConfig => swingConfig;

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;

        [FormerlySerializedAs("swingType")]
        [FormerlySerializedAs("swingTypeData")]
        [BelowRichLabel(nameof(SwingTypeLabel), isCallback: true)]
        [SerializeField] protected ItemSwingConfig swingConfig;

        public float GetTimeToFirstHit()
        {
            if (!swingConfig || swingConfig.Phases.Length == 0)
                return 0;

            float durationSum = swingConfig.Phases
                .TakeWhile(p => !p.shouldHit)
                .Sum(p => Mathf.Max(p.moveDuration, p.turnDuration));

            UsePhase hitPhase = swingConfig.Phases.First(p => p.shouldHit);
            return durationSum + Mathf.Max(hitPhase.moveDuration, hitPhase.turnDuration);
        }

        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {GetTimeToFirstHit()} sec";
    }
}

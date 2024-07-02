using System.Linq;
using SaintsField;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Usable Item")]
    public class Usable : Item
    {
        public virtual float Cooldown => cooldown;
        public virtual ItemSwingType SwingType => swingType;

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;

        [BelowRichLabel(nameof(SwingTypeLabel), isCallback: true)]
        [SerializeField] protected ItemSwingType swingType;

        public float GetTimeToFirstHit()
        {
            if (!swingType || swingType.Phases.Length == 0)
                return 0;

            float durationSum = swingType.Phases
                .TakeWhile(p => !p.shouldHit)
                .Sum(p => Mathf.Max(p.moveDuration, p.turnDuration));

            UsePhase hitPhase = swingType.Phases.First(p => p.shouldHit);
            return durationSum + Mathf.Max(hitPhase.moveDuration, hitPhase.turnDuration);
        }

        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {GetTimeToFirstHit()} sec";
    }
}

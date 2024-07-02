using System.Linq;
using SaintsField;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class ItemSwingType : ScriptableObject
    {
        public Vector2 ReadyPosition => readyPosition;
        public float ReadyAngle => readyAngle;
        public float ResetMoveDuration => resetMoveDuration;
        public float ResetTurnDuration => resetTurnDuration;
        public UsePhase[] Phases => phases;

        [Header("Config")]
        [SerializeField] protected Vector2 readyPosition;

        [OverlayRichLabel("<color=grey>deg")]
        [SerializeField] protected float readyAngle;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField] protected float resetMoveDuration;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField] protected float resetTurnDuration;

        [Header("Phases")]
        [SaintsRow(inline: true)]
        [SerializeField] protected UsePhase[] phases;

        private void OnValidate()
        {
            if (phases.Length > 0 && !phases.Any(phase => phase.shouldHit))
                phases[0].shouldHit = true;
        }
    }
}

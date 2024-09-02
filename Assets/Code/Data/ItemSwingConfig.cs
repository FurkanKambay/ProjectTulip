using System.Linq;
using SaintsField;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Item Swing Config")]
    public class ItemSwingConfig : ScriptableObject
    {
        public Vector2 ReadyPosition => readyPosition;
        public float ReadyAngle => readyAngle;
        public float ResetMoveDuration => resetMoveDuration;
        public float ResetTurnDuration => resetTurnDuration;

        /// Avoid resetting and loop to phase 0 after last phase.
        public bool Loop => loop;
        public UsePhase[] Phases => phases;

        [Header("Config")]
        [SerializeField] protected Vector2 readyPosition;

        [OverlayRichLabel("<color=grey>deg")]
        [SerializeField] protected float readyAngle;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField, Min(0)] protected float resetMoveDuration;

        [OverlayRichLabel("<color=grey>sec")]
        [SerializeField, Min(0)] protected float resetTurnDuration;

        [Header("Phases")]
        [InfoBox("Avoid resetting and loop to phase 0 after last phase.", show: nameof(loop))]
        [SerializeField] protected bool loop;

        [SaintsRow(inline: true)]
        [SerializeField] protected UsePhase[] phases;

        private void OnValidate()
        {
            if (phases.Length > 0 && !phases.Any(phase => phase.shouldHit))
                phases[0].shouldHit = true;
        }
    }
}

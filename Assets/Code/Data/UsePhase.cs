using System;
using UnityEngine;
using SaintsField;
using SaintsField.Playa;

namespace Tulip.Data
{
    [Serializable]
    public struct UsePhase
    {
        [Layout("", ELayout.Background)]
        [Layout("/Options", ELayout.TitleOut)]
        public bool isCancelable;

        [Layout("/Options")]
        public bool shouldHit;

        [Layout("/Transform", ELayout.TitleOut | ELayout.Horizontal)]
        [PostFieldRichLabel("in")]
        public Vector2 moveDelta;

        [Layout("/Transform")]
        [OverlayRichLabel("<color=grey>sec")] public float moveDuration;

        [Layout("/Turn", ELayout.Horizontal)]
        [PostFieldRichLabel("in")]
        [OverlayRichLabel("<color=grey>deg")] public float turnDelta;

        [Layout("/Turn")]
        [OverlayRichLabel("<color=grey>sec")] public float turnDuration;
    }
}

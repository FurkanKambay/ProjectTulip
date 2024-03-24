using UnityEngine;

namespace Tulip.Data.Gameplay
{
    public readonly struct DamageEventArgs
    {
        public readonly float Amount;
        public readonly IHealth Source;
        public readonly IHealth Target;
        public readonly Vector2 SourcePosition;

        public DamageEventArgs(float amount, IHealth source, IHealth target, Vector2 sourcePosition)
        {
            Amount = amount;
            Source = source;
            Target = target;
            SourcePosition = sourcePosition;
        }

        public override string ToString() => $"{Source} damaged {Target} for {Amount}";
    }
}

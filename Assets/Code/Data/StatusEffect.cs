using System;
using UnityEngine;

namespace Tulip.Data
{
    /// TODO: support non-Health effects
    [Serializable]
    public class StatusEffect
    {
        public bool IsDone => !Data.IsPermanent && RemainingDuration <= 0;

        [field: SerializeField] public StatusEffectData Data { get; private set; }
        [field: SerializeField] public HealthBase Source { get; private set; }
        [field: SerializeField] public HealthBase Target { get; private set; }

        public float RemainingDuration { get; private set; }

        private float timeSinceLastProc;

        internal StatusEffect(StatusEffectData data, HealthBase source, HealthBase target)
        {
            Data = data;
            Source = source;
            Target = target;
            RemainingDuration = data.Duration;
        }

        public void Tick(float deltaTime)
        {
            timeSinceLastProc += deltaTime;
            RemainingDuration -= deltaTime;

            if (timeSinceLastProc < Data.Rate || IsDone)
                return;

            Proc();
            timeSinceLastProc = 0;
        }

        private void Proc()
        {
            if (Data.Amount < 0)
                Target.Damage(-Data.Amount, Source, checkInvulnerable: false);
            else
                Target.Heal(Data.Amount, Source);
        }
    }
}

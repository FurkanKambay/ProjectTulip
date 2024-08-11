using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class EnemyAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;

        [Header("FMOD Events")]
        [SerializeField, Required] StudioEventEmitter hurtSfx;

        private PARAMETER_ID paramLifeState;

        private void Awake()
        {
            EventDescription description = RuntimeManager.GetEventDescription(hurtSfx.EventReference);
            description.getParameterDescriptionByName("Life State", out PARAMETER_DESCRIPTION paramDesc);
            paramLifeState = paramDesc.id;
        }

        private void OnEnable() => health.I.OnHurt += HandleHurt;
        private void OnDisable() => health.I.OnHurt -= HandleHurt;

        private void HandleHurt(HealthChangeEventArgs damage)
        {
            LifeState lifeState = damage.Target.IsAlive ? LifeState.Alive : LifeState.Dead;
            hurtSfx.Play();
            hurtSfx.SetParameter(paramLifeState, (float)lifeState);
        }
    }
}

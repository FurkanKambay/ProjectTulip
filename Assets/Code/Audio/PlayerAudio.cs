using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("FMOD Events")]
        [SerializeField, Required] StudioEventEmitter hurtSfx;
        [SerializeField, Required] StudioEventEmitter itemSfx;

        private PARAMETER_ID paramLifeState;

        private void Awake()
        {
            EventDescription description = RuntimeManager.GetEventDescription(hurtSfx.EventReference);
            description.getParameterDescriptionByName("Life State", out PARAMETER_DESCRIPTION paramDesc);
            paramLifeState = paramDesc.id;
        }

        private void OnEnable()
        {
            health.I.OnHurt += HandleHurt;
            itemWielder.I.OnSwingPerform += HandleItemSwing;
        }

        private void OnDisable()
        {
            health.I.OnHurt -= HandleHurt;
            itemWielder.I.OnSwingPerform -= HandleItemSwing;
        }

        private void HandleHurt(HealthChangeEventArgs damage)
        {
            LifeState lifeState = damage.Target.IsAlive ? LifeState.Alive : LifeState.Dead;
            hurtSfx.Play();
            hurtSfx.SetParameter(paramLifeState, (float)lifeState);
        }

        private void HandleItemSwing(ItemStack stack, Vector3 _) => itemSfx.Play();
    }
}

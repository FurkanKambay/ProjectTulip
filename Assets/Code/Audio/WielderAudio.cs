using FMOD.Studio;
using FMODUnity;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class WielderAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;
        [SerializeField] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("FMOD Events")]
        [SerializeField] EventReference hurtEvent;
        [SerializeField] EventReference itemSwingEvent;

        private PARAMETER_ID paramLifeState;

        private void Awake()
        {
            EventDescription description = RuntimeManager.GetEventDescription(hurtEvent);
            description.getParameterDescriptionByName("Life State", out PARAMETER_DESCRIPTION paramDesc);
            paramLifeState = paramDesc.id;
        }

        private void OnEnable()
        {
            health.I.OnHurt += HandleHurt;

            if (itemWielder.V)
                itemWielder.I.OnSwingPerform += HandleItemSwing;
        }

        private void OnDisable()
        {
            health.I.OnHurt -= HandleHurt;

            if (itemWielder.V)
                itemWielder.I.OnSwingPerform -= HandleItemSwing;
        }

        private void HandleHurt(HealthChangeEventArgs damage)
        {
            EventInstance hurtSfx = RuntimeManager.CreateInstance(hurtEvent);
            RuntimeManager.AttachInstanceToGameObject(hurtSfx, transform);

            LifeState lifeState = damage.Target.IsAlive ? LifeState.Alive : LifeState.Dead;
            hurtSfx.setParameterByID(paramLifeState, (float)lifeState);

            hurtSfx.start();
            hurtSfx.release();
        }

        private void HandleItemSwing(ItemStack stack, Vector3 _) =>
            RuntimeManager.PlayOneShotAttached(itemSwingEvent, transform.gameObject);
    }
}

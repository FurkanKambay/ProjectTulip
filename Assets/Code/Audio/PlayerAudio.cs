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

        private const string paramLifeState = "Life State";

        private void HandleHurt(HealthChangeEventArgs damage)
        {
            LifeState lifeState = damage.Target.IsAlive ? LifeState.Alive : LifeState.Dead;
            hurtSfx.Play();
            hurtSfx.SetParameter(paramLifeState, (float)lifeState);
        }

        private void HandleItemSwing(ItemStack stack, Vector3 _) => itemSfx.Play();

        private void OnEnable()
        {
            health.I.OnHurt += HandleHurt;
            itemWielder.I.OnSwing += HandleItemSwing;
        }

        private void OnDisable()
        {
            health.I.OnHurt -= HandleHurt;
            itemWielder.I.OnSwing -= HandleItemSwing;
        }
    }
}

using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] AudioSource audioSource;
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;
        [SerializeField, Required] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("Health")]
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip dieSound;

        [Header("Item Wielder")]
        [SerializeField] AudioClip swingSound;

        private void HandleHurt(DamageEventArgs damage) => audioSource.PlayOneShot(hurtSound);
        private void HandleDied(DamageEventArgs damage) => audioSource.PlayOneShot(dieSound);
        private void HandleItemSwing(ItemStack stack, Vector3 _) => audioSource.PlayOneShot(swingSound);

        private void OnEnable()
        {
            health.I.OnHurt += HandleHurt;
            health.I.OnDie += HandleDied;
            itemWielder.I.OnSwing += HandleItemSwing;
        }

        private void OnDisable()
        {
            health.I.OnHurt -= HandleHurt;
            health.I.OnDie -= HandleDied;
            itemWielder.I.OnSwing -= HandleItemSwing;
        }
    }
}

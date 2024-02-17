using Tulip.Data.Items;
using Tulip.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip dieSound;

        [Header("Item Wielder")]
        [SerializeField] AudioClip chargeSound;
        [SerializeField] AudioClip swingSound;

        private AudioSource audioSource;
        private Health health;
        private ItemWielder itemWielder;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            itemWielder = GetComponentInParent<ItemWielder>();
            health = GetComponentInParent<Health>();
        }

        private void HandleHurt(DamageEventArgs damage) => audioSource.PlayOneShot(hurtSound);
        private void HandleDied(DamageEventArgs damage) => audioSource.PlayOneShot(dieSound);
        private void HandleItemCharge(Usable item) => audioSource.PlayOneShot(chargeSound);
        private void HandleItemSwing(Usable item, ItemSwingDirection _) => audioSource.PlayOneShot(swingSound);

        private void OnEnable()
        {
            health.OnHurt += HandleHurt;
            health.OnDie += HandleDied;
            itemWielder.OnCharge += HandleItemCharge;
            itemWielder.OnSwing += HandleItemSwing;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDied;
            itemWielder.OnCharge -= HandleItemCharge;
            itemWielder.OnSwing -= HandleItemSwing;
        }
    }
}

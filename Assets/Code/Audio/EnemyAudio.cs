using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class EnemyAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] AudioSource audioSource;
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;

        [Header("Health")]
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip dieSound;

        private void HandleHurt(DamageEventArgs damage) => audioSource.PlayOneShot(hurtSound);

        private void HandleDied(DamageEventArgs damage) => audioSource.PlayOneShot(dieSound);

        private void OnEnable()
        {
            health.I.OnHurt += HandleHurt;
            health.I.OnDie += HandleDied;
        }

        private void OnDisable()
        {
            health.I.OnHurt -= HandleHurt;
            health.I.OnDie -= HandleDied;
        }
    }
}

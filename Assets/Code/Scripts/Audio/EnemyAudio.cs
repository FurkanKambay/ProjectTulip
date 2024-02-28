using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Audio
{
    public class EnemyAudio : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip dieSound;

        private AudioSource audioSource;
        private IHealth health;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            health = GetComponentInParent<IHealth>();
        }

        private void HandleHurt(DamageEventArgs damage) => audioSource.PlayOneShot(hurtSound);

        private void HandleDied(DamageEventArgs damage) => audioSource.PlayOneShot(dieSound);

        private void OnEnable()
        {
            health.OnHurt += HandleHurt;
            health.OnDie += HandleDied;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDied;
        }
    }
}

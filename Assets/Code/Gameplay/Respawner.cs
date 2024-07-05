using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class Respawner : MonoBehaviour, IRespawner
    {
        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] Rigidbody2D body;

        [Header("Config")]
        [SerializeField] bool autoRespawn = true;
        [SerializeField] float respawnDelay;
        [SerializeField] Vector3 respawnPosition;

        public float SecondsUntilRespawn { get; private set; }
        public bool CanRespawn => SecondsUntilRespawn <= 0;

        private void Update()
        {
            if (health.IsAlive)
                return;

            SecondsUntilRespawn -= Time.deltaTime;

            if (autoRespawn)
                TryRespawn();
        }

        [ContextMenu(nameof(TryRespawn))]
        public void TryRespawn()
        {
            if (!CanRespawn)
                return;

            health.Revive();
            body.MovePosition(respawnPosition);
            SecondsUntilRespawn = 0;
        }

        private void OnEnable() => health.OnDie += HandleDeath;
        private void OnDisable() => health.OnDie -= HandleDeath;

        private void HandleDeath(DamageEventArgs _) => SecondsUntilRespawn = respawnDelay;
    }
}

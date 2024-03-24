using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Respawner : MonoBehaviour, IRespawner
    {
        [SerializeField] bool autoRespawn = true;
        [SerializeField] float respawnDelay;
        [SerializeField] Vector3 respawnPosition;

        public float SecondsUntilRespawn { get; private set; }
        public bool CanRespawn => SecondsUntilRespawn <= 0;

        private IHealth health;
        private Rigidbody2D body;

        private void Awake()
        {
            health = GetComponent<IHealth>();
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (health.IsAlive) return;

            SecondsUntilRespawn -= Time.deltaTime;
            if (!CanRespawn) return;

            if (autoRespawn)
                Respawn();
        }

        [ContextMenu(nameof(Respawn))]
        public void Respawn()
        {
            if (!CanRespawn) return;

            health.Revive();
            body.MovePosition(respawnPosition);

            SecondsUntilRespawn = 0;
        }

        private void OnEnable() => health.OnDie += HandleDeath;
        private void OnDisable() => health.OnDie -= HandleDeath;

        private void HandleDeath(DamageEventArgs _) => SecondsUntilRespawn = respawnDelay;
    }
}

using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Character
{
    public class TangibleEntity : MonoBehaviour, ITangibleEntity
    {
        [Header("References")]
        public World world;
        [SerializeField] Rigidbody2D body;
        [SerializeField] Health health;

        public IWorld World => world;

        private void OnEnable()
        {
            if (!health)
                return;

            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            if (!health)
                return;

            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private void HandleDied(HealthChangeEventArgs damage) => body.simulated = false;
        private void HandleRevived(IHealth reviver) => body.simulated = true;
    }
}

using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Character
{
    public class TangibleEntity : MonoBehaviour, ITangibleEntity
    {
        [Header("References")]
        [SerializeField, Required] Entity entity;
        [SerializeField] Rigidbody2D body;
        [SerializeField] Health health;
        public World world;

        public Entity Entity => entity;
        public IWorld World => world;
        public Vector3Int Cell { get; private set; }

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

        public void Destroy() => Destroy(gameObject);

        private void HandleDied(HealthChangeEventArgs damage) => body.simulated = false;
        private void HandleRevived(IHealth reviver) => body.simulated = true;

        public void SetResidence(World homeWorld, Vector3Int baseCell)
        {
            world = homeWorld;
            Cell = baseCell;
        }
    }
}

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
        public HealthBase Health => health;

        public IWorld World => world;
        public Vector3Int Cell { get; private set; }
        public RectInt Rect => new ((Vector2Int)Cell, Entity.Size);

        private void OnEnable()
        {
            if (!health || !body)
                return;

            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            if (!health || !body)
                return;

            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private void HandleDied(HealthChangeEventArgs damage) => body.simulated = false;
        private void HandleRevived(IHealth reviver) => body.simulated = true;

        public void SetResidence(World homeWorld, Vector3Int baseCell)
        {
            world = homeWorld;
            Cell = baseCell;
        }
    }
}

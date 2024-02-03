using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

namespace Game.Debugging
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField, Min(0)] float spawnRadius = 5f;
        [SerializeField, Min(1)] int enemyHeight;
        [SerializeField] GameObject[] enemies;

        private KeyControl keySpawnRandom;
        private World world;

        private void Awake()
        {
            world = World.Instance;
            keySpawnRandom = Keyboard.current.rKey;
        }

        private void Update()
        {
            if (!keySpawnRandom.wasPressedThisFrame) return;
            if (!(enemies?.Length > 0)) return;

            GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Length)]);
            enemy.transform.position = GetRandomPosition();
        }

        private Vector3 GetRandomPosition()
        {
            while (true)
            {
                Vector2 randomPoint = (Vector2)player.position + (Random.insideUnitCircle * spawnRadius);
                if (!world.HasTile(world.WorldToCell(randomPoint)))
                    return randomPoint;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector2 mouseWorld = InputHelper.Instance.MouseWorldPoint;
            Vector3Int mouseCell = world.WorldToCell(mouseWorld);
            var enemySize = new Vector2Int(1, enemyHeight);

            Vector3 cellWorld = world.CellCenter(mouseCell);
            var gizmoSize = new Vector3(enemySize.x, enemySize.y, 1);
            var gizmoCenter = new Vector3(cellWorld.x, cellWorld.y + (gizmoSize.y / 2f) - 0.5f, 0);

            Gizmos.color = world.CanAccommodate(mouseCell, enemySize) ? Color.green : Color.red;
            Gizmos.DrawWireCube(gizmoCenter, gizmoSize);
        }
    }
}

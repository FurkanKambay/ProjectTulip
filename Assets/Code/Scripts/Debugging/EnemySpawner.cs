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
                if (!world.HasBlock(world.WorldToCell(randomPoint)))
                    return randomPoint;
            }
        }
    }
}

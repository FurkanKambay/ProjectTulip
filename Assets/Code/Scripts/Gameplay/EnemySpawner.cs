using System.Collections.Generic;
using System.Linq;
using Tulip.GameWorld;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        public Vector3Int[] SuitableSpawnCells { get; private set; }

        [SerializeField, Min(0)] int spawnRadius = 5;
        [SerializeField, Min(1)] int enemyHeight = 2;
        [SerializeField, Min(0)] float spawnInterval = 10f;
        [SerializeField] GameObject[] enemyOptions;

        private Camera mainCamera;

        private Vector3Int EnemySize => new(1, enemyHeight, 1);

        private void SpawnRandomEnemy()
        {
            UpdateSuitableCoordinates();
            if (SuitableSpawnCells.Length == 0) return;

            GameObject randomEnemy = Instantiate(GetRandomEnemy(), transform);
            randomEnemy.transform.position = World.Instance.CellCenter(GetRandomSpawnCell());
        }

        private void UpdateSuitableCoordinates() => SuitableSpawnCells = FindSuitableCells().ToArray();

        private GameObject GetRandomEnemy() => enemyOptions[Random.Range(0, enemyOptions.Length)];
        private Vector3Int GetRandomSpawnCell() => SuitableSpawnCells[Random.Range(0, SuitableSpawnCells.Length)];

        private IEnumerable<Vector3Int> FindSuitableCells()
        {
            float camExtentY = mainCamera.orthographicSize;
            float camExtentX = camExtentY * mainCamera.aspect;
            float spawnExtentY = camExtentY + spawnRadius;
            float spawnExtentX = camExtentX + spawnRadius;

            Vector3 cameraCenter = mainCamera.transform.position;

            Vector3Int camTopRight = (cameraCenter + new Vector3(camExtentX, camExtentY)).ToCell();
            Vector3Int camBottomLeft = (cameraCenter - new Vector3(camExtentX, camExtentY)).ToCell();
            Vector3Int spawnTopRight = (cameraCenter + new Vector3(spawnExtentX, spawnExtentY)).ToCell();
            Vector3Int spawnBottomLeft = (cameraCenter - new Vector3(spawnExtentX, spawnExtentY)).ToCell();

            for (int y = spawnBottomLeft.y; y <= spawnTopRight.y; y++)
            {
                for (int x = spawnBottomLeft.x; x <= spawnTopRight.x; x++)
                {
                    if (y <= camTopRight.y && y >= camBottomLeft.y &&
                        x <= camTopRight.x && x >= camBottomLeft.x) continue;

                    var cell = new Vector3Int(x, y);
                    if (World.Instance.CanAccommodate(cell, (Vector2Int)EnemySize))
                        yield return cell;
                }
            }
        }

        private void Awake()
        {
            Assert.IsTrue(enemyOptions?.Length > 0);
            mainCamera = Camera.main;
        }

        private void OnEnable() => InvokeRepeating(nameof(SpawnRandomEnemy), 0, spawnInterval);
        private void OnDisable() => CancelInvoke(nameof(SpawnRandomEnemy));

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            DrawGizmosForSpawnCells();
        }

        private void DrawGizmosForSpawnCells()
        {
            Gizmos.color = Color.yellow;

            foreach (Vector3Int cell in SuitableSpawnCells)
                Gizmos.DrawSphere(World.Instance.CellCenter(cell), 0.4f);
        }
    }
}

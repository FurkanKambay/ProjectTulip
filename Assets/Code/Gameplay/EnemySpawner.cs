using System.Collections.Generic;
using System.Linq;
using SaintsField;
using SaintsField.Playa;
using Tulip.GameWorld;
using UnityEditor;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [LayoutGroup("References", ELayout.Background | ELayout.TitleOut)]
        [SerializeField] World world;
        [SerializeField] new Camera camera;
        [SerializeField] Transform spawnParent;

        [LayoutGroup("Config", ELayout.Background | ELayout.TitleOut)]
        [LayoutGroup("Config/Spawning", ELayout.TitleOut)]
        [SerializeField, Min(0)] int maxSpawns = 100;

        [OverlayRichLabel("<color=gray>tiles")]
        [SerializeField, Min(0)] int radius = 5;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float interval = 10f;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float gracePeriod;

        [LayoutGroup("Config/Enemies", ELayout.TitleOut)]

        [OverlayRichLabel("<color=gray>tiles")]
        [SerializeField, Min(1)] int enemyHeight = 2;

        [SerializeField] GameObject[] enemyOptions;

        private IEnumerable<Vector3Int> suitableCells;

        [Button]
        // ReSharper disable once UnusedMember.Local
        private void DestroyAllSpawns()
        {
            for (int i = 0; i < spawnParent.childCount; i++)
                Destroy(spawnParent.GetChild(i).gameObject);
        }

        private void OnEnable() =>
            InvokeRepeating(nameof(SpawnEnemy), gracePeriod, interval);

        private void OnDisable() =>
            CancelInvoke(nameof(SpawnEnemy));

        private void SpawnEnemy()
        {
            if (spawnParent.childCount >= maxSpawns)
                return;

            if (enemyOptions.Length == 0)
                return;

            suitableCells = GetSuitableCells();

            if (!suitableCells.Any())
                return;

            GameObject randomEnemy = SpawnRandomEnemy();
            randomEnemy.transform.position = world.CellCenter(GetRandomSpawnCell());
        }

        private GameObject SpawnRandomEnemy() =>
            Instantiate(enemyOptions[Random.Range(0, enemyOptions.Length)], spawnParent);

        private Vector3Int GetRandomSpawnCell() =>
            suitableCells.ElementAt(Random.Range(0, suitableCells.Count()));

        private IEnumerable<Vector3Int> GetSuitableCells()
        {
            float camExtentY = camera.orthographicSize;
            float camExtentX = camExtentY * camera.aspect;
            float spawnExtentY = camExtentY + radius;
            float spawnExtentX = camExtentX + radius;

            Vector3 cameraCenter = camera.transform.position;

            Vector3Int camTopRight = world.WorldToCell(cameraCenter + new Vector3(camExtentX, camExtentY));
            Vector3Int camBottomLeft = world.WorldToCell(cameraCenter - new Vector3(camExtentX, camExtentY));
            Vector3Int spawnTopRight = world.WorldToCell(cameraCenter + new Vector3(spawnExtentX, spawnExtentY));
            Vector3Int spawnBottomLeft = world.WorldToCell(cameraCenter - new Vector3(spawnExtentX, spawnExtentY));

            Vector2Int enemySize = new(1, enemyHeight);

            for (int y = spawnBottomLeft.y; y <= spawnTopRight.y; y++)
            {
                for (int x = spawnBottomLeft.x; x <= spawnTopRight.x; x++)
                {
                    if (y <= camTopRight.y && y >= camBottomLeft.y && x <= camTopRight.x && x >= camBottomLeft.x)
                        continue;

                    var cell = new Vector3Int(x, y);

                    if (world.CanAccommodate(cell, enemySize))
                        yield return cell;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.yellow;

            foreach (Vector3Int cell in GetSuitableCells())
                Handles.DrawSolidDisc(world.CellCenter(cell), Vector3.forward, 0.2f);
        }
#endif
    }
}

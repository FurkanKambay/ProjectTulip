using System.Collections.Generic;
using System.Linq;
using SaintsField;
using SaintsField.Playa;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
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
        [SerializeField] Entity[] enemyOptions;

        private IEnumerable<Vector3Int> suitableCells;

        private bool isActive;
        private float timeSinceLastSpawn;

        [Button]
        private void DestroyAllSpawns()
        {
            for (int i = 0; i < spawnParent.childCount; i++)
                Destroy(spawnParent.GetChild(i).gameObject);

            world.ClearEntities();
        }

        private void OnEnable() => GameManager.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameManager.OnGameStateChange -= HandleGameStateChange;

        private void Update()
        {
            if (!isActive)
            {
                timeSinceLastSpawn = -gracePeriod;
                return;
            }

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn < interval)
                return;

            if (TrySpawnEnemy())
                timeSinceLastSpawn = 0;
        }

        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            isActive = newState != GameState.MainMenu;

            if (!isActive)
                DestroyAllSpawns();
        }

        private bool TrySpawnEnemy()
        {
            if (spawnParent.childCount >= maxSpawns)
                return false;

            if (enemyOptions.Length == 0)
                return false;

            Entity entity = GetRandomEnemy();
            suitableCells = GetSuitableCells(entity);

            if (!suitableCells.Any())
                return false;

            Vector3Int baseCell = GetRandomSpawnCell();
            Vector3Int centerCell = new(baseCell.x + (entity.Size.x / 2), baseCell.y);

            TangibleEntity spawnedEnemy = Spawn(entity.Prefab, world.CellCenter(centerCell));
            spawnedEnemy.SetResidence(world, baseCell);

            if (entity.IsStatic)
                world.TryAddStaticEntity(baseCell, spawnedEnemy);

            return true;
        }

        private TangibleEntity Spawn(GameObject prefab, Vector3 position) =>
            Instantiate(prefab, position, Quaternion.identity, spawnParent).GetComponent<TangibleEntity>();

        private Entity GetRandomEnemy() =>
            enemyOptions[Random.Range(0, enemyOptions.Length)];

        private Vector3Int GetRandomSpawnCell() =>
            suitableCells.ElementAt(Random.Range(0, suitableCells.Count()));

        private IEnumerable<Vector3Int> GetSuitableCells(Entity entity)
        {
            Vector3 cameraExtents = new(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            Vector3 spawnExtents = new(cameraExtents.x + radius, cameraExtents.y + radius);
            Vector3 cameraCenter = camera.transform.position;

            Vector3Int camTopRight = world.WorldToCell(cameraCenter + cameraExtents);
            Vector3Int camBottomLeft = world.WorldToCell(cameraCenter - cameraExtents);
            Vector3Int spawnTopRight = world.WorldToCell(cameraCenter + spawnExtents);
            Vector3Int spawnBottomLeft = world.WorldToCell(cameraCenter - spawnExtents);

            for (int y = spawnBottomLeft.y; y <= spawnTopRight.y; y++)
            {
                for (int x = spawnBottomLeft.x; x <= spawnTopRight.x; x++)
                {
                    if (y <= camTopRight.y && y >= camBottomLeft.y && x <= camTopRight.x && x >= camBottomLeft.x)
                        continue;

                    var cell = new Vector3Int(x, y);

                    if (entity.CanSpawnAt(world, cell))
                        yield return cell;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (enemyOptions.Length == 0)
                return;

            Handles.color = Color.yellow;

            foreach (Vector3Int cell in GetSuitableCells(enemyOptions[0]))
                Handles.DrawSolidDisc(world.CellCenter(cell), Vector3.forward, 0.2f);
        }
#endif
    }
}

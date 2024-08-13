using SaintsField;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class Entity : ScriptableObject
    {
        [SerializeField, Required] GameObject prefab;

        [Header("Spawning")]
        [SerializeField] bool isStatic;
        [SerializeField, Required] SpawnCondition spawnCondition;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        // BUG: only works with Static Entities in the world
        // TODO: make a separate LootTable class
        [Header("Loot")]
        [SerializeField] Item loot;
        [SerializeField] int lootAmount;

        public GameObject Prefab => prefab;
        public bool IsStatic => isStatic;
        public Vector2Int Size => size;
        public Item Loot => loot;
        public int LootAmount => lootAmount;

        public bool CanSpawnAt(IWorld world, Vector2Int cell) =>
            spawnCondition.CanSpawn(this, world, cell);
    }
}

using SaintsField;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class Entity : ScriptableObject
    {
        [SerializeField, Required] GameObject prefab;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        [SerializeField, Required] SpawnCondition spawnCondition;

        public GameObject Prefab => prefab;
        public Vector2Int Size => size;

        public bool CanSpawnAt(IWorld world, Vector3Int cell) =>
            spawnCondition.CanSpawn(this, world, cell);
    }
}

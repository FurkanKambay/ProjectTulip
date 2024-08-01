using SaintsField;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class Entity : ScriptableObject
    {
        [SerializeField, Required] GameObject prefab;

        [SerializeField] bool isStatic;
        [SerializeField, Required] SpawnCondition spawnCondition;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        public GameObject Prefab => prefab;
        public bool IsStatic => isStatic;
        public Vector2Int Size => size;

        public bool CanSpawnAt(IWorld world, Vector3Int cell) =>
            spawnCondition.CanSpawn(this, world, cell);
    }
}

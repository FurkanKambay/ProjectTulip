using System.Linq;
using SaintsField;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu]
    public class SpawnCondition : ScriptableObject
    {
        [Header("Ground")]
        [SerializeField] bool needsGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] bool needsSafeGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] Placeable[] groundTiles;

        [Header("Clearance")]
        [SerializeField, Min(0)] int clearanceAbove;

        [DisableIf(nameof(needsGround))]
        [SerializeField, Min(0)] int clearanceBelow;

        /// <param name="entity"></param>
        /// <param name="world"></param>
        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        public bool CanSpawn(Entity entity, IWorld world, Vector3Int baseCell)
        {
            if (!world.CanAccommodate(baseCell, entity.Size))
                return false;

            // Check tiles above
            for (int y = 0; y < clearanceAbove; y++)
            for (int x = 0; x < entity.Size.x; x++)
            {
                if (world.HasBlock(baseCell + new Vector3Int(x, entity.Size.y + y)))
                    return false;
            }

            // Check tiles below
            if (!needsGround)
            {
                for (int y = 1; y <= clearanceBelow; y++)
                for (int x = 0; x < entity.Size.x; x++)
                {
                    if (world.HasBlock(baseCell + new Vector3Int(x, -y)))
                        return false;
                }
            }

            if (!needsGround)
                return true;

            // Check ground only
            for (int x = 0; x < entity.Size.x; x++)
            {
                Vector3Int floorCell = baseCell + new Vector3Int(x, -1);
                Placeable floorTile = world.GetBlock(floorCell);

                if (!world.HasBlock(floorCell) || (needsSafeGround && floorTile.IsUnsafe))
                    return false;

                if (groundTiles.Length > 0 && !groundTiles.Contains(floorTile))
                    return false;
            }

            return true;
        }
    }
}

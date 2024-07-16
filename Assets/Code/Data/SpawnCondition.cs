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

        [DisableIf(nameof(needsAltitude))]
        [SerializeField] bool needsGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] bool needsSafeGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] WorldTile[] groundTiles;

        [Header("Altitude")]

        [DisableIf(nameof(needsGround))]
        [SerializeField] bool needsAltitude;

        [EnableIf(nameof(needsAltitude))]
        [SerializeField, Min(1)] int minAltitude;

        public bool CanSpawn(Enemy enemy, IWorld world, Vector3Int cell)
        {
            if (!world.CanAccommodate(cell, enemy.Size))
                return false;

            if (needsGround)
            {
                var floorCell = new Vector3Int(cell.x, cell.y - 1);
                WorldTile floorTile = world.GetBlock(floorCell);

                if (!world.HasBlock(floorCell) || (needsSafeGround && !floorTile.IsSafe))
                    return false;

                if (groundTiles.Length > 0 && !groundTiles.Contains(floorTile))
                    return false;
            }

            if (needsAltitude)
            {
                Vector3Int cellToCheck = cell;

                for (int y = 0; y < minAltitude; y++)
                {
                    cellToCheck.y--;

                    if (world.HasBlock(cellToCheck))
                        return false;
                }
            }

            return true;
        }
    }
}

using SaintsField;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.Data.Tiles
{
    [CreateAssetMenu(menuName = "World/Rule Tile")]
    public sealed class CustomRuleTileData : RuleTile<CustomRuleTileData.Neighbor>
    {
        [field: SerializeField, ReadOnly]
        public PlaceableData PlaceableData { get; internal set; }

        public override bool RuleMatch(int neighbor, TileBase tile) => neighbor switch
        {
            Neighbor.Null => tile == null,
            Neighbor.NotNull => tile != null,
            _ => base.RuleMatch(neighbor, tile)
        };

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(location, tilemap, ref tileData);
            tileData.color = PlaceableData.Color;

            if (PlaceableData.OreData)
                tileData.gameObject = PlaceableData.OreData.Prefab;
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor
        {
            public const int Null = 3;
            public const int NotNull = 4;
        }
    }
}

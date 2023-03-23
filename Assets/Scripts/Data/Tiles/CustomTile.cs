using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Data.Tiles
{
    public abstract class CustomTile : TileBase
    {
        public Sprite sprite;
        public Color color = Color.white;
        public Tile.ColliderType colliderType = Tile.ColliderType.Grid;

        private Matrix4x4 transform = Matrix4x4.identity;
        private GameObject instancedGameObject;
        private TileFlags flags = TileFlags.LockColor;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = this.sprite;
            tileData.color = this.color;
            tileData.transform = this.transform;
            tileData.gameObject = this.instancedGameObject;
            tileData.flags = this.flags;
            tileData.colliderType = this.colliderType;
        }
    }
}

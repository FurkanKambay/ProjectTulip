using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(fileName = "World Tile", menuName = "Items/World Tile")]
    public class WorldTile : Tool
    {
        public bool IsSafe => true;
        public CustomRuleTile RuleTile => ruleTile;

        public override Sprite Icon => ruleTile.m_DefaultSprite;
        public override float Cooldown => 0.25f;
        public override float ChargeTime => 0f;
        public override float SwingTime => 0f;

        [Header("Tile Data")]
        [SerializeField] CustomRuleTile ruleTile;
        public Color color = Color.white;

        [Header("World Tile Data")]
        [Min(1)] public int hardness = 50;

        [Header("Sounds")]
        public AudioClip hitSound;
        public AudioClip destroySound;
        public AudioClip placeSound;

        public override bool IsUsableOnTile(WorldTile worldTile) => worldTile is null;

        private void OnValidate() => RuleTile.WorldTile = this;

        private void Reset() => maxAmount = 999;
    }
}

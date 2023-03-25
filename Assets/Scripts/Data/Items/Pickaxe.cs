using UnityEngine;

namespace Game.Data.Items
{
    [CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
    public class Pickaxe : ScriptableObject, IUsable
    {
        public float UseTime => useTime;
        public Sprite Icon => icon;

        [SerializeField] private Sprite icon;

        [Header("Data")]
        public int power = 50;
        [SerializeField] private float useTime = .5f;

        public void Use(Vector3Int cell, Pickaxe pickaxe)
            => World.Instance.DamageBlock(cell, power);
    }
}

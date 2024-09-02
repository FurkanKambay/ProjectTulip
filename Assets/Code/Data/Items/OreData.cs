using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore", order = 1)]
    public class OreData : ItemData
    {
        public GameObject Prefab => prefab;

        [Header("Ore Data")]
        [SerializeField] GameObject prefab;
    }
}

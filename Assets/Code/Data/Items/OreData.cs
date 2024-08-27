using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore")]
    public class OreData : ItemData
    {
        public GameObject Prefab => prefab;

        [Header("Ore Data")]
        [SerializeField] GameObject prefab;
    }
}

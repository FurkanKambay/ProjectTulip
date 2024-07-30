using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore")]
    public class Ore : Item
    {
        public GameObject Prefab => prefab;

        [Header("Ore Data")]
        [SerializeField] GameObject prefab;
    }
}

using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(fileName = "Item Recipe", menuName = "Data/Item Recipe")]
    public class ItemRecipe : ScriptableObject
    {
        [field: SerializeField] public Item ItemResult { get; protected set; }
        [field: SerializeField] public ItemStack[] Ingredients { get; protected set; }
    }
}

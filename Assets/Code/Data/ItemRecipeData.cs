using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Player/Item Recipe")]
    public class ItemRecipeData : ScriptableObject
    {
        public ItemData ResultItemData => resultItemData;
        public ItemStack[] Ingredients => ingredients;

        [SerializeField] protected ItemData resultItemData;
        [SerializeField] protected ItemStack[] ingredients;
    }
}

using System.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore", order = 1)]
    public class OreData : ItemData
    {
        public GameObject Prefab => prefab;

        [Header("Ore Data")]
        [SerializeField] GameObject prefab;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout, marginTop: 16)]
        [SerializeField, ReadOnly] protected PlaceableData[] tileLoot;
        [SerializeField, ReadOnly] protected EntityData[] entityLoot;
        // ReSharper restore NotAccessedField.Global

        protected override void OnValidate()
        {
            base.OnValidate();

            tileLoot = Resources.FindObjectsOfTypeAll<PlaceableData>()
                .Where(placeableData => placeableData.OreData == this)
                .ToArray();

            entityLoot = Resources.FindObjectsOfTypeAll<EntityData>()
                .Where(entityData => entityData.Loot == this)
                .ToArray();
        }
    }
}

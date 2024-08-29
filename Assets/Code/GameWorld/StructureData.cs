using SaintsField;
using Tulip.Data;
using UnityEngine;

namespace Tulip.GameWorld
{
    [CreateAssetMenu(menuName = "Data/Structure")]
    public class StructureData : ScriptableObject
    {
        [SaintsRow(inline: true)]
        [SerializeField] internal WorldData worldData;
    }
}

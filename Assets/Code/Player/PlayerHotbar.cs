using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Player
{
    public sealed class PlayerHotbar : Hotbar
    {
        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IPlayerBrain> brain;

        private void Update()
        {
            if (brain.I.HotbarSelectionIndex.HasValue)
            {
                Select(brain.I.HotbarSelectionIndex.Value);
                return;
            }

            if (brain.I.HotbarSelectionDelta == 0)
                return;

            int currentIndex = SelectedIndex;
            Select(currentIndex - brain.I.HotbarSelectionDelta);
        }
    }
}

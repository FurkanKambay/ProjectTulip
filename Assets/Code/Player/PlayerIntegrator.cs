using SaintsField;
using Tulip.Core;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Player
{
    public sealed class PlayerIntegrator : MonoBehaviour
    {
        [SerializeField, Required] SaintsInterface<Component, IPlayerBrain> brain;
        [SerializeField] Inventory inventory;

        private void Update()
        {
            if (!brain.V) enabled = false;

            HandleSmartCursor();
            HandleHotbarSelection();
        }

        private void HandleHotbarSelection()
        {
            if (!inventory) return;

            if (brain.I.HotbarSelectionIndex.HasValue)
            {
                inventory.ChangeHotbarSelection(brain.I.HotbarSelectionIndex.Value);
                return;
            }

            if (brain.I.HotbarSelectionDelta == 0) return;

            int currentIndex = inventory.HotbarSelectedIndex;
            inventory.ChangeHotbarSelection(currentIndex - brain.I.HotbarSelectionDelta);
        }

        private void HandleSmartCursor()
        {
            if (brain.I.WantsToToggleSmartCursor)
                Options.Instance.Gameplay.UseSmartCursor = !Options.Instance.Gameplay.UseSmartCursor;
        }
    }
}

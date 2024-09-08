using SaintsField;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Player
{
    public sealed class PlayerIntegrator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] SaintsInterface<Component, IPlayerBrain> brain;
        [SerializeField] Hotbar hotbar;

        private void Update()
        {
            if (!brain.V)
                enabled = false;

            HandleSmartCursor();
            HandleHotbarSelection();
        }

        private void HandleHotbarSelection()
        {
            if (!hotbar)
                return;

            if (brain.I.HotbarSelectionIndex.HasValue)
            {
                hotbar.Select(brain.I.HotbarSelectionIndex.Value);
                return;
            }

            if (brain.I.HotbarSelectionDelta == 0)
                return;

            int currentIndex = hotbar.SelectedIndex;
            hotbar.Select(currentIndex - brain.I.HotbarSelectionDelta);
        }

        private void HandleSmartCursor()
        {
            if (brain.I.WantsToToggleSmartCursor)
                Options.Instance.Gameplay.UseSmartCursor = !Options.Instance.Gameplay.UseSmartCursor;
        }
    }
}

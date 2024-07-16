using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class UserBrain : MonoBehaviour, IUserBrain
    {
        [SerializeField] InputActionReference menu;
        [SerializeField] InputActionReference cancel;
        [SerializeField] InputActionReference switchTab;

        public bool WantsToMenu { get; private set; }
        public bool WantsToCancel { get; private set; }
        public int? TabSwitchDelta { get; private set; }

        private void Update()
        {
            WantsToMenu = menu.action.triggered;
            WantsToCancel = cancel.action.triggered;
            TabSwitchDelta = switchTab.action.triggered ? (int)switchTab.action.ReadValue<float>() : null;
        }
    }
}

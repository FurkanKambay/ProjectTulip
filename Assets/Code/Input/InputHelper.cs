using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : MonoBehaviour
    {
        private void Awake()
        {
            GameState.OnGameStateChange += HandleGameStateChanged;
            Debug.Log("[Input] Enabled input helper.");
        }

        private static void HandleGameStateChanged(GameState oldState, GameState newState)
        {
            InputActionMap playerControls = InputSystem.actions.actionMaps[0];
            InputActionMap uiControls = InputSystem.actions.actionMaps[1];

            if (newState.IsPlayerInputEnabled)
            {
                Debug.Log($"[Input] + Player input enabled by {newState}.");
                playerControls.Enable();
            }
            else
            {
                Debug.Log($"[Input] - Player input disabled by {newState}.");
                playerControls.Disable();
            }

            if (newState.IsUIInputEnabled)
            {
                Debug.Log($"[Input] - UI input enabled by {newState}.");
                uiControls.Enable();
            }
            else
            {
                Debug.Log($"[Input] - UI input disabled by {newState}.");
                uiControls.Disable();
            }
        }
    }
}

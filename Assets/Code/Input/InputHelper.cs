using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : MonoBehaviour
    {
        private void Awake() => Debug.Log("[Input] Enabled input helper.");
        private void OnEnable() => GameManager.OnGameStateChange += HandleGameStateChanged;
        private void OnDisable() => GameManager.OnGameStateChange -= HandleGameStateChanged;

        private static void HandleGameStateChanged(GameState oldState, GameState newState)
        {
            InputActionMap playerControls = InputSystem.actions.actionMaps[0];
            InputActionMap uiControls = InputSystem.actions.actionMaps[1];

            if (GameManager.IsPlayerInputEnabled)
            {
                Debug.Log($"[Input] + Player input enabled by {newState}.");
                playerControls.Enable();
            }
            else
            {
                Debug.Log($"[Input] - Player input disabled by {newState}.");
                playerControls.Disable();
            }

            if (GameManager.IsUIInputEnabled)
            {
                Debug.Log($"[Input] + UI input enabled by {newState}.");
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

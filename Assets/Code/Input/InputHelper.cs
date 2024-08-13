using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : MonoBehaviour
    {
        private void OnEnable() => GameManager.OnGameStateChange += HandleGameStateChanged;
        private void OnDisable() => GameManager.OnGameStateChange -= HandleGameStateChanged;

        private static void HandleGameStateChanged(GameState oldState, GameState newState)
        {
            InputActionMap playerControls = InputSystem.actions.actionMaps[0];
            InputActionMap uiControls = InputSystem.actions.actionMaps[1];

            if (GameManager.IsPlayerInputEnabled)
                playerControls.Enable();
            else
                playerControls.Disable();

            if (GameManager.IsUIInputEnabled)
                uiControls.Enable();
            else
                uiControls.Disable();
        }
    }
}

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

        private static void HandleGameStateChanged()
        {
            InputActionMap playerControls = InputSystem.actions.actionMaps[0];
            InputActionMap uiControls = InputSystem.actions.actionMaps[1];

            if (GameState.Current.IsPlayerInputEnabled)
            {
                Debug.Log($"[Input] + Player input enabled by {GameState.Current}.");
                playerControls.Enable();
            }
            else
            {
                Debug.Log($"[Input] - Player input disabled by {GameState.Current}.");
                playerControls.Disable();
            }

            if (GameState.Current.IsUIInputEnabled)
            {
                Debug.Log($"[Input] - UI input enabled by {GameState.Current}.");
                uiControls.Enable();
            }
            else
            {
                Debug.Log($"[Input] - UI input disabled by {GameState.Current}.");
                uiControls.Disable();
            }
        }
    }
}

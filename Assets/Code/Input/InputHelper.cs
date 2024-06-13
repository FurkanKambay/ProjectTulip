using Tulip.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Input
{
    public class InputHelper : ScriptableObject
    {
        private static InputHelper instance;

        private static void HandleGameStateChanged()
        {
            if (GameState.Current.IsPlayerInputEnabled)
            {
                Debug.Log($"[Input] + Player input enabled by {GameState.Current}.");
                InputSystem.actions.actionMaps[0].Enable();
                InputSystem.actions.actionMaps[1].Disable();
            }
            else
            {
                Debug.Log($"[Input] - Player input disabled by {GameState.Current}.");
                InputSystem.actions.actionMaps[0].Disable();
                InputSystem.actions.actionMaps[1].Enable();
            }
        }

        private void OnEnable()
        {
            instance = this;
            GameState.OnGameStateChange += HandleGameStateChanged;
            Debug.Log("[Input] Enabled input helper.");
        }
    }
}

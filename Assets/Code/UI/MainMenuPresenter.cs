using SaintsField;
using Tulip.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField, Required] UIDocument document;

        private VisualElement root;
        private Button playButton;

        private void OnEnable() => GameState.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameState.OnGameStateChange -= HandleGameStateChange;

        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            document.enabled = newState == GameState.MainMenu;

            if (document.enabled)
            {
                root = document.rootVisualElement.ElementAt(0);
                playButton = root.Q<Button>("PlayButton");
                playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
            }
            else
            {
                playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
            }
        }

        private async void HandlePlayClicked(ClickEvent _)
        {
            playButton.SetEnabled(false);
            playButton.text = "Loading...";

            await GameState.SwitchTo(GameState.Playing);
        }
    }
}

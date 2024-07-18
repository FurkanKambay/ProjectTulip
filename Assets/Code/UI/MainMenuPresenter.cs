using SaintsField;
using Tulip.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField] MenuPlayground playground;

        private VisualElement root;
        private Button playButton;

        private void OnEnable() => GameState.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameState.OnGameStateChange -= HandleGameStateChange;

        private void HandleGameStateChange(GameState oldState, GameState newState)
        {
            document.enabled = newState == GameState.MainMenu;
            playground.gameObject.SetActive(document.enabled);

            if (!document.enabled)
            {
                playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
                return;
            }

            root = document.rootVisualElement.ElementAt(0);
            playButton = root.Q<Button>("PlayButton");
            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
        }

        private async void HandlePlayClicked(ClickEvent _)
        {
            playButton.SetEnabled(false);
            playButton.text = "Loading...";

            await GameState.SwitchTo(GameState.Playing);
        }
    }
}

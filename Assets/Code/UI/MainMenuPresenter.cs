using FMODUnity;
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

        private void Awake() => UpdateCallbacks(GameManager.CurrentState);

        private void OnEnable() => GameManager.OnGameStateChange += HandleGameStateChange;
        private void OnDisable() => GameManager.OnGameStateChange -= HandleGameStateChange;

        private void UpdateCallbacks(GameState newState)
        {
            bool inMainMenu = newState == GameState.MainMenu;

            document.enabled = inMainMenu;
            playground.gameObject.SetActive(inMainMenu);

            if (!inMainMenu)
            {
                playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
                return;
            }

            root = document.rootVisualElement.ElementAt(0);
            playButton = root.Q<Button>("PlayButton");
            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
        }

        private void HandleGameStateChange(GameState oldState, GameState newState) =>
            UpdateCallbacks(newState);

        private void HandlePlayClicked(ClickEvent _)
        {
            playButton.SetEnabled(false);
            playButton.text = "Loading...";

            RuntimeManager.CoreSystem.mixerSuspend();
            GameManager.SwitchTo(GameState.Playing);
            RuntimeManager.CoreSystem.mixerResume();
        }
    }
}

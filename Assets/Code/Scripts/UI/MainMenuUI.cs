using Tulip.Core;
using Tulip.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private VisualElement root;
        private Button playButton;

        private SettingsUI settingsUI;

        private void Awake()
        {
            root =  GetComponent<UIDocument>().rootVisualElement.ElementAt(0);
            playButton = root.Q<Button>("PlayButton");

            settingsUI = FindAnyObjectByType<SettingsUI>();
            InputHelper.Actions.Player.Disable();
        }

        private void OnEnable()
        {
            settingsUI.OnShow += HandleSettingsShow;
            settingsUI.OnHide += HandleSettingsHide;

            playButton.RegisterCallback<ClickEvent>(HandlePlayClicked);
        }

        private void OnDisable()
        {
            settingsUI.OnShow -= HandleSettingsShow;
            settingsUI.OnHide -= HandleSettingsHide;

            playButton.UnregisterCallback<ClickEvent>(HandlePlayClicked);
        }

        private void HandleSettingsShow() => root.visible = false;
        private void HandleSettingsHide() => root.visible = true;

        private void HandlePlayClicked(ClickEvent _) => Bootstrapper.LoadGameScene();
    }
}

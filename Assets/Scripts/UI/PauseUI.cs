using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;
using Input = Game.Player.Input;

namespace Game.UI
{
    public class PauseUI : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement root;
        private InputSystemUIInputModule inputModule;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement.ElementAt(0);
            inputModule = GetComponentInParent<InputSystemUIInputModule>();

            root.Q<Button>("ResumeButton").RegisterCallback<ClickEvent>(OnResumeClicked);
            root.Q<Button>("SettingsButton").RegisterCallback<ClickEvent>(OnSettingsClicked);
            root.Q<Button>("SaveButton").RegisterCallback<ClickEvent>(OnSaveClicked);

            Input.Actions.Player.Menu.performed += OnPaused;
            Input.Actions.UI.Cancel.performed += OnResumed;
        }

        private void Start() => SetState(false);

        private void SetState(bool value)
        {
            root.visible = value;
            inputModule.enabled = value;

            if (value)
            {
                Input.Actions.Player.Disable();
                Input.Actions.UI.Enable();
                Time.timeScale = 0;
            }
            else
            {
                Input.Actions.Player.Enable();
                Input.Actions.UI.Disable();
                Time.timeScale = 1;
            }
        }

        private void OnResumeClicked(ClickEvent evt) => SetState(false);
        private void OnSettingsClicked(ClickEvent evt) => Debug.Log("Settings clicked");

        private void OnSaveClicked(ClickEvent evt)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void OnPaused(InputAction.CallbackContext context) => SetState(true);
        private void OnResumed(InputAction.CallbackContext context) => SetState(false);

        private void OnDisable() => SetState(false);
    }
}

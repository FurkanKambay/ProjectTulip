using Game.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

            root.Q<Button>("ResumeButton").RegisterCallback<ClickEvent>(HandleResumeClicked);
            root.Q<Button>("SettingsButton").RegisterCallback<ClickEvent>(HandleSettingsClicked);
            root.Q<Button>("SaveButton").RegisterCallback<ClickEvent>(HandleSaveClicked);
            root.Q<Button>("SuperquitButton").RegisterCallback<ClickEvent>(HandleSuperquitClicked);
        }

        private void Start() => SetState(false);

        private void SetState(bool value)
        {
            root.visible = value;
            inputModule.enabled = value;

            if (value)
            {
                InputHelper.Actions.Player.Disable();
                InputHelper.Actions.UI.Enable();
                Time.timeScale = 0;
            }
            else
            {
                InputHelper.Actions.Player.Enable();
                InputHelper.Actions.UI.Disable();
                Time.timeScale = 1;
            }
        }

        private void HandleResumeClicked(ClickEvent _) => SetState(false);
        private void HandleSettingsClicked(ClickEvent _) => Debug.Log("Settings clicked");

        private void HandleSaveClicked(ClickEvent _)
        {
            SaveGame();
            ReturnToMainMenu();
        }

        private void HandleSuperquitClicked(ClickEvent _)
        {
            SaveGame();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        // TODO: save game
        private void SaveGame() => Debug.Log("Saving...");
        private void ReturnToMainMenu() => SceneManager.LoadScene("Main Menu");

        private void HandlePause(InputAction.CallbackContext context) => SetState(true);
        private void HandleResume(InputAction.CallbackContext context) => SetState(false);

        private void OnEnable()
        {
            InputHelper.Actions.Player.Menu.performed += HandlePause;
            InputHelper.Actions.UI.Cancel.performed += HandleResume;
        }

        private void OnDisable()
        {
            InputHelper.Actions.Player.Menu.performed -= HandlePause;
            InputHelper.Actions.UI.Cancel.performed -= HandleResume;

            SetState(false);
        }
    }
}

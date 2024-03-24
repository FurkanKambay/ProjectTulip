using Tulip.Data;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class DeathOverlayUI : MonoBehaviour
    {
        [SerializeField] Transform player;

        // ReSharper disable UnusedMember.Global
        [CreateProperty] public DisplayStyle OverlayDisplay => health.IsDead.ToDisplay();
        [CreateProperty] public DisplayStyle RespawnButtonDisplay => respawner.CanRespawn.ToDisplay();
        [CreateProperty] public DisplayStyle CountdownDisplay => respawner.CanRespawn.ToDisplayInverse();

        [CreateProperty] public string DeathReason => health?.LatestDeathSource?.Name;
        [CreateProperty] public int SecondsUntilRespawn => Mathf.CeilToInt(respawner.SecondsUntilRespawn);
        // ReSharper restore UnusedMember.Global

        private VisualElement root;
        private Button respawnButton;

        private IHealth health;
        private IRespawner respawner;

        private void Awake()
        {
            health = player.GetComponent<IHealth>();
            respawner = player.GetComponent<IRespawner>();
        }

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            document.enabled = true;

            root = document.rootVisualElement;
            root.dataSource = this;

            respawnButton = root.Q<Button>();
            respawnButton.RegisterCallback<ClickEvent>(HandleRespawnClicked);
        }

        private void OnDisable() => respawnButton.UnregisterCallback<ClickEvent>(HandleRespawnClicked);

        private void HandleRespawnClicked(ClickEvent _) => respawner.Respawn();
    }
}

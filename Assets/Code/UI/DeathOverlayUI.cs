using SaintsField;
using Tulip.Data;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class DeathOverlayUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] SaintsInterface<Component, IRespawner> respawner;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool IsOverlayDisplayed => health.IsDead;
        [CreateProperty] bool IsRespawnButtonDisplayed => respawner.I.CanRespawn;
        [CreateProperty] bool IsCountdownDisplayed => !respawner.I.CanRespawn;

        [CreateProperty] string DeathReason => health?.LatestDeathSource?.Name;
        [CreateProperty] int SecondsUntilRespawn => Mathf.CeilToInt(respawner.I.SecondsUntilRespawn);
        // ReSharper restore UnusedMember.Local

        private VisualElement root;
        private Button respawnButton;

        private void OnEnable()
        {
            document.enabled = true;

            root = document.rootVisualElement;
            root.dataSource = this;

            respawnButton = root.Q<Button>();
            respawnButton.RegisterCallback<ClickEvent>(HandleRespawnClicked);
        }

        private void OnDisable() => respawnButton.UnregisterCallback<ClickEvent>(HandleRespawnClicked);

        private void HandleRespawnClicked(ClickEvent _) => respawner.I.TryRespawn();
    }
}

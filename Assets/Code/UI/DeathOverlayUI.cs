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
        [SerializeField, Required] SaintsInterface<Component, IHealth> health;
        [SerializeField, Required] SaintsInterface<Component, IRespawner> respawner;

        // ReSharper disable UnusedMember.Global
        [CreateProperty] public DisplayStyle OverlayDisplay => health.I.IsDead.ToDisplay();
        [CreateProperty] public DisplayStyle RespawnButtonDisplay => respawner.I.CanRespawn.ToDisplay();
        [CreateProperty] public DisplayStyle CountdownDisplay => respawner.I.CanRespawn.ToDisplayInverse();

        [CreateProperty] public string DeathReason => health?.I.LatestDeathSource?.Name;
        [CreateProperty] public int SecondsUntilRespawn => Mathf.CeilToInt(respawner.I.SecondsUntilRespawn);
        // ReSharper restore UnusedMember.Global

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

        private void HandleRespawnClicked(ClickEvent _) => respawner.I.Respawn();
    }
}

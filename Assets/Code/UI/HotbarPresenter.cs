using System.Collections;
using SaintsField;
using Tulip.Data;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public class HotbarPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] AudioSource audioSource;
        [SerializeField, Required] SaintsInterface<Component, IPlayerHotbar> hotbar;

        [Header("Config")]
        [OverlayRichLabel("<color=grey>ms")]
        [SerializeField] int tooltipShowDuration = 1000;

        [OverlayRichLabel("<color=grey>ms")]
        [SerializeField] int tooltipSlideDuration = 100;

        // ReSharper disable NotAccessedField.Local
        [CreateProperty] ItemStack heldItem;
        [CreateProperty] ItemStack[] items;
        // ReSharper restore NotAccessedField.Local

        private VisualElement hotbarRoot;
        private VisualElement tooltipRoot;

        private void Start() => UpdateTooltip();

        private void OnEnable()
        {
            hotbarRoot = document.rootVisualElement[0];
            tooltipRoot = document.rootVisualElement[1];

            document.rootVisualElement.dataSource = this;
            UpdateHotbar();

            hotbar.I.OnModify += UpdateHotbar;
            hotbar.I.OnChangeSelection += UpdateHotbarSelection;
        }

        private void OnDisable()
        {
            hotbar.I.OnModify -= UpdateHotbar;
            hotbar.I.OnChangeSelection -= UpdateHotbarSelection;
        }

        private void UpdateHotbar() => items = hotbar.I.Items;

        private void UpdateHotbarSelection(int index)
        {
            audioSource.Play();

            for (int i = 0; i < hotbarRoot.childCount; i++)
                hotbarRoot[i].RemoveFromClassList("selected");

            hotbarRoot[index].AddToClassList("selected");
            UpdateTooltip();
        }

        private void UpdateTooltip()
        {
            ItemStack selectedStack = hotbar.I.SelectedStack;
            heldItem = selectedStack;

            // BUG: tooltip doesn't hide when amount becomes 0
            tooltipRoot.visible = selectedStack.IsValid;

            int slotIndex = hotbar.I.SelectedIndex;
            Vector3 slotPosition = hotbarRoot[slotIndex].layout.position;
            float offset = (tooltipRoot.layout.size.x / 2f) - (hotbarRoot[slotIndex].layout.size.x / 2f);

            float newPositionX = slotPosition.x - tooltipRoot.layout.position.x - offset;
            tooltipRoot.experimental.animation.Position(Vector3.right * newPositionX, tooltipSlideDuration);

            if (!tooltipRoot.visible)
                return;

            StopAllCoroutines();
            StartCoroutine(showTooltip());
            return;

            IEnumerator showTooltip()
            {
                tooltipRoot.visible = true;
                yield return new WaitForSeconds(tooltipShowDuration / 1000f);
                tooltipRoot.visible = false;
            }
        }
    }
}
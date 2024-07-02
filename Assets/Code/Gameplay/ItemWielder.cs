using System;
using SaintsField;
using Tulip.Core.Unity;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class ItemWielder : MonoBehaviour, IItemWielder
    {
        public event Action<Usable, Vector3> OnSwing;
        public event Action<Usable> OnReady;

        public Item CurrentItem => HotbarSelectedItem ? HotbarSelectedItem : equippedItem;
        private Item HotbarSelectedItem => inventory.I?.HotbarSelected?.Item;

        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] SaintsInterface<Component, IWielderBrain> brain;
        [SerializeField] SaintsInterface<Component, IInventory> inventory;
        [SerializeField, Required] SpriteRenderer itemRenderer;

        [Header("Config")]
        [SerializeField] Usable equippedItem;

        [Header("Item Visuals")]
        [SerializeField] float itemStowDelay = 2f;
        [SerializeField] float readyAngle = -10f;
        [SerializeField] float swingAngle = -90f;

        private Transform itemPivot;
        private Transform itemVisual;

        private Usable itemToSwing;
        private ItemSwingState itemState;
        private Vector3 rendererScale;
        private float timeSinceLastUse;

        private void Awake()
        {
            itemPivot = itemRenderer.transform.parent;
            itemVisual = itemRenderer.transform;

            itemVisual.localEulerAngles = Vector3.forward * readyAngle;
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            bool shouldShowItem = timeSinceLastUse < itemStowDelay || brain.I.WantsToUse;
            itemVisual.localScale = shouldShowItem ? rendererScale : Vector3.zero;
            UpdateReadyItemVisual();

            if (!itemToSwing)
            {
                itemToSwing = CurrentItem as Usable;
                itemState = ItemSwingState.Ready;
                return;
            }

            (float targetAngle, float decay) = itemState switch
            {
                ItemSwingState.Swinging => (swingAngle, itemToSwing.SwingTime),
                _ => (readyAngle, itemToSwing.SwingTime * 2f)
            };

            SetAngle(targetAngle, decay);
            float deltaAngle = Mathf.DeltaAngle(itemVisual.localEulerAngles.z, targetAngle);

            if (Mathf.Abs(deltaAngle) > 0.1f) return;
            // reached the current target angle

            switch (itemState)
            {
                case ItemSwingState.Ready when brain.I.WantsToUse:
                    itemToSwing = CurrentItem as Usable;
                    if (!itemToSwing || timeSinceLastUse <= itemToSwing.Cooldown)
                        break;

                    itemState = ItemSwingState.Swinging;
                    timeSinceLastUse = 0f;
                    break;

                case ItemSwingState.Swinging:
                    itemState = ItemSwingState.Resetting;
                    OnSwing?.Invoke(itemToSwing, brain.I.AimPosition);
                    break;

                default:
                case ItemSwingState.Resetting:
                    // Can't swap item mid-swing
                    GetItemReady();
                    break;
            }
        }

        private void GetItemReady()
        {
            itemState = ItemSwingState.Ready;
            itemToSwing = CurrentItem as Usable;
            if (itemToSwing)
                OnReady?.Invoke(itemToSwing);
        }

        private void SetAngle(float target, float decay)
        {
            float angle = itemVisual.localEulerAngles.z;
            float delta = Mathf.DeltaAngle(angle, target);
            float targetAngle = angle.ExpDecay(angle + delta, decay, Time.deltaTime);
            itemVisual.localEulerAngles = Vector3.forward * targetAngle;
        }

        private void UpdateReadyItemVisual()
        {
            // Don't rotate item in the middle of swinging
            if (itemState != ItemSwingState.Ready) return;

            Vector2 pivotPosition = itemPivot.position;
            Vector2 aimDirection = brain.I.AimPosition - pivotPosition;
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            bool isLeft = aimAngle is < -90 or > 90;
            itemPivot.localScale = new Vector3(1, isLeft ? -1 : 1, 1);
            itemPivot.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }

        private void UpdateItemSprite(int _)
        {
            Item item = CurrentItem;

            float scale = item ? item.IconScale : 1;
            Color tint = Color.white;

            if (item is WorldTile tile)
            {
                scale = item.IconScale * 0.8f;
                tint = tile.color;
            }

            itemRenderer.sprite = item ? item.Icon : null;
            itemRenderer.color = tint;

            rendererScale = Vector2.one * scale;
            itemRenderer.transform.localScale = rendererScale;
        }

        private void HandleDie(DamageEventArgs _) => itemRenderer.enabled = false;
        private void HandleRevived(IHealth source) => itemRenderer.enabled = true;

        private void OnEnable()
        {
            UpdateItemSprite(0);

            health.OnDie += HandleDie;
            health.OnRevive += HandleRevived;

            if (inventory.I == null) return;
            inventory.I.OnChangeHotbarSelection += UpdateItemSprite;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;

            if (inventory.I == null) return;
            inventory.I.OnChangeHotbarSelection -= UpdateItemSprite;
        }

        private enum ItemSwingState
        {
            Ready,
            Swinging,
            Resetting
        }
    }
}

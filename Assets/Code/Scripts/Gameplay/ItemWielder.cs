using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using Tulip.Gameplay.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Gameplay
{
    [RequireComponent(typeof(IWielderBrain))]
    public class ItemWielder : MonoBehaviour, IItemWielder
    {
        public event Action<Usable> OnCharge;
        public event Action<Usable, ItemSwingDirection> OnSwing;
        public event Action<Usable> OnReady;

        public Item CurrentItem => HotbarSelectedItem ? HotbarSelectedItem : equippedItem;
        private Item HotbarSelectedItem => inventory?.HotbarSelected?.Item;

        [SerializeField] Usable equippedItem;
        [SerializeField] Transform itemPivot;

        [Header("Item Visuals")]
        [SerializeField] float itemStowDelay = 2f;
        [SerializeField] float itemDrawStowDuration = 0.5f;
        [SerializeField] float readyAngle = -10;
        [SerializeField] float chargeAngle = 45f;
        [SerializeField] float swingAngle = -90f;

        private IInventory inventory;
        private IWielderBrain brain;
        private IHealth health;
        private Transform itemVisual;
        private SpriteRenderer itemRenderer;

        private Usable itemToSwing;
        private ItemSwingState itemState;
        private ItemSwingDirection intendedSwingDirection;
        private ItemSwingDirection itemVisualState;
        private float timeSinceLastUse;
        private TweenerCore<Quaternion, Vector3, QuaternionOptions> currentTween;

        private void Awake()
        {
            Assert.IsNotNull(itemPivot);

            inventory = GetComponent<IInventory>();
            brain = GetComponent<IWielderBrain>();
            health = GetComponent<IHealth>();
            itemRenderer = itemPivot.GetComponentInChildren<SpriteRenderer>();
            itemVisual = itemRenderer.transform;

            itemVisual.localEulerAngles = Vector3.forward * readyAngle;
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            float deltaToTarget = brain.AimPosition.x - transform.position.x;
            intendedSwingDirection = deltaToTarget switch
            {
                < 0 => ItemSwingDirection.Left,
                > 0 => ItemSwingDirection.Right,
                _ => ItemSwingDirection.None
                // TODO: implement up/down swing
            };

            bool shouldShowItem = timeSinceLastUse < itemStowDelay || brain.WantsToUse;
            UpdateItemVisual(shouldShowItem ? intendedSwingDirection : ItemSwingDirection.None);

            if (brain.WantsToUse)
                ChargeAndSwing(intendedSwingDirection);
        }

        private void ChargeAndSwing(ItemSwingDirection swingDirection)
        {
            if (itemState != ItemSwingState.Ready) return;

            itemToSwing = CurrentItem as Usable;
            if (itemToSwing == null || timeSinceLastUse <= itemToSwing.Cooldown) return;

            itemToSwing = CurrentItem as Usable;
            timeSinceLastUse = 0f;
            DoCharge(onComplete: () => DoSwing(swingDirection));
        }

        private void DoCharge(Action onComplete = null)
        {
            itemState = ItemSwingState.Charging;
            currentTween = itemVisual
                .DOLocalRotate(Vector3.forward * chargeAngle, itemToSwing.ChargeTime)
                .OnComplete(() =>
                {
                    itemState = ItemSwingState.Charged;
                    OnCharge?.Invoke(itemToSwing);
                    onComplete?.Invoke();
                });
        }

        private void DoSwing(ItemSwingDirection swingDirection, Action onComplete = null)
        {
            itemState = ItemSwingState.Swinging;
            currentTween = itemVisual
                .DOLocalRotate(Vector3.forward * swingAngle, itemToSwing.SwingTime)
                .OnComplete(() =>
                {
                    OnSwing?.Invoke(itemToSwing, swingDirection);
                    onComplete?.Invoke();
                    ResetState();
                });
        }

        private void ResetState()
        {
            if (itemState == ItemSwingState.Ready) return;

            itemState = ItemSwingState.Resetting;
            currentTween = itemVisual
                .DOLocalRotate(Vector3.forward * readyAngle, itemToSwing.SwingTime)
                .OnComplete(() =>
                {
                    itemState = ItemSwingState.Ready;
                    itemToSwing = CurrentItem as Usable;

                    if (itemToSwing != null)
                        OnReady?.Invoke(itemToSwing);
                });
        }

        private void UpdateItemVisual(ItemSwingDirection swingDirection)
        {
            if (itemVisualState == swingDirection) return;

            // Don't update item visuals in the middle of swinging
            if (itemState != ItemSwingState.Ready) return;

            var endValue = new Vector3(swingDirection.ToVector2().x, 1, 1);
            itemPivot.DOScale(endValue, itemDrawStowDuration);

            itemVisualState = swingDirection;
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
            itemRenderer.transform.localScale = Vector2.one * scale;
        }

        private void HandleDie(DamageEventArgs _)
        {
            currentTween.Kill();
            itemRenderer.enabled = false;
        }

        private void HandleRevived(IHealth source) => itemRenderer.enabled = true;

        private void OnEnable()
        {
            UpdateItemSprite(0);

            health.OnDie += HandleDie;
            health.OnRevive += HandleRevived;

            if (inventory == null) return;
            inventory.OnChangeHotbarSelection += UpdateItemSprite;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;

            if (inventory == null) return;
            inventory.OnChangeHotbarSelection -= UpdateItemSprite;
        }

        private enum ItemSwingState
        {
            Ready,
            Charging,
            Charged,
            Swinging,
            Resetting
        }
    }
}

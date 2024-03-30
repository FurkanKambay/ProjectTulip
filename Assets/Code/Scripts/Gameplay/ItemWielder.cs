using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Gameplay
{
    [RequireComponent(typeof(IWielderBrain))]
    public class ItemWielder : MonoBehaviour, IItemWielder
    {
        public event Action<Usable> OnCharge;
        public event Action<Usable, Vector3> OnSwing;
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
        private bool isItemVisible;
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

            bool shouldShowItem = timeSinceLastUse < itemStowDelay || brain.WantsToUse;
            UpdateItemVisual(shouldShowItem);

            if (brain.WantsToUse)
                ChargeAndSwing();
        }

        private void ChargeAndSwing()
        {
            if (itemState != ItemSwingState.Ready) return;

            itemToSwing = CurrentItem as Usable;
            if (itemToSwing == null || timeSinceLastUse <= itemToSwing.Cooldown) return;

            itemToSwing = CurrentItem as Usable;
            timeSinceLastUse = 0f;
            DoCharge(onComplete: () => DoSwing());
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

        private void DoSwing(Action onComplete = null)
        {
            itemState = ItemSwingState.Swinging;
            currentTween = itemVisual
                .DOLocalRotate(Vector3.forward * swingAngle, itemToSwing.SwingTime)
                .OnComplete(() =>
                {
                    OnSwing?.Invoke(itemToSwing, brain.AimPosition);
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

        private void UpdateItemVisual(bool shouldShow)
        {
            if (isItemVisible != shouldShow)
            {
                itemVisual.DOScale(shouldShow ? Vector3.one : Vector3.zero, itemDrawStowDuration);
                isItemVisible = shouldShow;
            }

            // Don't rotate item in the middle of swinging
            if (itemState != ItemSwingState.Ready) return;

            Vector3 pivotPosition = itemPivot.position;
            Vector3 aimDirection =  brain.AimPosition - pivotPosition;
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

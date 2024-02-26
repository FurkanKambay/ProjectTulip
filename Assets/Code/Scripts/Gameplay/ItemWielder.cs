using System;
using DG.Tweening;
using Tulip.Data.Items;
using Tulip.Gameplay.Extensions;
using Tulip.Input;
using Tulip.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Gameplay
{
    [RequireComponent(typeof(Inventory))]
    public class ItemWielder : MonoBehaviour
    {
        public event Action<Usable> OnCharge;
        public event Action<Usable, ItemSwingDirection> OnSwing;
        public event Action<Usable> OnReady;

        public Usable HotbarItem => inventory.HotbarSelected?.Item as Usable;

        [SerializeField] Transform itemPivot;

        [Header("Item Visuals")]
        [SerializeField] float itemStowDelay = 2f;
        [SerializeField] float itemDrawStowDuration = 0.5f;
        [SerializeField] float readyAngle = -10;
        [SerializeField] float chargeAngle = 45f;
        [SerializeField] float swingAngle = -90f;

        private Inventory inventory;
        private Transform itemVisual;
        private SpriteRenderer itemRenderer;
        private Camera mainCamera;

        private Usable itemToSwing;
        private ItemSwingState itemState;
        private ItemSwingDirection itemVisualState;
        private float timeSinceLastUse;

        private void Awake()
        {
            Assert.IsNotNull(itemPivot);

            inventory = GetComponent<Inventory>();
            itemRenderer = itemPivot.GetComponentInChildren<SpriteRenderer>();
            itemVisual = itemRenderer.transform;
            mainCamera = Camera.main;

            itemVisual.localEulerAngles = Vector3.forward * readyAngle;
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(InputHelper.Instance.MouseScreenPoint);
            float deltaToMouse = mouseWorld.x - transform.position.x;

            // TODO: implement up/down swing
            ItemSwingDirection intendedSwingDirection = deltaToMouse < 0
                ? ItemSwingDirection.Left
                : ItemSwingDirection.Right;

            bool shouldShowItem = timeSinceLastUse < itemStowDelay || InputHelper.Actions.Player.Use.inProgress;
            UpdateItemVisual(shouldShowItem ? intendedSwingDirection : ItemSwingDirection.None);

            if (InputHelper.Actions.Player.Use.inProgress)
                ChargeAndSwing(intendedSwingDirection);
        }

        private void ChargeAndSwing(ItemSwingDirection swingDirection)
        {
            if (itemState != ItemSwingState.Ready) return;

            itemToSwing = HotbarItem;
            if (itemToSwing == null || timeSinceLastUse <= itemToSwing.Cooldown) return;

            itemToSwing = HotbarItem;
            timeSinceLastUse = 0f;
            DoCharge(onComplete: () => DoSwing(swingDirection));
        }

        private void DoCharge(Action onComplete = null)
        {
            itemState = ItemSwingState.Charging;
            itemVisual.DOLocalRotate(Vector3.forward * chargeAngle, itemToSwing.ChargeTime)
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
            itemVisual.DOLocalRotate(Vector3.forward * swingAngle, itemToSwing.SwingTime)
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
            itemVisual.DOLocalRotate(Vector3.forward * readyAngle, itemToSwing.SwingTime)
                .OnComplete(() =>
                {
                    itemState = ItemSwingState.Ready;
                    itemToSwing = HotbarItem;

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
            Item item = inventory.HotbarSelected?.Item;

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

        private void OnEnable() => inventory.OnChangeHotbarSelection += UpdateItemSprite;
        private void OnDisable() => inventory.OnChangeHotbarSelection -= UpdateItemSprite;

        private enum ItemSwingState
        {
            Ready,
            Charging,
            Charged,
            Swinging,
            Resetting
        }
    }

    public enum ItemSwingDirection
    {
        None,
        Left,
        Right,
        Down,
        Up
    }
}

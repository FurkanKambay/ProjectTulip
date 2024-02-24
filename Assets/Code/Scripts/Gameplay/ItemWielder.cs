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
        private ItemSwingState state;
        private float timeSinceLastUse;

        private void ChargeAndSwing(ItemSwingDirection swingDirection)
        {
            if (state != ItemSwingState.Ready) return;

            itemToSwing = HotbarItem;
            if (itemToSwing == null || timeSinceLastUse <= itemToSwing.Cooldown) return;

            itemToSwing = HotbarItem;
            timeSinceLastUse = 0f;
            DoCharge(onComplete: () => DoSwing(swingDirection));
        }

        private void DoCharge(Action onComplete = null)
        {
            state = ItemSwingState.Charging;
            itemVisual.DOLocalRotate(Vector3.forward * chargeAngle, itemToSwing.ChargeTime)
                .OnComplete(() =>
                {
                    state = ItemSwingState.Charged;
                    OnCharge?.Invoke(itemToSwing);
                    onComplete?.Invoke();
                });
        }

        private void DoSwing(ItemSwingDirection swingDirection, Action onComplete = null)
        {
            state = ItemSwingState.Swinging;
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
            if (state == ItemSwingState.Ready) return;

            state = ItemSwingState.Resetting;
            itemVisual.DOLocalRotate(Vector3.forward * readyAngle, itemToSwing.SwingTime)
                .OnComplete(() =>
                {
                    state = ItemSwingState.Ready;
                    itemToSwing = HotbarItem;

                    if (itemToSwing != null)
                        OnReady?.Invoke(itemToSwing);
                });
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            bool shouldShowItem = timeSinceLastUse < itemStowDelay || InputHelper.Actions.Player.Use.inProgress;
            Vector3 targetScale = itemPivot.localScale;

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(InputHelper.Instance.MouseScreenPoint);
            Vector2 deltaToMouse = mouseWorld - transform.position;

            // TODO: implement up/down swing
            ItemSwingDirection swingDirection = deltaToMouse.x < 0
                ? ItemSwingDirection.Left
                : ItemSwingDirection.Right;

            // Only flip around when not charging/swinging
            if (state == ItemSwingState.Ready)
                targetScale = new Vector3(swingDirection.ToVector2().x, 1, 1);

            targetScale = shouldShowItem ? targetScale : Vector3.zero;

            // TODO: don't do this in Update()
            if ((itemPivot.localScale - targetScale).sqrMagnitude > 0.01f)
                itemPivot.DOScale(targetScale, itemDrawStowDuration);

            if (InputHelper.Actions.Player.Use.inProgress)
                ChargeAndSwing(swingDirection);
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

        private void Awake()
        {
            Assert.IsNotNull(itemPivot);

            inventory = GetComponent<Inventory>();
            itemRenderer = itemPivot.GetComponentInChildren<SpriteRenderer>();
            itemVisual = itemRenderer.transform;
            mainCamera = Camera.main;

            itemVisual.localEulerAngles = Vector3.forward * readyAngle;
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
        Left,
        Right,
        Down,
        Up
    }
}

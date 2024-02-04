using System;
using DG.Tweening;
using Game.Data.Interfaces;
using Game.Data.Tiles;
using Game.Gameplay.Extensions;
using Game.Input;
using Game.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Gameplay
{
    [RequireComponent(typeof(Inventory))]
    public class ItemWielder : MonoBehaviour
    {
        public event Action<IUsable> OnCharge;
        public event Action<IUsable, ItemSwingDirection> OnSwing;
        public event Action<IUsable> OnReady;

        public IUsable HotbarItem => inventory.HotbarSelected?.Item as IUsable;

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

        private IUsable itemToSwing;
        private ItemSwingState state;
        private float timeSinceLastUse;

        private void ChargeAndSwing(ItemSwingDirection swingDirection)
        {
            itemToSwing ??= HotbarItem;

            if (itemToSwing == null || timeSinceLastUse <= itemToSwing.Cooldown) return;
            if (state != ItemSwingState.Ready) return;

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

            Vector2 deltaToMouse = InputHelper.Instance.MouseWorldPoint - (Vector2)transform.position;

            // TODO: implement up/down swing
            ItemSwingDirection swingDirection = deltaToMouse.x < 0
                ? ItemSwingDirection.Left
                : ItemSwingDirection.Right;

            // Only flip around when not charging/swinging
            if (state == ItemSwingState.Ready)
                targetScale = new Vector3(swingDirection.ToVector2().x, 1, 1);

            targetScale = shouldShowItem ? targetScale : Vector3.zero;

            if (itemPivot.localScale != targetScale)
                itemPivot.DOScale(targetScale, itemDrawStowDuration);

            if (InputHelper.Actions.Player.Use.inProgress)
                ChargeAndSwing(swingDirection);
        }

        private void UpdateItemSprite(int _)
        {
            IItem item = inventory.HotbarSelected?.Item;

            float scale = item?.IconScale ?? 1;
            Color tint = Color.white;

            if (item is WorldTile tile)
            {
                scale = item.IconScale * 0.8f;
                tint = tile.color;
            }

            itemRenderer.sprite = item?.Icon;
            itemRenderer.color = tint;
            itemRenderer.transform.localScale = Vector2.one * scale;
        }

        private void Awake()
        {
            Assert.IsNotNull(itemPivot);

            inventory = GetComponent<Inventory>();
            itemRenderer = itemPivot.GetComponentInChildren<SpriteRenderer>();
            itemVisual = itemRenderer.transform;

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

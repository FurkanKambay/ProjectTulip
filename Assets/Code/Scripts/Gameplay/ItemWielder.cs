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
        public IUsable Current => inventory.HotbarSelected?.Item as IUsable;

        [SerializeField] Transform itemPivot;
        [SerializeField] float hideItemDelay = 2f;

        [Header("Item Swing")]
        [SerializeField] float readyAngle = -10;

        [SerializeField] float chargeAngle = 45f;
        [SerializeField] float swingAngle = -90f;

        [Header("Item Animations")]
        [SerializeField] float chargeDuration = 0.2f;

        [SerializeField] float swingDuration = 0.1f;
        [SerializeField] float itemShowHideDuration = 0.5f;

        private ItemSwingState state;
        private float timeSinceLastUse;

        private Inventory inventory;
        private Transform itemVisual;
        private SpriteRenderer itemRenderer;

        public event Action<IUsable> OnCharge;
        public event Action<IUsable, ItemSwingDirection> OnSwing;
        public event Action<IUsable> OnReady;

        public void ChargeAndSwing(ItemSwingDirection swingDirection)
        {
            if (timeSinceLastUse <= Current?.Cooldown) return;
            if (state != ItemSwingState.Ready) return;
            timeSinceLastUse = 0f;

            DoCharge(() => DoSwing(swingDirection));
        }

        private void DoCharge(Action onComplete = null)
        {
            state = ItemSwingState.Charging;
            itemVisual.DOLocalRotate(Vector3.forward * chargeAngle, chargeDuration)
                .OnComplete(() =>
                {
                    state = ItemSwingState.Charged;
                    OnCharge?.Invoke(Current);
                    onComplete?.Invoke();
                });
        }

        private void DoSwing(ItemSwingDirection swingDirection, Action onComplete = null)
        {
            state = ItemSwingState.Swinging;
            itemVisual.DOLocalRotate(Vector3.forward * swingAngle, swingDuration)
                .OnComplete(() =>
                {
                    OnSwing?.Invoke(Current, swingDirection);
                    onComplete?.Invoke();
                    ResetState();
                });
        }

        private void ResetState()
        {
            if (state == ItemSwingState.Ready) return;

            state = ItemSwingState.Resetting;
            itemVisual.DOLocalRotate(Vector3.forward * readyAngle, swingDuration)
                .OnComplete(() =>
                {
                    state = ItemSwingState.Ready;
                    OnReady?.Invoke(Current);
                });
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            bool shouldShowItem = timeSinceLastUse < hideItemDelay || InputHelper.Actions.Player.Use.inProgress;
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
                itemPivot.DOScale(targetScale, itemShowHideDuration);

            if (InputHelper.Actions.Player.Use.inProgress)
                ChargeAndSwing(swingDirection);
        }

        private void UpdateItemSprite(int _)
        {
            IItem item = inventory.HotbarSelected?.Item;

            float scale = item?.IconScale ?? 1;
            Color tint = Color.white;

            if (item is { Type: ItemType.Block or ItemType.Wall })
            {
                scale = item.IconScale * 0.8f;
                tint = ((BlockTile)item).color;
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

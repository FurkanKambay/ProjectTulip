using DG.Tweening;
using Game.Data.Interfaces;
using Game.Data.Tiles;
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

        [SerializeField] private Transform itemPivot;

        [SerializeField] private float hideItemDelay = 2f;

        [SerializeField] private float readyAngle = -10;

        [SerializeField] private float chargeAngle = 45f;
        [SerializeField] private float swingAngle = -90f;

        [SerializeField] private float chargeDuration = 0.2f;
        [SerializeField] private float swingDuration = 0.1f;

        private ItemSwingState state;
        private float timeSinceLastUse;

        private Inventory inventory;
        private Transform itemVisual;
        private SpriteRenderer itemRenderer;

        public void ChargeAndSwing(TweenCallback onCharge = null, TweenCallback onSwing = null)
        {
            if (timeSinceLastUse <= Current?.Cooldown) return;
            if (state != ItemSwingState.Ready) return;
            timeSinceLastUse = 0f;

            DoCharge(() =>
            {
                onCharge?.Invoke();
                DoSwing(onSwing);
            });
        }

        private void DoCharge(TweenCallback onComplete)
        {
            state = ItemSwingState.Charging;
            itemVisual.DOLocalRotate(Vector3.forward * chargeAngle, chargeDuration)
                .OnComplete(() =>
                {
                    state = ItemSwingState.Charged;
                    onComplete?.Invoke();
                });
        }

        private void DoSwing(TweenCallback onComplete)
        {
            state = ItemSwingState.Swinging;
            itemVisual.DOLocalRotate(Vector3.forward * swingAngle, swingDuration)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    ResetState();
                });
        }

        private void ResetState()
        {
            state = ItemSwingState.Resetting;
            itemVisual.DOLocalRotate(Vector3.forward * readyAngle, swingDuration)
                .OnComplete(() => state = ItemSwingState.Ready);
        }

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;

            itemRenderer.enabled = timeSinceLastUse < hideItemDelay;

            if (state == ItemSwingState.Ready)
            {
                Vector2 deltaToMouse = InputHelper.Instance.MouseWorldPoint - (Vector2)transform.position;
                float aimDirectionSign = Mathf.Sign(deltaToMouse.x);
                itemPivot.localScale = new Vector3(aimDirectionSign < 0 ? -1 : 1, 1, 1);
            }

            if (InputHelper.Actions.Player.Use.inProgress)
                ChargeAndSwing();
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
            inventory = GetComponent<Inventory>();
            itemRenderer = itemPivot.GetComponentInChildren<SpriteRenderer>();
            itemVisual = itemRenderer.transform;
            Assert.IsNotNull(itemPivot);

            itemVisual.localEulerAngles = Vector3.forward * readyAngle;
        }

        private void OnEnable() => inventory.HotbarSelectionChanged += UpdateItemSprite;
        private void OnDisable() => inventory.HotbarSelectionChanged -= UpdateItemSprite;
    }

    internal enum ItemSwingState
    {
        Ready,
        Charging,
        Charged,
        Swinging,
        Resetting
    }
}

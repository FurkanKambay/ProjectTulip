using System;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using Tulip.Player;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class ItemWielder : MonoBehaviour, IItemWielder
    {
        public event Action<ItemStack, Vector3> OnSwing;
        public event Action<ItemStack> OnReady;

        public ItemStack CurrentStack => HotbarItem.IsValid ? HotbarItem : fallbackStack;
        private ItemStack HotbarItem => hotbar ? hotbar.SelectedStack : default;

        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] SaintsInterface<Component, IWielderBrain> brain;
        [SerializeField] PlayerHotbar hotbar;
        [SerializeField, Required] SpriteRenderer itemRenderer;

        [Header("Config")]
        [SerializeField] ItemStack fallbackStack;
        [SerializeField] float itemStowDelay = 2f;

        private Transform itemPivot;
        private Transform itemVisual;

        // state
        private ItemStack handStack;
        private float timeSinceLastUse;
        private ItemSwingState swingState;
        private Vector3 rendererScale;

        // state: phase (motion)
        private bool wantsToSwapItems;
        private int phaseIndex;
        private MotionState motion;

        private void Awake()
        {
            itemVisual = itemRenderer.transform;
            itemPivot = itemVisual.parent;
        }

        private void Start() => RefreshItem();

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;
            TickSwingState();
        }

        private void TickSwingState()
        {
            if (!handStack.item || handStack.item is not Usable usable)
                return;

            bool wantsToUse = brain.I.WantsToUse && !wantsToSwapItems;
            ItemSwingType swingType = usable.SwingType;
            UsePhase phase = swingType.Phases.Length > 0 ? swingType.Phases[phaseIndex] : default;

            if (!phase.preventAim || swingState == ItemSwingState.Ready)
                AimItem();

            switch (swingState)
            {
                case ItemSwingState.Ready:
                    // Only update sprite when ready to swing again
                    bool shouldDisplay = wantsToUse || timeSinceLastUse < itemStowDelay;
                    itemVisual.localScale = shouldDisplay ? rendererScale : Vector3.zero;

                    if (wantsToUse && timeSinceLastUse > usable.Cooldown)
                    {
                        SwitchState(ItemSwingState.Swinging);
                        timeSinceLastUse = 0f;
                    }

                    break;
                case ItemSwingState.Swinging:
                    // cancel the swing if needed
                    if (phase.isCancelable && !wantsToUse)
                    {
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // proceed normally (not interrupting the motion)
                    TickMotionLerp();

                    // we're still Lerping, so we skip to the next tick
                    if (!IsMotionDone())
                        break;

                    // we reached the target angle. move to next phase or reset after final phase

                    // if no phases, hit and reset swing
                    if (swingType.Phases.Length == 0)
                    {
                        OnSwing?.Invoke(handStack, brain.I.AimPosition);
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // hit if we need to before checking for final exit
                    if (phase.shouldHit)
                        OnSwing?.Invoke(handStack, brain.I.AimPosition);

                    bool isFinalPhase = phaseIndex == swingType.Phases.Length - 1;
                    bool shouldReset = !wantsToUse || !swingType.Loop;

                    if (isFinalPhase && shouldReset)
                    {
                        SwitchState(ItemSwingState.Resetting);
                        break;
                    }

                    // still not ending so next phase. keeps swinging without resetting
                    // looping: start from phase 0 again

                    // "reset" to phase 0 with `phase.XDuration`, NOT `swingType.ResetXDuration`
                    phaseIndex = isFinalPhase ? 0 : phaseIndex + 1;

                    // this belongs in a state machine. Motion is a sub-state machine of Swing
                    SetMotionToPhase();

                    break;
                case ItemSwingState.Resetting:
                    TickMotionLerp();

                    if (IsMotionDone())
                        SwitchState(ItemSwingState.Ready);

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(swingState));
            }
        }

        private void SwitchState(ItemSwingState state)
        {
            if (state == swingState)
                return;

            if (!handStack.IsValid || handStack.item is not Usable)
            {
                swingState = ItemSwingState.Ready;
                return;
            }

            swingState = state;

            switch (state)
            {
                case ItemSwingState.Ready:
                    // Only swap items when reset and ready
                    wantsToSwapItems = false;
                    RefreshItem();

                    OnReady?.Invoke(handStack);
                    break;
                case ItemSwingState.Swinging:
                    phaseIndex = 0;
                    SetMotionToPhase();
                    break;
                case ItemSwingState.Resetting:
                    SetMotionToReady();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(state));
            }
        }

        private void RefreshItem()
        {
            handStack = CurrentStack;
            UpdateItemSprite();

            phaseIndex = 0;
            ResetMotionStart();

            if (handStack.item is Usable usable)
                SetSpriteTransformInstant(usable.SwingType.ReadyPosition, usable.SwingType.ReadyAngle);
        }

#region Motion Helpers

        private void SetMotionToPhase()
        {
            if (handStack.item is not Usable usable)
                return;

            ItemSwingType swingType = usable.SwingType;
            UsePhase phase = swingType.Phases.Length > 0 ? swingType.Phases[phaseIndex] : default;

            ResetMotionStart();
            motion.EndPosition = swingType.ReadyPosition + phase.moveDelta;
            motion.EndAngle = swingType.ReadyAngle + phase.turnDelta;
            motion.MoveDuration = phase.moveDuration;
            motion.TurnDuration = phase.turnDuration;
        }

        private void SetMotionToReady()
        {
            if (handStack.item is not Usable usable)
                return;

            ItemSwingType swingType = usable.SwingType;

            ResetMotionStart();
            motion.EndPosition = swingType.ReadyPosition;
            motion.EndAngle = swingType.ReadyAngle;
            motion.MoveDuration = swingType.ResetMoveDuration;
            motion.TurnDuration = swingType.ResetTurnDuration;
        }

        private void ResetMotionStart()
        {
            motion = default;
            motion.StartPosition = itemVisual.localPosition;
            motion.StartAngle = itemVisual.localEulerAngles.z;
            // need to reset lerp values too here
            motion.LerpMove = 0;
            motion.LerpTurn = 0;
        }

        private void TickMotionLerp()
        {
            motion.LerpMove = motion.MoveDuration <= 0 || motion.LerpMove >= 1 ? 1
                : Mathf.MoveTowards(motion.LerpMove, 1, Time.deltaTime / motion.MoveDuration);

            motion.LerpTurn = motion.TurnDuration <= 0 || motion.LerpTurn >= 1 ? 1
                : Mathf.MoveTowards(motion.LerpTurn, 1, Time.deltaTime / motion.TurnDuration);

            SetSpriteTransformInstant(
                Vector2.Lerp(motion.StartPosition, motion.EndPosition, motion.LerpMove),
                Mathf.LerpAngle(motion.StartAngle, motion.EndAngle, motion.LerpTurn)
            );
        }

        private bool IsMotionDone() => Mathf.Approximately(motion.LerpMove, 1) && Mathf.Approximately(motion.LerpTurn, 1);

#endregion

        private void SetSpriteTransformInstant(Vector2 targetPosition, float targetAngle)
        {
            itemVisual.localPosition = targetPosition;
            itemVisual.localEulerAngles = Vector3.forward * targetAngle;
        }

        private void AimItem()
        {
            Vector2 aimDirection = brain.I.AimPosition - (Vector2)itemPivot.position;
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            bool isLeft = aimAngle is < -90 or > 90;

            itemPivot.localScale = new Vector3(1, isLeft ? -1 : 1, 1);
            itemPivot.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        }

        private void UpdateItemSprite()
        {
            Item item = handStack.item;

            float scale = item ? item.IconScale : 1;
            Color tint = Color.white;

            if (item is WorldTile tile)
            {
                scale = item.IconScale * 0.8f;
                tint = tile.Color;
            }

            rendererScale = Vector2.one * scale;

            itemRenderer.transform.localScale = rendererScale;
            itemRenderer.sprite = item ? item.Icon : null;
            itemRenderer.color = tint;
        }

        private void HandleDie(DamageEventArgs _) => itemRenderer.enabled = false;
        private void HandleRevived(IHealth source) => itemRenderer.enabled = true;

        private void HandleHotbarSelectionChanged(int _)
        {
            if (swingState != ItemSwingState.Ready)
            {
                wantsToSwapItems = true;
                return;
            }

            RefreshItem();
        }

        private void OnEnable()
        {
            UpdateItemSprite();

            health.OnDie += HandleDie;
            health.OnRevive += HandleRevived;

            if (hotbar)
                hotbar.OnChangeSelection += HandleHotbarSelectionChanged;
        }

        private void OnDisable()
        {
            health.OnDie -= HandleDie;
            health.OnRevive -= HandleRevived;

            if (hotbar)
                hotbar.OnChangeSelection -= HandleHotbarSelectionChanged;
        }

        private struct MotionState
        {
            public Vector2 StartPosition;
            public Vector2 EndPosition;
            public float StartAngle;
            public float EndAngle;

            public float MoveDuration;
            public float TurnDuration;
            public float LerpMove;
            public float LerpTurn;
        }

        private enum ItemSwingState
        {
            Ready,
            Swinging,
            Resetting
        }
    }
}

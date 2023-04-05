using System;
using UnityEngine;

namespace Game.CharacterController
{
    public class AutoStepper : MonoBehaviour
    {
        [Header("Step")]
        [SerializeField] private float stepHeight = 1f;
        [SerializeField] private float stepWidth;
        [SerializeField] private Vector2 stepSpeed = Vector2.one;

        [Header("Thresholds")]
        [SerializeField] private float velocityThreshold = .1f;
        [SerializeField] private float range = .5f;

        [SerializeField] private Vector3 offset = (Vector3.up * .5f);

        private World world;
        private PlayerMovement movement;
        private GroundChecker ground;
        private Rigidbody2D body;

        private Vector2? targetPosition;

        private void Awake()
        {
            world = World.Instance;
            movement = GetComponent<PlayerMovement>();
            ground = GetComponent<GroundChecker>();
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            StepType step = CanStepUp();

            if (step == StepType.None)
            {
                targetPosition = null;
                return;
            }

            if (!ground.IsGrounded) return;

            Vector2 position = body.position;
            float width = step == StepType.Left ? -stepWidth : stepWidth;
            targetPosition = new Vector2(position.x + width, position.y + stepHeight);
        }

        private void FixedUpdate()
        {
            if (!targetPosition.HasValue) return;
            Vector2 position = body.position;
            Vector2 moveDirection = (targetPosition.Value - position).normalized;
            Vector2 moveDelta = moveDirection * stepSpeed * Time.deltaTime;
            body.MovePosition(position + moveDelta);
        }

        private StepType CanStepUp()
        {
            float velocity = movement.DesiredVelocity.x;
            if (Mathf.Abs(velocity) < velocityThreshold)
                return StepType.None;

            Vector2 direction = Vector2.right * Math.Sign(velocity);
            Vector2 hotspot = transform.position + offset;

            RaycastHit2D hit = Physics2D.Raycast(
                hotspot, direction, range,
                LayerMask.GetMask("World"));

            Vector2 hitPoint = hit.point - (hit.normal * 0.1f);
            Vector3Int cell1 = world.WorldToCell(hitPoint) + Vector3Int.up;
            Vector3Int cell2 = cell1 + Vector3Int.up;
            Vector3Int cell3 = cell2 + (velocity < 0 ? Vector3Int.right : Vector3Int.left);

            if (!hit) return StepType.None;

            if (world.HasBlock(cell1) || world.HasBlock(cell2) || world.HasBlock(cell3))
                return StepType.None;

            return velocity > 0 ? StepType.Right : StepType.Left;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            if (!targetPosition.HasValue) return;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition.Value, .3f);
        }

        enum StepType
        {
            None,
            Left,
            Right
        }
    }
}

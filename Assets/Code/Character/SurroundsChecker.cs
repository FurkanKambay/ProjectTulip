using SaintsField;
using UnityEngine;

namespace Tulip.Character
{
    public class SurroundsChecker : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        public bool IsLeftBlocked { get; private set; }
        public bool IsRightBlocked { get; private set; }

        [Header("References")]
        [SerializeField, Required] new Collider2D collider;

        [Header("Ground")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundRange = .5f;
        [SerializeField] float groundWidth = .5f;

        [Header("Sides")]
        [SerializeField] LayerMask sidesLayer;
        [SerializeField] float sideRange = .5f;
        [SerializeField] float sideHeight = .5f;

        private static readonly Vector2 safetyLeft = Vector2.left * .001f;
        private static readonly Vector2 safetyRight = Vector2.right * .001f;

        private Vector2 MinBounds => collider.bounds.min;
        private Vector2 MaxBounds => collider.bounds.max;

        private Vector2 GroundLeft => transform.position - (Vector3.right * groundWidth);
        private Vector2 GroundRight => transform.position + (Vector3.right * groundWidth);

        private Vector2 LeftTop => new Vector2(MinBounds.x, MinBounds.y + sideHeight) + safetyLeft;
        private Vector2 LeftBottom => MinBounds + safetyLeft;

        private Vector2 RightTop => new Vector2(MaxBounds.x, MinBounds.y + sideHeight) + safetyRight;
        private Vector2 RightBottom => new Vector2(MaxBounds.x, MinBounds.y) + safetyRight;

        private void FixedUpdate()
        {
            UpdateGrounded();
            UpdateSides();
        }

        private void UpdateGrounded()
        {
            bool groundLeft = Physics2D.Raycast(GroundLeft, Vector2.down, groundRange, groundLayer);
            bool groundRight = Physics2D.Raycast(GroundRight, Vector2.down, groundRange, groundLayer);
            IsGrounded = groundLeft || groundRight;
        }

        private void UpdateSides()
        {
            bool leftTop = Physics2D.Raycast(LeftTop, Vector2.left, sideRange, sidesLayer);
            bool leftBottom = Physics2D.Raycast(LeftBottom, Vector2.left, sideRange, sidesLayer);
            bool rightTop = Physics2D.Raycast(RightTop, Vector2.right, sideRange, sidesLayer);
            bool rightBottom = Physics2D.Raycast(RightBottom, Vector2.right, sideRange, sidesLayer);
            IsLeftBlocked = leftTop || leftBottom;
            IsRightBlocked = rightTop || rightBottom;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.gray;
            Gizmos.DrawRay(GroundLeft, Vector2.down * groundRange);
            Gizmos.DrawRay(GroundRight, Vector2.down * groundRange);

            Gizmos.color = IsLeftBlocked ? Color.green : Color.gray;
            Gizmos.DrawRay(LeftTop, Vector2.left * sideRange);
            Gizmos.DrawRay(LeftBottom, Vector2.left * sideRange);

            Gizmos.color = IsRightBlocked ? Color.green : Color.gray;
            Gizmos.DrawRay(RightTop, Vector2.right * sideRange);
            Gizmos.DrawRay(RightBottom, Vector2.right * sideRange);
        }
    }
}

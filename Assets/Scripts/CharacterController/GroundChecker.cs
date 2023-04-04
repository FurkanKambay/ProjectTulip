using UnityEngine;

namespace Game.CharacterController
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float checkHeight = .5f;
        [SerializeField] private float offset = .5f;

        private Vector2 LeftSide => transform.position - (Vector3.right * offset);
        private Vector2 RightSide => transform.position + (Vector3.right * offset);

        private void FixedUpdate()
        {
            bool left = Physics2D.Raycast(LeftSide, Vector2.down, checkHeight, groundLayer);
            bool right = Physics2D.Raycast(RightSide, Vector2.down, checkHeight, groundLayer);
            IsGrounded = left || right;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(LeftSide, LeftSide + (Vector2.down * checkHeight));
            Gizmos.DrawLine(RightSide, RightSide + (Vector2.down * checkHeight));
        }
    }
}

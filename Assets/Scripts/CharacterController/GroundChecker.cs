using UnityEngine;

namespace Game.CharacterController
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsOnGround { get; private set; }

        [SerializeField] private float groundLength = .5f;
        [SerializeField] private float colliderOffset = .5f;
        [SerializeField] private LayerMask groundLayer;

        private Vector3 LeftSide => transform.position - (Vector3.right * colliderOffset);
        private Vector3 RightSide => transform.position + (Vector3.right * colliderOffset);

        private void Update()
        {
            IsOnGround =
                Physics2D.Raycast(LeftSide, Vector2.down, groundLength, groundLayer)
                || Physics2D.Raycast(RightSide, Vector2.down, groundLength, groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOnGround ? Color.green : Color.red;
            Gizmos.DrawLine(LeftSide, LeftSide + (Vector3.down * groundLength));
            Gizmos.DrawLine(RightSide, RightSide + (Vector3.down * groundLength));
        }
    }
}

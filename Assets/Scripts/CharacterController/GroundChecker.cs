using UnityEngine;

namespace Game.CharacterController
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsOnGround { get; private set; }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 boxSize;

        private void FixedUpdate() => IsOnGround = Physics2D.OverlapBox(
            transform.position, boxSize, 0, groundLayer);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOnGround ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }
}

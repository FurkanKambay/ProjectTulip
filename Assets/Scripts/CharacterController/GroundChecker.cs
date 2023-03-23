using UnityEngine;

namespace Game.CharacterController
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsOnGround { get; private set; }

        [SerializeField] private float groundLength = .5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 boxSize;

        private void FixedUpdate()
        {
            IsOnGround = Physics2D.BoxCast(transform.position, boxSize, 0,
                Vector2.down, groundLength, groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOnGround ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }
}

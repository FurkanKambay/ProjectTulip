using UnityEngine;

namespace Game.CharacterController
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsOnGround { get; private set; }

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float size;

        private void FixedUpdate() => IsOnGround = Physics2D.OverlapCircle(
            transform.position, size, groundLayer);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOnGround ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, size);
        }
    }
}

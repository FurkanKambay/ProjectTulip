using UnityEngine;

namespace Tulip.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Vector3 respawnPosition;
        private Rigidbody2D body;

        private void Awake() => body = GetComponent<Rigidbody2D>();
        public void Respawn() => body.MovePosition(respawnPosition);
    }
}

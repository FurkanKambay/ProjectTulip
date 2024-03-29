using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    [RequireComponent(typeof(IDasherBrain))]
    public class Dasher : MonoBehaviour
    {
        public float dashSpeed = 10f;
        public float dashCooldown = 0.5f;

        [SerializeField] ForceMode2D forceMode;

        private IDasherBrain brain;
        private Rigidbody2D body;

        private float timeSinceLastDash;

        private void Awake()
        {
            brain = GetComponent<IDasherBrain>();
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            timeSinceLastDash += Time.deltaTime;
            if (!brain.WantsToDash || timeSinceLastDash < dashCooldown) return;

            timeSinceLastDash = 0f;
            float direction = brain.HorizontalMovement;

            if (Mathf.Abs(direction) > 0.1f)
                body.AddForce(Vector2.right * (direction * dashSpeed), forceMode);
        }
    }
}

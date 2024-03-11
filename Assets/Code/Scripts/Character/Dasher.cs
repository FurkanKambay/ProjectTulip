using Tulip.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tulip.Character
{
    public class Dasher : MonoBehaviour
    {
        [SerializeField] private InputHelper inputHelper;

        public float dashSpeed = 10f;
        public float dashCooldown = 0.5f;

        [SerializeField] ForceMode2D forceMode;

        private Rigidbody2D body;
        private InputAction movement;

        private float timeSinceLastDash;

        private void Update()
        {
            timeSinceLastDash += Time.deltaTime;
            bool dashing = inputHelper.Actions.Player.Dash.IsInProgress();
            if (!dashing || timeSinceLastDash < dashCooldown) return;

            timeSinceLastDash = 0f;
            float direction = movement.ReadValue<float>();

            if (Mathf.Abs(direction) > 0.1f)
                body.AddForce(Vector2.right * (direction * dashSpeed), forceMode);
        }

        private void Awake()
        {
            movement = inputHelper.Actions.Player.MoveX;
            body = GetComponent<Rigidbody2D>();
        }
    }
}

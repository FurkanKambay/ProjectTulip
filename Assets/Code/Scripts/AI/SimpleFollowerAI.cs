using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    [RequireComponent(typeof(ICharacterMovement))]
    public class SimpleFollowerAI : MonoBehaviour
    {
        private ICharacterMovement movement;
        private IHealth health;
        private Transform target;

        private void Awake()
        {
            movement = GetComponent<ICharacterMovement>();
            health = GetComponent<IHealth>();
            if (!target) target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (!target || movement == null) return;

            if (health.IsDead)
            {
                movement.Input = Vector2.zero;
                return;
            }

            Vector3 distance = target.transform.position - transform.position;
            movement.Input = Mathf.Abs(distance.x) < 1f ? Vector2.zero : distance.normalized;
        }
    }
}

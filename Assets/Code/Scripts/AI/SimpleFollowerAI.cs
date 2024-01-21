using Game.Data.Interfaces;
using UnityEngine;

namespace Game.AI
{
    [RequireComponent(typeof(IMovement))]
    public class SimpleFollowerAI : MonoBehaviour
    {
        private IMovement movement;
        private IHealth health;
        private Transform target;

        private void Awake()
        {
            movement = GetComponent<IMovement>();
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

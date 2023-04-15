using Game.Data.Interfaces;
using UnityEngine;

namespace Game.AI
{
    [RequireComponent(typeof(IMovement))]
    public class SimpleFollowerAI : MonoBehaviour
    {
        private Transform target;
        private IMovement movement;

        private void Awake()
        {
            movement = GetComponent<IMovement>();
            if (!target) target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (!target || movement == null) return;
            Vector3 distance = target.transform.position - transform.position;
            movement.Input = Mathf.Abs(distance.x) < 1f ? Vector2.zero : distance.normalized;
        }
    }
}

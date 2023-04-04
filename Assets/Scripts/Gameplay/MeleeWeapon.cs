using System;
using Game.Data.Items;
using UnityEngine;
using Input = Game.Player.Input;

namespace Game.Gameplay
{
    public class MeleeWeapon : MonoBehaviour
    {
        public WeaponData data;
        public LayerMask hitMask;

        private new SpriteRenderer renderer;
        private float timeSinceLastUse;
        private int aimDirectionSign;

        private void Awake() => renderer = GetComponentInChildren<SpriteRenderer>();

        private void Update()
        {
            timeSinceLastUse += Time.deltaTime;
            if (Input.Actions.Player.Fire.inProgress)
                Attack();
        }

        private void Attack()
        {
            if (timeSinceLastUse <= data.Cooldown) return;
            timeSinceLastUse = 0f;

            Health health = CheckAttackBox(Input.Instance.MouseWorldPoint);
            if (health)
                health.TakeDamage(data.Damage);
        }

        private Health CheckAttackBox(Vector2 mouseWorld)
        {
            Vector2 position = transform.position;
            aimDirectionSign = Math.Sign((mouseWorld - position).x);
            renderer.flipX = aimDirectionSign < 0;

            Collider2D hit = Physics2D.OverlapBox(
                position + new Vector2(data.Range / 2f * aimDirectionSign, 1f),
                new Vector2(data.Range, 1f),
                default,
                hitMask);

            return hit ? hit.GetComponent<Health>() : null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled) return;

            Vector2 position = transform.position;

            var box = new Vector3(data.Range, 1f, 1f);
            var offset = new Vector3(box.x / 2f * aimDirectionSign, 1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector3)position + offset, box);
        }
    }
}

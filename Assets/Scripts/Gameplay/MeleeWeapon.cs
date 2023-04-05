using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data.Items;
using UnityEngine;
using Input = Game.Player.Input;

namespace Game.Gameplay
{
    public class MeleeWeapon : MonoBehaviour
    {
        public WeaponData data;
        public LayerMask hitMask;

        private Health playerHealth;
        private new SpriteRenderer renderer;
        private float timeSinceLastUse;
        private int aimDirectionSign;

        private void Awake()
        {
            playerHealth = GetComponent<Health>();
            renderer = GetComponentInChildren<SpriteRenderer>();
        }

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

            IEnumerable<Health> targets = CheckAttackBox(Input.Instance.MouseWorldPoint);

            foreach (Health target in targets)
            {
                if (!target) continue;
                target.TakeDamage(data.Damage, playerHealth);
            }
        }

        private IEnumerable<Health> CheckAttackBox(Vector2 mouseWorld)
        {
            Vector2 position = transform.position;
            aimDirectionSign = Math.Sign((mouseWorld - position).x);
            renderer.flipX = aimDirectionSign < 0;

            Vector2 point = position + new Vector2(data.Range / 2f * aimDirectionSign, 1f);
            var size = new Vector2(data.Range, 1f);

            if (data.CanMultiHit)
            {
                // TODO: limit the amount of hits?
                Collider2D[] allHits = Physics2D.OverlapBoxAll(point, size, default, hitMask);
                return allHits.Select(hit => hit.GetComponent<Health>());
            }
            else
            {
                // TODO: find the closest hit instead?
                Collider2D hit = Physics2D.OverlapBox(point, size, default, hitMask);
                return new[] { hit ? hit.GetComponent<Health>() : null };
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data.Items;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay
{
    public class WeaponWielder : MonoBehaviour
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
            if (InputHelper.Actions.Player.Use.inProgress)
                Attack();
        }

        private void Attack()
        {
            if (timeSinceLastUse <= data.Cooldown) return;
            timeSinceLastUse = 0f;

            IEnumerable<Health> targets = CheckAttackBox(InputHelper.Instance.MouseWorldPoint);

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
            var attackBoxSize = new Vector2(data.Range, 1f);

            var hits = new Collider2D[9];
            if (data.IsMultiTarget)
            {
                _ = Physics2D.OverlapBoxNonAlloc(point, attackBoxSize, default, hits, hitMask);
                return hits.Select(hit => hit ? hit.GetComponent<Health>() : null);
            }

            // TODO: find the closest hit instead?
            _ = Physics2D.OverlapBoxNonAlloc(point, attackBoxSize, default, hits, hitMask);
            Collider2D singleHit = hits[0];
            return new[] { singleHit ? singleHit.GetComponent<Health>() : null };
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

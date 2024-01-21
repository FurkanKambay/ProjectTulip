using System.Collections.Generic;
using System.Linq;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Gameplay.Extensions;
using UnityEngine;

namespace Game.Gameplay
{
    public class WeaponWielder : MonoBehaviour
    {
        public LayerMask hitMask;

        private Health health;
        private ItemWielder itemWielder;
        private new SpriteRenderer renderer;

        private WeaponData weaponData;
        private ItemSwingDirection swingDirection;

        private void Awake()
        {
            health = GetComponent<Health>();
            itemWielder = GetComponent<ItemWielder>();
            renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Attack(IUsable usable, ItemSwingDirection direction)
        {
            if (usable is not WeaponData weapon) return;
            weaponData = weapon;

            swingDirection = direction;
            IEnumerable<Health> targets = CheckAttackBox();

            foreach (Health target in targets)
            {
                if (!target) continue;
                target.TakeDamage(weaponData.Damage, health);
            }
        }

        private IEnumerable<Health> CheckAttackBox()
        {
            Vector2 position = transform.position;
            renderer.flipX = swingDirection == ItemSwingDirection.Left;
            // renderer.flipY = swingDirection == ItemSwingDirection.Down;

            var direction = swingDirection.ToVector2();
            Vector2 point = position + new Vector2(weaponData.Range / 2f * direction.x, 1f * direction.y);
            var attackBoxSize = new Vector2(weaponData.Range, 1f);

            var hits = new Collider2D[9];
            if (weaponData.IsMultiTarget)
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

            if (!weaponData) return;
            var box = new Vector3(weaponData.Range, 1f, 1f);
            var offset = new Vector3(box.x / 2f * swingDirection.ToVector2().x, 1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector3)position + offset, box);
        }

        private void OnEnable() => itemWielder.OnSwing += Attack;
        private void OnDisable() => itemWielder.OnSwing -= Attack;
    }
}

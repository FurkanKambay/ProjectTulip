using System;
using System.Collections.Generic;
using System.Linq;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using Tulip.Gameplay.Extensions;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class WeaponWielder : MonoBehaviour
    {
        [SerializeField] ContactFilter2D hitContactFilter;
        [SerializeField] int maxMultiTargetAmount = 9;

        private Health health;
        private ItemWielder itemWielder;
        private new SpriteRenderer renderer;

        private Weapon weapon;
        private ItemSwingDirection swingDirection;
        private Collider2D[] hits = Array.Empty<Collider2D>();

        private void Awake()
        {
            health = GetComponent<Health>();
            itemWielder = GetComponent<ItemWielder>();
            renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Attack(Usable usable, ItemSwingDirection direction)
        {
            if (usable is not Weapon usedWeapon) return;
            weapon = usedWeapon;
            swingDirection = direction;

            Array.Resize(ref hits, weapon.IsMultiTarget ? maxMultiTargetAmount : 1);

            foreach (Health target in GetTargets())
            {
                if (!target.enabled) continue;
                target.TakeDamage(weapon.Damage, health);
            }
        }

        private IEnumerable<Health> GetTargets()
        {
            Vector2 position = transform.position;
            renderer.flipX = swingDirection == ItemSwingDirection.Left;
            // renderer.flipY = swingDirection == ItemSwingDirection.Down;

            var direction = swingDirection.ToVector2();
            Vector2 point = position + new Vector2(weapon.Range / 2f * direction.x, 1f * direction.y);
            var attackBoxSize = new Vector2(weapon.Range, 1f);

            // TODO: find the closest hit instead for single hit?
            // BUG: attacks shouldn't go through walls
            int hitCount = Physics2D.OverlapBox(point, attackBoxSize, default, hitContactFilter, hits);
            return hits.Take(hitCount)
                .TakeWhile(hit => (bool)hit)
                .Select(hit => hit.GetComponent<Health>())
                .Where(hitHealth => (bool)hitHealth);
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled) return;

            Vector2 position = transform.position;

            if (!weapon) return;
            var box = new Vector3(weapon.Range, 1f, 1f);
            var offset = new Vector3(box.x / 2f * swingDirection.ToVector2().x, 1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector3)position + offset, box);
        }

        private void OnEnable() => itemWielder.OnSwing += Attack;
        private void OnDisable() => itemWielder.OnSwing -= Attack;
    }
}

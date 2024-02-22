using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<Health> targets = CheckAttackBox();

            foreach (Health target in targets)
            {
                if (!target) continue;
                target.TakeDamage(weapon.Damage, health);
            }
        }

        private IEnumerable<Health> CheckAttackBox()
        {
            Vector2 position = transform.position;
            renderer.flipX = swingDirection == ItemSwingDirection.Left;
            // renderer.flipY = swingDirection == ItemSwingDirection.Down;

            var direction = swingDirection.ToVector2();
            Vector2 point = position + new Vector2(weapon.Range / 2f * direction.x, 1f * direction.y);
            var attackBoxSize = new Vector2(weapon.Range, 1f);

            var hits = new Collider2D[maxMultiTargetAmount];
            if (weapon.IsMultiTarget)
            {
                Physics2D.OverlapBox(point, attackBoxSize, default, hitContactFilter, hits);
                return hits.Select(hit => hit.GetComponent<Health>());
            }

            // TODO: find the closest hit instead?
            Physics2D.OverlapBox(point, attackBoxSize, default, hitContactFilter, hits);
            Collider2D singleHit = hits[0];
            return new[] { singleHit ? singleHit.GetComponent<Health>() : null };
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

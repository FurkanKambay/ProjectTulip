using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class WeaponWielder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] ItemWielder itemWielder;

        [Header("Config")]
        [SerializeField] ContactFilter2D hitContactFilter;
        [SerializeField] int maxMultiTargetAmount = 9;

        private Weapon weapon;
        private Vector3 aimPosition;
        private Collider2D[] hits = Array.Empty<Collider2D>();

        private void Attack(Usable usable, Vector3 position)
        {
            if (usable is not Weapon usedWeapon) return;
            weapon = usedWeapon;
            aimPosition = position;

            Array.Resize(ref hits, weapon.IsMultiTarget ? maxMultiTargetAmount : 1);

            foreach (Health target in GetTargets())
            {
                if (!target.enabled) continue;
                target.TakeDamage(weapon.Damage, health);
            }
        }

        private IEnumerable<Health> GetTargets()
        {
            Vector3 position = transform.position;
            var aimDirection = Vector3.Normalize(aimPosition - position);
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            Vector2 point = (Vector2)position + new Vector2(weapon.Range / 2f * aimDirection.x, 1f * aimDirection.y);
            var attackBoxSize = new Vector2(weapon.Range, 1f);

            // TODO: find the closest hit instead for single hit?
            // BUG: attacks shouldn't go through walls
            int hitCount = Physics2D.OverlapBox(point, attackBoxSize, aimAngle, hitContactFilter, hits);
            return hits.Take(hitCount)
                .TakeWhile(hit => (bool)hit)
                .Select(hit => hit.GetComponent<Health>())
                .Where(hitHealth => (bool)hitHealth);
        }

        private void OnEnable() => itemWielder.OnSwing += Attack;
        private void OnDisable() => itemWielder.OnSwing -= Attack;
    }
}

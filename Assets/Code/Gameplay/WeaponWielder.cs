using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class WeaponWielder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] HealthBase health;
        [SerializeField, Required] ItemWielder itemWielder;

        [Header("Config")]
        [SerializeField] ContactFilter2D hitContactFilter;
        [SerializeField] int maxMultiTargetAmount = 9;

        private Weapon weapon;
        private Collider2D[] hits = Array.Empty<Collider2D>();

        private void Attack(ItemStack stack, Vector3 targetPoint)
        {
            if (stack.item is not Weapon usedWeapon)
                return;

            weapon = usedWeapon;

            Array.Resize(ref hits, weapon.IsMultiTarget ? maxMultiTargetAmount : 1);

            foreach (HealthBase target in GetTargets(transform.position, targetPoint))
            {
                if (!target.enabled) continue;
                target.Damage(weapon.Damage, health);
            }
        }

        private IEnumerable<HealthBase> GetTargets(Vector2 origin, Vector2 aimPoint)
        {
            Vector2 direction = (aimPoint - origin).normalized;

            var results = new RaycastHit2D[hits.Length];
            int hitCount = Physics2D.Raycast(origin, direction, hitContactFilter, results, weapon.Range);
            hits = results.Select(hit => hit.collider).ToArray();

            Debug.DrawRay(origin, direction * weapon.Range, Color.green, 1f);

            return hits
                .Take(hitCount)
                .TakeWhile(hit => (bool)hit)
                .Select(hit => hit.GetComponentInChildren<HealthBase>())
                .TakeWhile(hitHealth => (bool)hitHealth);
        }

        private void OnEnable() => itemWielder.OnSwing += Attack;
        private void OnDisable() => itemWielder.OnSwing -= Attack;
    }
}

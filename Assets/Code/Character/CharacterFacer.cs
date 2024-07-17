using SaintsField;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    public class CharacterFacer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] SpriteRenderer sprite;
        [SerializeField, Required] SaintsInterface<Component, ICharacterBrain> brain;

        private void Update()
        {
            if (brain.I.HorizontalMovement != 0)
            {
                sprite.flipX = brain.I.HorizontalMovement < 0;
                return;
            }

            Vector2 delta = brain.I.AimPosition - (Vector2)transform.position;
            sprite.flipX = delta.x < 0;
        }
    }
}

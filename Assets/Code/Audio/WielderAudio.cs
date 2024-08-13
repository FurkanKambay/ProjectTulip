using FMODUnity;
using SaintsField;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Audio
{
    public class WielderAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SaintsInterface<Component, IItemWielder> itemWielder;

        [Header("FMOD Events")]
        [SerializeField] EventReference itemSwingEvent;

        private void OnEnable()
        {
            if (itemWielder.V)
                itemWielder.I.OnSwingPerform += HandleItemSwing;
        }

        private void OnDisable()
        {
            if (itemWielder.V)
                itemWielder.I.OnSwingPerform -= HandleItemSwing;
        }

        private void HandleItemSwing(ItemStack stack, Vector3 _) =>
            RuntimeManager.PlayOneShotAttached(itemSwingEvent, transform.gameObject);
    }
}

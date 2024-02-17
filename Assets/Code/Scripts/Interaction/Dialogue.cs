using UnityEngine;

namespace Tulip.Interaction
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] Interactable interactable;
        [SerializeField, TextArea] string greetingText;

        private void Awake()
        {
            if (!interactable) interactable = GetComponent<Interactable>();
        }

        private void OnEnable() => interactable.OnInteract += HandleInteract;
        private void OnDisable() => interactable.OnInteract -= HandleInteract;

        private void HandleInteract() => Debug.Log(greetingText);
    }
}

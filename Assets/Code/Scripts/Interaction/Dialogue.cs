using UnityEngine;

namespace Game.Interaction
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] Interactable interactable;
        [SerializeField, TextArea] string greetingText;

        private void Awake()
        {
            interactable ??= GetComponent<Interactable>();
            interactable.OnInteract += HandleInteract;
        }

        private void HandleInteract() => Debug.Log(greetingText);
    }
}

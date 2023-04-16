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
            interactable.Interacted += OnInteract;
        }

        private void OnInteract() => Debug.Log(greetingText);
    }
}

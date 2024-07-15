using UnityEngine;

namespace Tulip.Core
{
    public class DebugTesting : MonoBehaviour
    {
        private async void Start() => await GameState.SwitchTo(GameState.Testing);
    }
}

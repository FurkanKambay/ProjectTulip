using UnityEngine;

namespace Tulip.Core
{
    public class DebugTesting : MonoBehaviour
    {
        private void Start() => GameManager.SwitchTo(GameState.Testing);
    }
}

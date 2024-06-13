using UnityEngine;

namespace Tulip.Core
{
    public class DebugTileTest : MonoBehaviour
    {
        private async void Start() => await GameState.SwitchTo(GameState.Testing);
    }
}

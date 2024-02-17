using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tulip
{
    public class Bootstrapper : MonoBehaviour
    {
        private void Start() => SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
    }
}

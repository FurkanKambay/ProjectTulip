using UnityEngine;

namespace Tulip.UI
{
    public class MenuPlayground : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] new Camera camera;

        private void Update()
        {
            Vector3 cameraCenter = camera.transform.position;
            float cameraExtent = camera.orthographicSize * camera.aspect;
            float left = cameraCenter.x - cameraExtent;
            float right = cameraCenter.x + cameraExtent;

            Vector3 targetPosition = player.position;

            if (player.position.x > right)
                targetPosition.x = left;
            else if (player.position.x < left)
                targetPosition.x = right;

            player.position = targetPosition;
        }
    }
}

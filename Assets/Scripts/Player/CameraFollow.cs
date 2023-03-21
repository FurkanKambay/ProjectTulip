using Game.Data;
using UnityEngine;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        public ZoomOptions zoom;

        [SerializeField] private Vector2 offset;
        [SerializeField] private Transform player;
        private new Camera camera;

        private void Awake() => camera = GetComponent<Camera>();

        private void LateUpdate()
        {
            Transform cam = transform;
            Vector3 center = player.position + (Vector3)offset;
            cam.position = new Vector3(center.x, center.y, cam.position.z);
            camera.orthographicSize = zoom.Value;
        }

        [System.Serializable]
        public class ZoomOptions : IValidate
        {
            [SerializeField] private float value = 10f;

            public float Value
            {
                get => value;
                set => this.value = Mathf.Clamp(value, Min, Max);
            }

            [field: SerializeField] public float Min { get; private set; } = 10f;
            [field: SerializeField] public float Max { get; private set; } = 10f;

            public void OnValidate() => Value = value;
        }
    }
}

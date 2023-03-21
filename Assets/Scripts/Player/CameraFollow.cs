using Game.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector2 offset;
        public ZoomOptions zoom;

        private new Camera camera;

        private void Awake() => camera = GetComponent<Camera>();
        private void Update() => zoom.Value -= Mouse.current.scroll.y.value * zoom.Sensitivity;

        private void LateUpdate()
        {
            Transform cam = transform;
            Vector3 center = player.position + (Vector3)offset;
            cam.position = new Vector3(center.x, center.y, cam.position.z);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom.Value, Time.deltaTime * zoom.Speed);
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

            [field: SerializeField] public float Min { get; private set; } = 1f;
            [field: SerializeField] public float Max { get; private set; } = 100f;
            [field: SerializeField] public float Sensitivity { get; private set; } = .01f;
            [field: SerializeField] public float Speed { get; private set; } = 10f;

            public void OnValidate() => Value = value;
        }
    }
}

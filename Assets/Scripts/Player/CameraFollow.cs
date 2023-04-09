using System;
using Game.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform player;

        public TrackingOptions tracking;
        public ZoomOptions zoom;

        private new Camera camera;

        private void Awake() => camera = GetComponent<Camera>();

        private void Update()
        {
            tracking.Target = player.position + (Vector3)tracking.Offset;
            // zoom.Target -= Mouse.current.scroll.y.value * zoom.Sensitivity;
        }

        private void LateUpdate()
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom.Target, Time.deltaTime * zoom.Speed);

            Vector3 position = transform.position;
            float lerpX = Mathf.Lerp(position.x, tracking.Target.x, Time.deltaTime * tracking.Speed.x);
            float lerpY = Mathf.Lerp(position.y, tracking.Target.y, Time.deltaTime * tracking.Speed.y);

            Vector3 distance = tracking.Target - position;
            float targetX = Mathf.Abs(distance.x) < tracking.SnapValue ? tracking.Target.x : lerpX;
            float targetY = Mathf.Abs(distance.y) < tracking.SnapValue ? tracking.Target.y : lerpY;
            transform.position = new Vector3(targetX, targetY, tracking.Target.z);
        }

        [Serializable]
        public class TrackingOptions : IValidate
        {
            private Vector3 target;

            public Vector3 Target
            {
                get => target;
                set => target = new Vector3(value.x, value.y, -10f);
            }

            [field: SerializeField] public Vector2 Offset { get; private set; }
            [field: SerializeField] public Vector2 Speed { get; private set; } = Vector2.one * 10f;
            [field: SerializeField] public float SnapValue { get; private set; } = .5f;

            public void OnValidate() { }
        }

        [Serializable]
        public class ZoomOptions : IValidate
        {
            [SerializeField] float target = 10f;

            public float Target
            {
                get => target;
                set => this.target = Mathf.Clamp(value, Min, Max);
            }

            [field: SerializeField] public float Min { get; private set; } = 1f;
            [field: SerializeField] public float Max { get; private set; } = 100f;
            [field: SerializeField] public float Sensitivity { get; private set; } = .01f;
            [field: SerializeField] public float Speed { get; private set; } = 10f;

            public void OnValidate() => Target = target;
        }
    }
}

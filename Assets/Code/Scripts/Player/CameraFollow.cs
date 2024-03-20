using System;
using Tulip.Core;
using Tulip.Data;
using Tulip.Input;
using UnityEngine;

namespace Tulip.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform player;

        [Header("Config")]
        [SerializeField, Range(-0.5f, 0.5f)] float menuPeekAmountX;
        [SerializeField, Range(-0.5f, 0.5f)] float menuPeekAmountY;
        [SerializeField] Vector2 menuOffset;

        public TrackingOptions tracking;
        public ZoomOptions zoom;

        private new Camera camera;
        private Vector3 initialPosition;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            initialPosition = transform.position;
        }

        private void OnEnable() => tracking.Target = initialPosition;

        private void Update()
        {
            if (GameState.Current == GameState.MainMenu)
            {
                Vector2 mouseScreenPoint = InputHelper.Instance.Actions.UI.Point.ReadValue<Vector2>();
                Vector3 mouseWorldPoint = camera.ScreenToWorldPoint(mouseScreenPoint);
                Vector3 peekAmount = mouseWorldPoint * new Vector2(menuPeekAmountX, menuPeekAmountY);

                tracking.Target = initialPosition + peekAmount;
            }
            else if (GameState.Current == GameState.Playing)
            {
                Vector3 targetPoint = (bool)player ? player.position : initialPosition;
                tracking.Target = targetPoint + (Vector3)tracking.Offset;

                float zoomDelta = InputHelper.Instance.Actions.Player.Zoom.ReadValue<float>();
                zoom.Target -= zoomDelta * zoom.Sensitivity * Time.deltaTime;
            }
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

        private void OnValidate()
        {
            initialPosition = new Vector3(menuOffset.x, menuOffset.y, -10);
            transform.position = initialPosition;
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

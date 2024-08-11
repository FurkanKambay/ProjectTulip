using System;
using SaintsField;
using Tulip.Core;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] new Camera camera;
        [SerializeField, GetComponentInScene] SaintsInterface<Component, IPlayerBrain> brain;

        [Header("Config")]
        [SerializeField, Range(-0.5f, 0.5f)] float menuPeekAmountX;
        [SerializeField, Range(-0.5f, 0.5f)] float menuPeekAmountY;
        [SerializeField] Vector2 menuOffset;

        [SerializeField] TrackingOptions trackingConfig;
        [SerializeField] ZoomOptions zoomConfig;

        private Vector3 initialPosition;

        private void Awake() => initialPosition = transform.position;

        private void OnEnable() => trackingConfig.Target = initialPosition;

        private void Update()
        {
            if (GameState.Current == GameState.MainMenu)
            {
                var clampedScreenPoint = new Vector3(
                    x: Mathf.Clamp(brain.I.AimPointScreen.x, 0, camera.pixelWidth),
                    y: Mathf.Clamp(brain.I.AimPointScreen.y, 0, camera.pixelHeight));

                Vector3 mouseWorldPoint = camera.ScreenToWorldPoint(clampedScreenPoint);
                Vector3 peekAmount = mouseWorldPoint * new Vector2(menuPeekAmountX, menuPeekAmountY);

                trackingConfig.Target = initialPosition + peekAmount;
            }
            else if (GameState.Current == GameState.Playing || GameState.Current == GameState.Testing)
            {
                Vector3 targetPoint = (bool)brain.V ? brain.V.transform.position : initialPosition;
                trackingConfig.Target = targetPoint + (Vector3)trackingConfig.Offset;
                zoomConfig.Target -= brain.I.ZoomDelta * zoomConfig.Sensitivity * Time.deltaTime;
            }
        }

        private void LateUpdate()
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomConfig.Target, Time.deltaTime * zoomConfig.Speed);

            Vector3 position = transform.position;
            float lerpX = Mathf.Lerp(position.x, trackingConfig.Target.x, Time.deltaTime * trackingConfig.Speed.x);
            float lerpY = Mathf.Lerp(position.y, trackingConfig.Target.y, Time.deltaTime * trackingConfig.Speed.y);

            Vector3 distance = trackingConfig.Target - position;
            float targetX = Mathf.Abs(distance.x) < trackingConfig.SnapValue ? trackingConfig.Target.x : lerpX;
            float targetY = Mathf.Abs(distance.y) < trackingConfig.SnapValue ? trackingConfig.Target.y : lerpY;
            camera.transform.position = new Vector3(targetX, targetY, trackingConfig.Target.z);
        }

        private void OnValidate()
        {
            initialPosition = menuOffset.WithZ(-10);
            camera.transform.position = initialPosition;
        }

        [Serializable]
        public class TrackingOptions : IValidate
        {
            private Vector3 target;

            public Vector3 Target
            {
                get => target;
                set => target = value.With(z: -10f);
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

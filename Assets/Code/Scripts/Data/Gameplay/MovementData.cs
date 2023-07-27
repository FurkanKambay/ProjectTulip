using UnityEngine;

namespace Game.Data.Gameplay
{
    [CreateAssetMenu(menuName = "Data/Movement")]
    public class MovementData : ScriptableObject
    {
        [Range(0f, 100f)] public float maxSpeed = 10f;
        [SerializeField] public float friction;

        [Header("Acceleration")]
        public bool useAcceleration;

        [Header("Ground Acceleration")]
        [Range(0f, 100f)] public float maxAcceleration = 50f;
        [Range(0f, 100f)] public float maxDeceleration = 50f;
        [Range(0f, 100f)] public float maxTurnSpeed = 80f;

        [Header("Air Acceleration")]
        [Range(0f, 100f)] public float maxAirAcceleration = 50f;
        [Range(0f, 100f)] public float maxAirDeceleration = 50f;
        [Range(0f, 100f)] public float maxAirTurnSpeed = 80f;
    }
}

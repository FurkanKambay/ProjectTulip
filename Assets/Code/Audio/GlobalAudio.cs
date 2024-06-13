using SaintsField;
using UnityEngine;

namespace Tulip.Audio
{
    public class GlobalAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] AudioSource audioSource;

        [Header("Config")]
        [SerializeField] AudioClip backgroundMusic;

        public AudioClip BackgroundMusic
        {
            get => backgroundMusic;
            set => backgroundMusic = audioSource.clip = value;
        }
    }
}

using UnityEngine;

namespace Tulip.Audio
{
    public class GlobalAudio : MonoBehaviour
    {
        public AudioSource AudioSource { get; private set; }

        [SerializeField] AudioClip backgroundMusic;

        public AudioClip BackgroundMusic
        {
            get => backgroundMusic;
            set => backgroundMusic = AudioSource.clip = value;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AudioSource = GetComponent<AudioSource>();
        }
    }
}

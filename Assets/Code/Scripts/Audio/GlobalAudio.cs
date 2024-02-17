using UnityEngine;

namespace Game
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

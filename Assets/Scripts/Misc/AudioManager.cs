using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SwordsInSpace
{
    public class AudioManager : NetworkBehaviour
    {
        public static AudioManager instance;

        [SerializeField]
        private AudioSource effectAudioSource, BGMAudioSource;

        [SerializeField]
        public AudioClip[] AudioClips;


        private void Awake()
        {
            if (instance)
            {
                Debug.Log("More than 1 AudioManager present at once! Keeping old AudioManager.");
                Destroy(this);
                return;
            }
            instance = this;
        }

        [ServerRpc]
        public void ServerPlay(int audioClipIndex)
        {
            ObserversPlay(audioClipIndex);
        }

        [ObserversRpc]
        public void ObserversPlay(int audioClipIndex)
        {
            effectAudioSource.PlayOneShot(AudioClips[audioClipIndex]);
        }

    }

}

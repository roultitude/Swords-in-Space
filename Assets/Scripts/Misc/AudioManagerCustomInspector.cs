using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SwordsInSpace
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AudioManager am = (AudioManager) target;
            if (GUILayout.Button("UpdateAudioClips", GUILayout.Height(30)))
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(AudioClip).Name);  //FindAssets uses tags check documentation for more info
                List<AudioClip> newAudioClips = new List<AudioClip>();
                for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    newAudioClips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(path));
                }
                am.AudioClips = newAudioClips.ToArray();
            }

        }
    }
#endif
}


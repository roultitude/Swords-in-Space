using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Display : MonoBehaviour
    {
        [SerializeField]
        private bool mobileOnly;

        public virtual void Setup(Interactable callingInteractable = null) {}

#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        private void Awake()
        {
            //gameObject.SetActive(true);
            
        }
#else
        private void Awake()
        {
            //if (mobileOnly) Destroy(gameObject);
            //gameObject.SetActive(!mobileOnly);
        }
#endif
    }
}
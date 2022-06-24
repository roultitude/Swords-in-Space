using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class Display : MonoBehaviour
    {
        [SerializeField]
        protected bool mobileOnly;

        public virtual void Setup(Interactable callingInteractable = null) {}

#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        public virtual void Awake()
        {
            gameObject.SetActive(true);
        }
#else
        public virtual void Awake()
        {
            if (mobileOnly)
            {
                Destroy(gameObject);
            }
        }
#endif
    }
}
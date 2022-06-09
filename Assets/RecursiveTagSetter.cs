using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{


    public class RecursiveTagSetter : MonoBehaviour
    {
        [SerializeField]
        string targetTag;
        // Start is called before the first frame update
        public void Awake()
        {
            AddTagRecursively(gameObject.transform, targetTag);
        }
        public static void AddTagRecursively(Transform trans, string tag)
        {
            trans.gameObject.tag = tag;
            if (trans.childCount > 0)
                foreach (Transform t in trans)
                    AddTagRecursively(t, tag);
        }
    }
}
;
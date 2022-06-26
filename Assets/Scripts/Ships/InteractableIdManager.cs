using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SwordsInSpace
{
    public class InteractableIdManager : MonoBehaviour
    {
        // Start is called before the first frame update
        List<Interactable> data;
        void Start()
        {
            RefreshData();
        }

        public void RefreshData()
        {
            data = new List<Interactable>(GetComponentsInChildren<Interactable>());
            // data.Sort((x, y) => x.name.CompareTo(y.name));
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Interactable GetInteractable(int id)
        {
            RefreshData();
            if (id < data.Count)
            {
                return data[id];
            }
            return null;
        }

        public int GetId(Interactable inp)
        {
            RefreshData();
            if (data.Contains(inp))
            {
                return data.IndexOf(inp);
            }
            return -1;
        }
    }
};

using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SwordsInSpace
{
    public class UpgradeBackgroundPanel : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        Image image;


        public Color tier1;
        public Color tier2;
        public Color tier3;
        public Color tier4;


        public void UpdateColor(int currenttier)
        {

            switch (currenttier)
            {
                case 1:
                    image.color = tier1;
                    break;
                case 2:
                    image.color = tier2;
                    break;
                case 3:
                    image.color = tier3;
                    break;
                case 4:
                    image.color = tier4;
                    break;
            }
        }
    }
};

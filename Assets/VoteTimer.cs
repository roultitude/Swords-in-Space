using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SwordsInSpace
{
    public class VoteTimer : MonoBehaviour
    {
        // Start is called before the first frame update
        public Timer timer;

        public TextMeshProUGUI textBox;

        // Update is called once per frame
        void Update()
        {

            string timestr = "" + (int)(timer.waitTime - timer.currentTime);
            if (timestr != textBox.text)
            {
                textBox.text = timestr;
            }
        }
    }
};
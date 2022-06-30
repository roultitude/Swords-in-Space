
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class LazerAfterimage : MonoBehaviour
    {
        public LineRenderer rend;
        private float fadeSpeed;

        public void Setup(Vector3 start, Vector3 end, float fade, Color startColor, Color endColor)
        {
            fadeSpeed = fade;
            rend.useWorldSpace = true;

            rend.positionCount = 2;
            rend.SetPositions(new Vector3[] { start, end });


            rend.startColor = startColor;
            rend.endColor = endColor;

            StartCoroutine("Fadeout");
        }



        IEnumerator Fadeout()
        {
            for (float f = 1f; f >= -0.05f; f -= fadeSpeed * Time.deltaTime)
            {
                Color c = rend.startColor;
                c.a = f;
                rend.startColor = c;
                c = rend.endColor;
                c.a = f;
                rend.endColor = c;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
};

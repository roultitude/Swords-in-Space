using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class FadeScript : MonoBehaviour
    {

        public SpriteRenderer rend;
        public float AnimTime = 30f;
        public void doFade()
        {
            StartCoroutine("Fadeout");

        }

        IEnumerator Fadeout()
        {

            float alpha = rend.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / AnimTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
                rend.color = newColor;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
};
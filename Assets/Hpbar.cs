using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace SwordsInSpace
{
    public class Hpbar : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        GameObject Foreground;

        Image[] images;

        public bool fade = false;
        public float fadeDur = 0.8f;
        public float fadeTime = 0.8f;
        public void Resize(float fillamt)
        {
            Foreground.GetComponent<Image>().fillAmount = fillamt;

            if (fade)
            {
                if (images == null)
                    images = GetComponentsInChildren<Image>();

                foreach (Image rend in images)
                {
                    Color toChange = rend.color;
                    Color newColor = new Color(toChange.r, toChange.g, toChange.b, 1);
                    rend.color = newColor;
                    DOTween.To(() => rend.color.a
                    , x => rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, x)
                    , 0, fadeDur);
                }


            }
        }

        private void Hide()
        {

            if (images == null)
                images = GetComponentsInChildren<Image>();

            foreach (Image rend in images)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0);
            }

        }

        public void DoFade(float fadeDur)
        {
            if (fadeDur > 0f)
            {
                fade = true;
                this.fadeDur = fadeDur;
                Hide();

            }
            else { fade = false; }
        }
    }
};

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordsInSpace
{
    public class MessageDisplay : Display
    {
        [SerializeField]
        GameObject messageLayoutGroup, messagePrefab;

        CanvasGroup cg;
        ScrollRect sr;
        new private void Awake()
        {
            base.Awake();
            sr = GetComponentInChildren<ScrollRect>();
            cg = GetComponentInChildren<CanvasGroup>();
            sr.onValueChanged.AddListener(OnScroll);
        }
        public void DisplayMessage(string message)
        {
            cg.alpha = 1;
            GameObject newMessage = Instantiate(messagePrefab, sr.content);
            newMessage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        }

        public void OnScroll(Vector2 value)
        {
            cg.alpha = 1;
        }

        private void Update()
        {
            cg.alpha -= Time.deltaTime * 0.2f;
        }
    }

}

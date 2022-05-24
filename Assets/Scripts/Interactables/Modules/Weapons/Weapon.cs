using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SwordsInSpace
{
    public abstract class Weapon : Module
    {
        // Start is called before the first frame update
        [SerializeField]
        public WeaponSO data;

        public int currentAmmo;
        [SerializeField]
        public GameObject uiScreen;
        private bool uiShown = false;
        public Shooter[] shooters;

        private UIManager manager;

        void Awake()
        {

            shooters = GetComponentsInChildren<Shooter>();

            foreach (Shooter comp in shooters)
            {
                Debug.Log(comp.name);
                comp.data = data;
            }
        }

        void Start()
        {
            uiScreen = Instantiate(uiScreen, Vector3.zero, Quaternion.identity);
            uiShown = true;
            foreach (Button btn in uiScreen.GetComponentsInChildren<Button>())
            {
                btn.onClick.AddListener(delegate { ButtonClicked(btn); });
            }
            manager = UIManager.manager;
            uiScreen.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Interact(GameObject player)
        {
            manager.Offer(uiScreen);
        }

        private void ButtonClicked(Button btn)
        {
            string btnText = btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
            switch (btnText)
            {
                case "LEFT":
                    foreach (Shooter comp in shooters)
                    {

                        comp.Left();
                    }
                    break;
                case "RIGHT":
                    foreach (Shooter comp in shooters)
                    {
                        comp.Right();
                    }
                    break;
                case "FIRE":
                    foreach (Shooter comp in shooters)
                    {
                        comp.Fire();
                    }
                    break;
                case "A.FIRE":
                    foreach (Shooter comp in shooters)
                    {
                        comp.Fire();
                    }
                    break;
                case "EXIT":
                    manager.Close();
                    break;
            }
        }

        public abstract void Shoot();


    }
};
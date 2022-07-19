using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class WeaponDisplay : Display
    {
        [SerializeField] 
        UILoadingBar loadingBar;

        Weapon currentWeaponModule;
        public override void Setup(Interactable callingMod)
        {
            if (callingMod is not Weapon)
            {
                Debug.Log("Non Weapon module opening Weapon UI!");
                return;
            }
            currentWeaponModule = (Weapon)callingMod;

        }

        void Update()
        {
            if (currentWeaponModule.canFire)
            {
                loadingBar.gameObject.SetActive(false);
            } else
            {
                loadingBar.gameObject.SetActive(true);
                loadingBar.UpdateGraphic(currentWeaponModule.percentageReloaded);
            }
            
        }
    }
}


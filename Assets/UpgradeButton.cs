using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace SwordsInSpace
{

    public class UpgradeButton : MonoBehaviour
    {
        // Start is called before the first frame update

        [SerializeField]
        TextMeshProUGUI Description;

        [SerializeField]
        Image UpgradeImage;

        [SerializeField]
        TextMeshProUGUI Votes;


        public void UpdateButton(UpgradeSO data)
        {
            Description.text = data.upgradeName + "\n" + data.description;
            UpgradeImage.sprite = data.sprite;

        }

        public void AddVote(string name)
        {
            Votes.text += name + "\n";
        }

        public void ClearVote()
        {
            Votes.text = "";
        }

    }
};

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    [CreateAssetMenu(fileName = "UserData", menuName = "Assets/UserData")]
    public class UserData : ScriptableObject
    {
        public string username = "";

        public void setUsername(string name)
        {
            username = name;
        }
    }
}

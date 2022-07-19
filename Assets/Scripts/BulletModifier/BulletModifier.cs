using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace SwordsInSpace
{


    public abstract class BulletModifier
    {

        public UpgradeSO.UpgradeAttributes thisUpgradeAttribute;

        public abstract void Apply(List<AttackManager.BulletInfo> info);
    }
};
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordsInSpace
{
    public class AfterImageSpawner : NetworkBehaviour
    {
        // Start is called before the first frame update
        public SpriteRenderer rend;

        public GameObject afterimagePrefab;

        [ObserversRpc(IncludeOwner = true)]
        public void SpawnImage()
        {
            GameObject toSpawn = Instantiate(afterimagePrefab, gameObject.transform.position, gameObject.transform.rotation);
            toSpawn.GetComponent<SpriteRenderer>().sprite = rend.sprite;
            toSpawn.transform.localScale = gameObject.transform.localScale;
            toSpawn.GetComponent<FadeScript>().doFade();
        }
    }
};

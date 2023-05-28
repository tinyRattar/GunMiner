using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobFactory : MonoBehaviour
{
    [SerializeField] List<GameObject> pfbMobs;

    public GameObject GenerateMob(MobInfo mobInfo, Vector3 position, Quaternion rotation, Transform genParent)
    {
        GameObject goMob = GameObject.Instantiate(pfbMobs[mobInfo.PfbIndex], position, rotation, genParent);
        // tmp Code
        //Mob mob = goMob.GetComponent<Mob>();
        return goMob;
    }
}

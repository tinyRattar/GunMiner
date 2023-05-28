using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMobWeakpoint : MonoBehaviour, IClawHitable
{
    [SerializeField] WallMob mob;
    public bool OnClawHit()
    {
        mob.StartDead();
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mob == null)
            mob = this.GetComponentInParent<WallMob>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

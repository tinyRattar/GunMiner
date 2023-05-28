using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobWeakpoint : MonoBehaviour, IClawHitable
{
    [SerializeField] Mob mob;

    public bool OnClawHit()
    {
        mob.StartDead();
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mob == null)
            mob = this.GetComponentInParent<Mob>();
    }

}
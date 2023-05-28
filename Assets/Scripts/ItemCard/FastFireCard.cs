using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastFireCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        EffectManager.Instance.ActivateFastFire(lastTime);
    }
}

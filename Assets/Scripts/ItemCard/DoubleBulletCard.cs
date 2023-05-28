using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBulletCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        EffectManager.Instance.ActivateDoubleBullet(lastTime);
    }
}

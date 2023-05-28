using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastClawCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        EffectManager.Instance.ActivateFastClaw(lastTime);
    }
}

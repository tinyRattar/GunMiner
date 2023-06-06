using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetClawCard : ItemCard
{
    public override void OnStartUse()
    {
        EffectManager.Instance.ActivateMagnetClaw(lastTime);
        EffectManager.Instance.ActivateLootAbsorb(lastTime);
        base.OnStartUse();
    }
}

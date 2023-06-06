using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAbsorbCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        EffectManager.Instance.ActivateLootAbsorb(lastTime);
        EffectManager.Instance.ActivateMagnetClaw(lastTime);
    }
}

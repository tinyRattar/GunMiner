using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetClawCard : ItemCard
{
    public override void OnStartUse()
    {
        EffectManager.Instance.ActivateMagnetClaw(lastTime);
        base.OnStartUse();
    }
}

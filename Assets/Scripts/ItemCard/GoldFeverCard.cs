using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldFeverCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        LootManager.Instance.ActivateGoldFever(lastTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleStarCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        MinerManager.Instance.GetMiner().StartInvincibleStar(lastTime);
    }
}

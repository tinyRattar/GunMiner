using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        MinerManager.Instance.GetMiner().StartSpeedUp(lastTime);
    }
}

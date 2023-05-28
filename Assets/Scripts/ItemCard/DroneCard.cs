using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCard : ItemCard
{
    public override void OnStartUse()
    {
        base.OnStartUse();
        DroneManager.Instance.ActivateDrone(lastTime);
    }
}

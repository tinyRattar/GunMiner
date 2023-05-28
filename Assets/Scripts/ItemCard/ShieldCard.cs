using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCard : ItemCard
{
    [SerializeField] int shieldValue;

    public override void OnStartUse()
    {
        base.OnStartUse();
        MinerManager.Instance.GetMiner().ChangeShield(shieldValue);
    }
}

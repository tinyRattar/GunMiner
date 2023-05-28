using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairCard : ItemCard
{
    [SerializeField] int healValue;

    public override void OnStartUse()
    {
        base.OnStartUse();
        MinerManager.Instance.GetMinerEntity().OnHeal(healValue);
    }
}

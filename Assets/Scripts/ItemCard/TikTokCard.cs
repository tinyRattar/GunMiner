using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikTokCard : ItemCard
{
    [SerializeField] float value;
    public override void OnStartUse()
    {
        MainManager.Instance.ChangeTimerLevel(value);
    }
}

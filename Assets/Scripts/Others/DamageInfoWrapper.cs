using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    bullet,
    touch,
    explode,
    remove,
    melee
}

public class DamageInfo
{
    private float damageValue;
    private Entity src;
    private DamageType dType;
    private float luckBonus;
    private float toolDamageBonus; // use for shopMob

    private float slowTime;

    public DamageInfo(float damageValue, Entity src, DamageType dType = DamageType.bullet, float luckBonus = 0.0f)
    {
        this.damageValue = damageValue;
        this.src = src;
        this.dType = dType;
        this.luckBonus = luckBonus;
        toolDamageBonus = 1;
    }

    public DamageInfo(DamageInfo info)
    {
        this.damageValue = info.damageValue;
        this.src = info.src;
        this.DType = info.DType;
    }

    public float DamageValue { get => damageValue; set => damageValue = value; }
    public Entity Src { get => src; set => src = value; }
    public DamageType DType { get => dType; set => dType = value; }
    public float SlowTime { get => slowTime; set => slowTime = value; }
    public float LuckBonus { get => luckBonus; set => luckBonus = value; }
    public float ToolDamageBonus { get => toolDamageBonus; set => toolDamageBonus = value; }
}

public class DamageInfoWrapper : MonoBehaviour
{
    [SerializeField] DamageInfo info;
    [SerializeField] float damageValue;
    [SerializeField] Entity src;

    public DamageInfo GetDamageInfo()
    {
        return info;
    }
    private void Awake()
    {
        info = new DamageInfo(damageValue, src);
    }
}

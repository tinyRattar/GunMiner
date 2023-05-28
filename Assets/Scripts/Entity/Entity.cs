using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    player,
    ally,
    enemy
}

public class Entity : MonoBehaviour
{
    [SerializeField] EntityType entityType;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float curHealth;//tmp [SerializeField]
    protected bool canHit = true;
    protected bool waitDead = false; // todo: implement it

    [Header("»÷ÍË")]
    [SerializeField] protected bool canKnockback = false;
    [SerializeField] protected float knockbackScale_power = 1.0f;
    [SerializeField] protected float knockbackScale_time = 1.0f;
    [SerializeField] protected float knockbackMinY;
    protected bool inKnockback = false;
    protected float timerKnockback = 0.0f;
    protected float timeKnockback = 0.0f;
    protected Vector3 knockbackDirect;
    protected float knockbackPower;

    public EntityType GetEntityType()
    {
        return entityType;
    }
    public float GetCurHealth()
    {
        return curHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetCurHealthScale()
    {
        return (float)curHealth/(float)maxHealth;
    }
    public virtual bool OnHit(DamageInfo info)
    {
        if (canHit)
        {
            curHealth -= info.DamageValue;
            if (curHealth <= 0) StartDead();
            return true;
        }
        return false;
    }

    public virtual bool OnHeal(int value)
    {
        curHealth += value;
        if (curHealth > maxHealth) curHealth = maxHealth;

        return true;
    }

    public virtual bool StartKnockback(float kbPower, float kbTime, Vector3 direct)
    {
        if (canKnockback)
        {
            inKnockback = true;
            knockbackDirect = direct.normalized;
            if (knockbackDirect.y < knockbackMinY)
                knockbackDirect.y = knockbackMinY;
            knockbackDirect = direct.normalized;
            knockbackPower = kbPower * knockbackScale_power;
            timeKnockback = kbTime * knockbackScale_time;
            timerKnockback = timeKnockback;
            return true;
        }
        return false;
    }

    // should be called by FixedUpdate()
    protected virtual bool OnKnockback()
    {
        if (inKnockback)
        {
            timerKnockback -= Time.fixedDeltaTime;
            if (timerKnockback <= 0) { inKnockback = false; timerKnockback = 0.0f; }
            float _scale = timerKnockback / timeKnockback;
            this.transform.Translate(knockbackDirect * knockbackPower * _scale * Time.fixedDeltaTime, Space.World);
            return true;
        }
        return false;
    }

    public virtual bool OnClawHit()
    {
        return false;
    }

    public virtual void StartDead()
    {
        Destroy(this.gameObject);
    }

    protected virtual void Init()
    {
        curHealth = maxHealth;
    }

}

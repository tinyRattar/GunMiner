using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShopMobType
{
    confirm,
    cancel,
    exit,
    v50
}

public class ShopMob : Mob
{
    [SerializeField] bool inWork;
    [SerializeField] ShopMobType shopMobType;
    [SerializeField] float recoverDelay;
    float timerDelay;
    [SerializeField] float recoverRate;
    [SerializeField] Image uiHealthBar;

    public void SetShopMobType(ShopMobType type)
    {
        shopMobType = type;
    }

    public void SetInWork(bool flag = true)
    {
        inWork = flag;
        timerDelay = 0.0f;
    }

    public override bool OnHit(DamageInfo info)
    {
        if (timerHitFX > 0.0f) { timerDelay = recoverDelay; return true; }
        if(shopMobType == ShopMobType.exit && curHealth == maxHealth)
        {
            ShopManager.Instance.SayGoodBye();
        }
        if (!inWork) return false;
        timerDelay = recoverDelay;
        info.DamageValue = info.DamageValue * info.ToolDamageBonus;
        return base.OnHit(info);
    }


    public override void StartDead()
    {
        ShopManager.Instance.OnShopMobDead(shopMobType);
        SetInWork(false);
        //base.StartDead();
    }

    private void Recover()
    {
        if (timerDelay > 0.0f)
        {
            timerDelay -= Time.deltaTime;
        }
        else
        {
            curHealth += recoverRate * Time.deltaTime;

            if (curHealth > maxHealth) curHealth = maxHealth;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        BasicBehavior();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSR();
        Recover();
        uiHealthBar.fillAmount = curHealth / maxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopGoodType
{
    weapon,
    claw,
    other,
    upgrade
}

public class ShopGoodOre : Ore
{
    [Header("…Ã∆∑")] 
    [SerializeField] ShopGoodType goodType;
    [SerializeField] int goodIndex;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int price;
    [SerializeField] float timeJudge;
    [SerializeField] GameObject freeFX;
    [SerializeField] GameObject blueprintFx;
    float timerJudge;

    public ShopGoodType GetGoodType()
    {
        return goodType;
    }

    public void InitByShop(ShopGoodType goodType, int goodIndex, int price, Sprite sp)
    {
        this.goodType = goodType;
        this.goodIndex = goodIndex;
        sr.sprite = sp;
        this.price = price;
        //if (price == 0)
        //    freeFX.SetActive(true);
        //else
        //    freeFX.SetActive(false);
        if (price != 0)
        {
            blueprintFx.GetComponent<SpriteMask>().sprite = sp;
            blueprintFx.SetActive(true);
        }
        else
        {
            blueprintFx.SetActive(false);
        }
    }

    public void UpdatePrice(int price)
    {
        this.price = price;
        if (price != 0)
        {
            blueprintFx.SetActive(true);
        }
        else
        {
            blueprintFx.SetActive(false);
        }
    }

    /// <summary>
    /// mannual adjust
    /// </summary>
    /// <returns></returns>
    public int GetSheetIdx()
    {
        int ret = goodIndex;
        switch (goodType)
        {
            case ShopGoodType.weapon:
                return goodIndex;
            case ShopGoodType.claw:
                return goodIndex;
            case ShopGoodType.other:
                break;
            case ShopGoodType.upgrade:
                return goodIndex;
            default:
                break;
        }
        Debug.LogError("no sheet idx");
        return -1;
    }

    public int GetPrice()
    {
        return price;
    }

    public void OnConfirm()
    {
        cannotDrag = false;
        claw.TryRefreshDragingOre(this, OreOnHitBehavior.none);
        blueprintFx.SetActive(false);
    }

    public void OnCancel()
    {
        claw.TryRefreshDragingOre(this, OreOnHitBehavior.destory);
        ShopManager.Instance.UnlockExit();
    }

    public override void OnGotcha()
    {
        ShopManager.Instance.UnlockExit();
        ShopManager.Instance.OnGoodGotcha(this.transform.parent, goodType, goodIndex);
        switch (goodType)
        {
            case ShopGoodType.weapon:
                WeaponManager.Instance.ChangeCurrentWeapon(goodIndex);
                break;
            case ShopGoodType.claw:
                ClawManager.Instance.ChangeCurrentClaw(goodIndex);
                break;
            case ShopGoodType.other:
                break;
            case ShopGoodType.upgrade:
                if (goodIndex == 0)
                    MinerManager.Instance.UpgradeGunner();
                else
                    MinerManager.Instance.UpgradeMiner();
                break;
            default:
                break;
        }
        base.OnGotcha();
    }

    public override void StartOnDrag(Claw srcClaw)
    {
        ShopManager.Instance.LockExit();
        if (price == 0)
        {
            cannotDrag = false;
        }
        else
        {
            ShopManager.Instance.BindCurGood(this);
        }
        //if (MinerManager.Instance.GetCurMoney() >= price)
        //{
        //    cannotDrag = false;
        //}
        //else
        //{
        //    cannotDrag = true;
        //    //timerJudge = timeJudge;
        //}
        base.StartOnDrag(srcClaw);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (timerJudge > 0.0f)
        {
            timerJudge -= Time.deltaTime;
            if (timerJudge < 0.0f)
            {
                claw.TryRefreshDragingOre(this, OreOnHitBehavior.destory);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinerManager : Singleton<MinerManager>
{
    public const int MAX_LEVEL_MINER = 3;
    public const int MAX_LEVEL_GUNNER = 3;
    [SerializeField] Miner miner;
    [SerializeField] int money;
    [SerializeField] int score;
    [SerializeField] int levelMiner;
    [SerializeField] int levelGunner;
    // tmpCode
    [SerializeField] Text txtHP;
    [SerializeField] Text txtGold;

    [SerializeField] FX_UICounter moneyUI;

    public int GetLevelMiner()
    {
        return levelMiner;
    }
    public void UpgradeMiner()
    {
        levelMiner++;
        if (levelMiner > MAX_LEVEL_MINER) { Debug.LogError("too much level"); levelMiner = MAX_LEVEL_MINER; }
        ClawManager.Instance.UpdateClawInfo();
    }
    public int GetUpgradePriceMiner()
    {
        return 0;
    }
    public int GetLevelGunner()
    {
        return levelGunner;
    }
    public void UpgradeGunner()
    {
        levelGunner++;
        if (levelGunner > MAX_LEVEL_GUNNER) { Debug.LogError("too much level"); levelGunner = MAX_LEVEL_GUNNER; }
        WeaponManager.Instance.UpdateWeaponInfo();
    }
    public int GetUpgradePriceGunner()
    {
        return 0;
    }

    public int GetCurMoney()
    {
        return money;
    }
    public void ChangeMoney(int value)
    {
        money += value;
        if (money > 9999) money = 9999;
        if (value > 0)
            AchievementManager.Instance.ChangeValue(RecordType.earnMoney, value);
        else
            AchievementManager.Instance.ChangeValue(RecordType.spendMoney, -value);
    }

    public void ChangeScore(int value)
    {
        score += value;
    }

    public Entity GetMinerEntity()
    {
        return miner;
    }

    public Miner GetMiner()
    {
        return miner;
    }


    public float GetCurSpeed()
    {
        return ((Miner)miner).GetCurSpeed();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // tmpCode
        if (txtHP)
            txtHP.text = "HP: " + miner.GetCurHealth().ToString() + "[" + miner.GetCurShield().ToString() + "]";
        if (txtGold)
            txtGold.text = "Gold: " + money.ToString();

        if (moneyUI)
            moneyUI.SetValue(money);
    }
}

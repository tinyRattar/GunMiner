using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeState
{
    enterTalk,
    inMiniGame,
    finishTalk,
    waitExit
}

public class ShopManager : Singleton<ShopManager>
{
    //bool inWork = false;
    bool inShop = false; // is true when no anim
    [SerializeField] GameObject pfbGood;
    //[SerializeField] GameObject pfbSelectMob;
    //[SerializeField] Transform anchorConfirm;
    //[SerializeField] Transform anchorCancel;
    //[SerializeField] Transform anchorExit;
    [SerializeField] List<Transform> listAnchorsGoodWeapon;
    [SerializeField] List<Transform> listAnchorsGoodClaw;
    [SerializeField] List<Sprite> listWeaponSprites;
    [SerializeField] List<Sprite> listClawSprites;

    [Header("¹ºÂò")]
    [SerializeField] int originPrice;
    [SerializeField] int increasePricePerPurch;
    int purchaseTime = 0;
    [SerializeField] Text txtPrice;
    [SerializeField] GameObject goPriceDialog;
    [SerializeField] FX_UICounter UI_price;
    [SerializeField] GameObject vfx_baked;

    [Header("Éý¼¶")]
    [SerializeField] bool isUpgrade;
    bool waitDialogFinish = false;
    [SerializeField] List<Transform> listAnchorsUpgrade;
    [SerializeField] List<Sprite> listUpgradeSprites;
    // state: talk->[space]->select->[space]->showCG->[miniGame]->showCG->talk->[space]->exit
    bool inUpgradePhase;
    [SerializeField] UpgradeState upgradeState;
    [SerializeField] UpgradeMiniGame miniGame;

    [SerializeField] bool inSecondChance;
    [SerializeField] int originPrice_SecondChance;
    [SerializeField] int increasePrice_SecondChance;
    int secondChanceTime = 0;
    int lastUpgraded;
    

    //GameObject goExit;
    //GameObject goConfirm;
    //GameObject goCancel;
    [SerializeField] ShopMob smExit;
    [SerializeField] ShopMob smConfirm;
    [SerializeField] ShopMob smCancel;


    [SerializeField] Animator animator;
    ShopGoodOre curGood;

    [Header("¿¾Ïä¶¯»­")]
    [SerializeField] bool inBakeAnim;
    [SerializeField] float timeBakeOpen;
    [SerializeField] float timeBakeAudio;
    [SerializeField] float timeBakeFill;
    float timerBakeAnim;
    [SerializeField] GameObject pfbBakeCoin;
    [SerializeField] float timeBakeCoinGenInterval;
    [SerializeField] int bakeCoinGenNum;
    [SerializeField] float minBakeFlyMidRange;
    [SerializeField] float maxBakeFlyMidRange;
    [SerializeField] Transform anchorBakeSrc;
    [SerializeField] Transform anchorBakeTar;
    [SerializeField] float bakeTarOffsetX;
    [SerializeField] float bakeTarOffsetY;
    [SerializeField] AudioClip audioBake;
    bool flagAudioBake;
    float timerBakeCoinGen;

    [Header("¶Ô»°¿ò")]
    [SerializeField] NPC npc;

    public void SetIsUpgrade(bool flag)
    {
        isUpgrade = flag;
    }

    public void LockExit()
    {
        smExit.SetInWork(false);
    }
    public void UnlockExit()
    {
        smExit.SetInWork(true);
    }

    public void BindCurGood(ShopGoodOre shopGoodOre)
    {
        //smExit.SetInWork(false);
        curGood = shopGoodOre;
        //goConfirm = GameObject.Instantiate(pfbSelectMob, anchorConfirm.transform.position, Quaternion.identity, anchorConfirm.transform);
        //goConfirm.GetComponent<ShopMob>().SetShopMobType(ShopMobType.confirm);
        //goCancel = GameObject.Instantiate(pfbSelectMob, anchorCancel.transform.position, Quaternion.identity, anchorCancel.transform);
        //goCancel.GetComponent<ShopMob>().SetShopMobType(ShopMobType.cancel);
        smConfirm.SetInWork();
        smCancel.SetInWork();
        int sheetIdx = -1;
        switch (shopGoodOre.GetGoodType())
        {
            case ShopGoodType.weapon:
                sheetIdx = curGood.GetSheetIdx();
                npc.ShowText(2, sheetIdx, 0);
                break;
            case ShopGoodType.claw:
                sheetIdx = curGood.GetSheetIdx() + 4;
                npc.ShowText(2, sheetIdx, 0);
                break;
            case ShopGoodType.other:
                break;
            case ShopGoodType.upgrade:
                int upgradeIdx = curGood.GetSheetIdx();
                if(upgradeIdx == 0) // gunner
                {
                    sheetIdx = WeaponManager.Instance.GetCurrentWeaponIdx();
                    npc.ShowText(3, sheetIdx, MinerManager.Instance.GetLevelGunner()+2);
                }
                else
                {
                    sheetIdx = ClawManager.Instance.GetCurrentClawIdx() + 4;
                    npc.ShowText(3, sheetIdx, MinerManager.Instance.GetLevelMiner()+2);
                }
                break;
            default:
                break;
        }
        goPriceDialog.SetActive(true);
        txtPrice.text = GetCurPrice().ToString();
        UI_price.SetValue(GetCurPrice());
    }

    public void OnGoodGotcha(Transform transform, ShopGoodType sType, int idx)
    {
        switch (sType)
        {
            case ShopGoodType.weapon:
                GenerateShopGoodOre(transform, sType, WeaponManager.Instance.GetCurrentWeaponIdx());
                WeaponManager.Instance.SetPrice(idx, 0);
                break;
            case ShopGoodType.claw:
                GenerateShopGoodOre(transform, sType, ClawManager.Instance.GetCurrentClawIdx());
                ClawManager.Instance.SetPrice(idx, 0);
                break;
            case ShopGoodType.other:
                break;
            case ShopGoodType.upgrade:
                PutOffGoods();
                break;
            default:
                break;
        }
    }

    public void ReleaseCurGood()
    {
        //if (goConfirm) Destroy(goConfirm);
        //if (goCancel) Destroy(goCancel);
        smConfirm.SetInWork(false);
        smCancel.SetInWork(false);
        //smExit.SetInWork(true);
        curGood = null;
        goPriceDialog.SetActive(false);
    }

    public void StartOpenShop()
    {
        this.transform.position = MinerManager.Instance.GetMiner().transform.position;
        InputManager.Instance.SetFreezeInput(true);
        animator.Play("enter");
    }

    /// <summary>
    /// called by animation "enter" when screen is black
    /// </summary>
    public void OnBlackInOpenShop()
    {
        if (isUpgrade)
        {
            inUpgradePhase = true;
            inSecondChance = false;
            upgradeState = UpgradeState.enterTalk;
            InputManager.Instance.SetFreezeInput(true);
            waitDialogFinish = false;
            // tmp code
            //PutOnGoods();
        }
        else
        {
            PutOnGoods();
        }
        OreManager.Instance.ForceRemoveAll();
        LootManager.Instance.ForceRemoveAll();
        MinerManager.Instance.GetMiner().OnHeal(999);
    }

    /// <summary>
    /// called by animation "enter"
    /// </summary>
    public void FinishOpenShop()
    {
        if (!isUpgrade)
            InputManager.Instance.SetFreezeInput(false);
        //goExit = GameObject.Instantiate(pfbSelectMob, anchorExit.transform.position, Quaternion.identity, anchorExit.transform);
        //goExit.GetComponent<ShopMob>().SetShopMobType(ShopMobType.exit);
        //smExit.SetInWork(true);
        UnlockExit();
        //npc.ShowText(MainManager.Instance.GetCurLevel());
        npc.ShowText(1, -1, MainManager.Instance.GetCurLevel() + 1);
        inShop = true;
    }


    public void StartCloseShop()
    {
        InputManager.Instance.SetFreezeInput(true);
        animator.Play("exit");
        npc.CloseDialog();
        inShop = false;
    }

    /// <summary>
    /// called by animation "exit" when screen is black
    /// </summary>
    public void OnBlackInCloseShop()
    {
        MinerManager.Instance.GetMiner().SetFreezeScreen(false);
        if (curGood)
        {
            curGood.OnCancel();
        }
        ReleaseCurGood();
        //if (goExit) Destroy(goExit);
        if (smExit) LockExit();
        PutOffGoods();
        //npc.CloseDialog();
        MainManager.Instance.FinishShopState();
    }

    /// <summary>
    /// called by animation "exit"
    /// </summary>
    public void FinishCloseShop()
    {
        InputManager.Instance.SetFreezeInput(false);
    }

    public void OnShopMobDead(ShopMobType sType)
    {
        switch (sType)
        {
            case ShopMobType.confirm:
                if (curGood)
                {
                    if (MinerManager.Instance.GetCurMoney() >= curGood.GetPrice())
                    {
                        MinerManager.Instance.ChangeMoney(-curGood.GetPrice());
                        purchaseTime++;
                        UpdateGoods();
                        //curGood.OnConfirm();
                        StartBake();
                        smConfirm.SetInWork(false);
                        smCancel.SetInWork(false);
                    }
                    else
                    {
                        curGood.OnCancel();
                        ReleaseCurGood();
                        npc.ShowText(4, 0, 0);
                    }
                }
                else
                    ReleaseCurGood();
                break;
            case ShopMobType.cancel:
                if (curGood)
                    curGood.OnCancel();
                ReleaseCurGood();
                break;
            case ShopMobType.exit:
                //if (curGood)
                //    curGood.OnCancel();
                //ReleaseCurGood();
                StartCloseShop();
                break;
            case ShopMobType.v50:
                MinerManager.Instance.ChangeMoney(50);
                break;
            default:
                break;
        }
    }

    public void SayGoodBye()
    {
        if (inShop)
        {
            if (curGood)
                npc.ShowText(5, 1, 0);
            else
                npc.ShowText(5, 0, 0);
        }
    }
    public void PleaseSay(int triggerIdx, int conditionIdx1, int conditionIdx2)
    {
        if (inShop)
            npc.ShowText(triggerIdx, conditionIdx1, conditionIdx2);
    }
    public bool IsSaying()
    {
        return !npc.IsDialogTotalHide();
    }
    private int GetCurPrice()
    {
        return originPrice + purchaseTime * increasePricePerPurch;
    }

    private void GenerateShopGoodOre(Transform anchor, ShopGoodType sType, int idx)
    {
        GameObject go = GameObject.Instantiate(pfbGood, anchor.position, Quaternion.identity, anchor);
        ShopGoodOre shopGoodOre = go.GetComponent<ShopGoodOre>();
        Sprite sp = listWeaponSprites[0];
        int price = 0;
        switch (sType)
        {
            case ShopGoodType.weapon:
                sp = listWeaponSprites[idx];
                price = WeaponManager.Instance.GetPrice(idx);
                if (price != 0)
                    price = GetCurPrice();
                break;
            case ShopGoodType.claw:
                sp = listClawSprites[idx];
                price = ClawManager.Instance.GetPrice(idx);
                if (price != 0)
                    price = GetCurPrice();
                break;
            case ShopGoodType.other:
                Debug.LogError("Invalid");
                break;
            case ShopGoodType.upgrade:
                sp = listUpgradeSprites[idx];
                if (idx == 0)
                    price = MinerManager.Instance.GetUpgradePriceGunner();
                else
                    price = MinerManager.Instance.GetUpgradePriceMiner();
                break;
            default:
                break;
        }
        shopGoodOre.InitByShop(sType, idx, price, sp);
    }

    private void UpdateGoods()
    {
        foreach (ShopGoodOre item in this.GetComponentsInChildren<ShopGoodOre>())
        {
            if (!item.IsOnDrag())
            {
                if (item.GetPrice() != 0)
                    item.UpdatePrice(GetCurPrice());
            }
        }
        txtPrice.text = GetCurPrice().ToString();
        UI_price.SetValue(GetCurPrice());
    }

    private void PutOnGoods()
    {
        if (isUpgrade)
        {
            for (int i = 0; i < listAnchorsUpgrade.Count; i++)
            {
                GenerateShopGoodOre(listAnchorsUpgrade[i].transform, ShopGoodType.upgrade, i);
            }
        }
        else
        {
            int curWeaponIdx = WeaponManager.Instance.GetCurrentWeaponIdx();
            for (int i = 0; i < listAnchorsGoodWeapon.Count; i++)
            {
                int idx = i + 1;
                if (idx == curWeaponIdx)
                    idx = 0;
                GenerateShopGoodOre(listAnchorsGoodWeapon[i].transform, ShopGoodType.weapon, idx);
            }
            int curClawIdx = ClawManager.Instance.GetCurrentClawIdx();
            for (int i = 0; i < listAnchorsGoodClaw.Count; i++)
            {
                int idx = i + 1;
                if (idx == curClawIdx)
                    idx = 0;
                GenerateShopGoodOre(listAnchorsGoodClaw[i].transform, ShopGoodType.claw, idx);
            }
        }
        UI_price.SetValue(GetCurPrice());
    }

    private void PutOffGoods()
    {
        foreach (ShopGoodOre item in this.GetComponentsInChildren<ShopGoodOre>())
        {
            if (!item.IsOnDrag())
            {
                Destroy(item.gameObject);
            }
        }
    }

    private void StartBake()
    {
        timerBakeAnim = 0.0f;
        inBakeAnim = true;
        animator.Play("openGate");
        flagAudioBake = true;
    }
    private void FinishBake()
    {
        curGood.OnConfirm();
        if (vfx_baked) GameObject.Instantiate(vfx_baked, curGood.transform.position, Quaternion.identity);
        inBakeAnim = false;
        animator.Play("closeGate");
        ReleaseCurGood();
    }
    private void UpdateBakeAnim()
    {
        timerBakeAnim += Time.deltaTime;
        if (timerBakeAnim < timeBakeOpen)
        {
            // do nothing
        }
        else if (timerBakeAnim < timeBakeOpen + timeBakeFill)
        {
            
            BakeFill();
        }
        else
        {
            FinishBake();
        }

        if (flagAudioBake && timerBakeAnim > timeBakeAudio) {
            if (audioBake) SEManager.Instance.PlaySE(audioBake);
            flagAudioBake = false;
        }
    }

    private void BakeFill()
    {
        timerBakeCoinGen -= Time.deltaTime;
        if (timerBakeCoinGen <= 0.0f)
        {
            timerBakeCoinGen = timeBakeCoinGenInterval;
            Vector3 genPos = anchorBakeSrc.position;
            for (int i = 0; i < bakeCoinGenNum; i++)
            {
                GameObject go = GameObject.Instantiate(pfbBakeCoin, genPos, Quaternion.identity);
                float midLength = UnityEngine.Random.Range(minBakeFlyMidRange, maxBakeFlyMidRange);
                float angle = UnityEngine.Random.Range(0, 360.0f);
                Vector3 midPos = genPos + Quaternion.Euler(0, 0, angle) * new Vector3(midLength, 0, 0);
                float offX = UnityEngine.Random.Range(-bakeTarOffsetX, bakeTarOffsetX);
                float offY = UnityEngine.Random.Range(-bakeTarOffsetY, bakeTarOffsetY);
                Vector3 tarPos = anchorBakeTar.position + new Vector3(offX, offY, 0);
                go.GetComponent<FX_ShopCoin>().Init(genPos, midPos, tarPos);
            }
        }
    }

    public void OnMiniGameFinish(int retIdx) //0:gunner, 1:miner 2:giveup
    {
        if (retIdx==0)
        {
            MinerManager.Instance.UpgradeGunner();
            npc.ShowText(7, 0, 0);
            lastUpgraded = 0;
            if (inSecondChance)
                secondChanceTime++;
        }
        else if(retIdx==1)
        {
            MinerManager.Instance.UpgradeMiner();
            npc.ShowText(7, 1, 0);
            lastUpgraded = 1;
            if (inSecondChance)
                secondChanceTime++;
        }
        else
        {
            npc.ShowText(7, 2, 0);
        }

        upgradeState = UpgradeState.finishTalk;
    }


    // upgrade
    private void DuringUpgrade()
    {
        switch (upgradeState)
        {
            case UpgradeState.enterTalk:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitDialogFinish = true;
                }
                if (waitDialogFinish && npc.IsDialogTotalHide())
                {
                    waitDialogFinish = false;
                    upgradeState = UpgradeState.inMiniGame;
                    if (inSecondChance)
                    {
                        miniGame.StartMiniGame_SecondChance(lastUpgraded, originPrice_SecondChance + increasePrice_SecondChance * secondChanceTime);
                    }
                    else
                    {
                        miniGame.StartMiniGame();
                    }
                }
                break;
            case UpgradeState.inMiniGame:
                break;
            case UpgradeState.finishTalk:
                upgradeState = UpgradeState.waitExit;
                break;
            case UpgradeState.waitExit:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!inSecondChance)
                    {
                        inSecondChance = true;
                        upgradeState = UpgradeState.enterTalk;
                        waitDialogFinish = false;
                        npc.ShowText(8, 0, 0);
                    }
                    else
                    {
                        waitDialogFinish = true;
                    }
                }
                if (waitDialogFinish && npc.IsDialogTotalHide())
                {
                    InputManager.Instance.SetFreezeInput(false);
                    StartCloseShop();
                    inUpgradePhase = false;
                    waitDialogFinish = false;
                }
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inShop)
        {
            if (inBakeAnim)
            {
                UpdateBakeAnim();
            }
            if (inUpgradePhase)
            {
                DuringUpgrade();
            }
        }
    }
}

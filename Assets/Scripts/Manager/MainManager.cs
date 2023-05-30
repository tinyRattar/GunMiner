using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using System;

public enum LevelState
{
    battle,
    shop,
    title
}
public class LevelInfo
{
    int minMobGenNum;
    int maxMobGenNum;
    float timeMaxMobGen;
    float timeLevel;

    public LevelInfo(int minMobGenNum, int maxMobGenNum, float timeMaxMobGen, float timeLevel)
    {
        this.minMobGenNum = minMobGenNum;
        this.maxMobGenNum = maxMobGenNum;
        this.timeMaxMobGen = timeMaxMobGen;
        this.timeLevel = timeLevel;
    }

    public int MinMobGenNum { get => minMobGenNum; set => minMobGenNum = value; }
    public int MaxMobGenNum { get => maxMobGenNum; set => maxMobGenNum = value; }
    public float TimeMaxMobGen { get => timeMaxMobGen; set => timeMaxMobGen = value; }
    public float TimeLevel { get => timeLevel; set => timeLevel = value; }
}

public class MainManager : Singleton<MainManager>
{
    public const int WEAPON_NUM = 4;
    [SerializeField] GameObject parent_bullets;
    [SerializeField] GameObject parent_mobs;
    [SerializeField] GameObject parent_ores;
    [SerializeField] GameObject parent_loots;

    [Header("关卡")]
    [SerializeField] float timeLevel;
    [SerializeField] float timeAddPerLevel;
    [SerializeField] float timeLevelMax;
    float timerLevel;
    [SerializeField] string csv_name;
    List<LevelInfo> levelInfos = new List<LevelInfo>();

    static int maxLevel = 10;
    [SerializeField] int curLevel = 0;

    

    [SerializeField] LevelState curLevelState;

    [Header("商店")]
    [SerializeField] GameObject pfbShop;
    [SerializeField] int upgradeShopPerLevel;
    GameObject shop;
    [Header("设置")]
    [SerializeField] bool callSettingByESC;
    [SerializeField] SettingMenu settingMenu;
    [SerializeField] GameObject pfbSettingMob;
    [SerializeField] Transform settingMobParent;
    [SerializeField] AudioClip audioCallSetting;

    [Header("背景音乐")]
    [SerializeField] AudioSource asBGM;
    [SerializeField] AudioClip audioBGMShop;
    [SerializeField] AudioClip audioBGMBattle;

    [Header("CG结算")]
    [SerializeField] CGBehavior cgBehavior;
    bool isGameOver = false;
    bool isGameWin = false;
    bool spaceRestart = false;
    AsyncOperation asyncOperation;

    [Header("UI")]
    [SerializeField] FX_UITimer UITimer;
    [SerializeField] Text txtLevelRemain;

    [Header("教程")]
    [SerializeField] GameObject pfbPageHelp;
    [SerializeField] float timePageShow;
    [SerializeField] Transform pageGenAnchor;

    [Header("其他")]
    [SerializeField] CameraShock cameraShock;
    [SerializeField] SpriteRenderer srProspect;
    [SerializeField] SpriteRenderer srInputHint;
    [SerializeField] List<Sprite> listProspectSprites;

    [Header("调试")]
    [SerializeField] bool startShop;
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtLevelTime;
    [SerializeField] bool startLevel;
    [SerializeField] bool cheatMode;


    public static int MaxLevel { get => maxLevel; }
    public int GetCurLevel()
    {
        return curLevel;
    }
    public void ChangeTimerLevel(float value)
    {
        timerLevel += value;
    }

    public static GameObject GetParentBullets() { return Instance.parent_bullets; }
    public static GameObject GetParentMobs() { return Instance.parent_mobs; }
    public static GameObject GetParentOres() { return Instance.parent_ores; }
    public static GameObject GetParentLoots() { return Instance.parent_loots; }

    public bool InCheatMode()
    {
        return cheatMode;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadEndCG()
    {
        if (asyncOperation != null)
            asyncOperation.allowSceneActivation = true;
        else
            SceneManager.LoadScene(3);
    }

    public void OpenSettingMenu()
    {
        SetGamePause(true);
        settingMenu.SetEnable(true);
        if (audioCallSetting) SEManager.Instance.PlaySE(audioCallSetting);
    }

    public void CloseSettingMenu()
    {
        settingMenu.SetEnable(false);
        if (settingMobParent)
            GameObject.Instantiate(pfbSettingMob, settingMobParent.position, Quaternion.identity, settingMobParent);
        SetGamePause(false);
        if (audioCallSetting) SEManager.Instance.PlaySE(audioCallSetting);
    }
    public void SetGamePause(bool flag)
    {
        if (flag)
        {
            Time.timeScale = 0.0f;
            InputManager.Instance.SetFreezeInput(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            InputManager.Instance.SetFreezeInput(false);
        }
    }
    public void StartShopState(bool forceUpgrade = false)
    {
        if (curLevel == maxLevel - 1)
        {
            StartGameOver_Win();
            return;
        }

        if (forceUpgrade)
            ShopManager.Instance.SetIsUpgrade(true);
        else
            ShopManager.Instance.SetIsUpgrade((curLevel + 1) % upgradeShopPerLevel == 0);
        startShop = true;
        timerLevel = 0.0f;
        curLevelState = LevelState.shop;
        MobManager.Instance.ForceRemoveAll();
        //OreManager.Instance.ForceRemoveAll();
        //LootManager.Instance.ForceRemoveAll();
        //shop = GameObject.Instantiate(pfbShop, MinerManager.Instance.GetMiner().transform.position, Quaternion.identity, this.transform);
        ShopManager.Instance.StartOpenShop();
        MinerManager.Instance.GetMiner().SetFreezeScreen(true);
        WeaponManager.Instance.GetCurrentWeapon().ResetDegree();
        asBGM.clip = audioBGMShop;
        asBGM.Play(); asBGM.loop = true;
        cameraShock.SetInShock(true);
        txtLevelRemain.text = (10 - curLevel - 1).ToString();
    }

    public void FinishShopState()
    {
        //tmp code
        //if (shop)
        //    Destroy(shop);
        startShop = false;

        curLevelState = LevelState.battle;
        curLevel++;
        if (curLevel >= maxLevel) curLevel = 0;
        if (txtLevel)
            txtLevel.text = "[F1]Level:" + (curLevel + 1).ToString();
        StartLevel();
        //MinerManager.Instance.GetMiner().SetFreezeScreen(false);
        asBGM.clip = audioBGMBattle;
        asBGM.Play(); asBGM.loop = true;
    }

    public void StartGameOver()
    {
        UITimer.StopAudioWarning();
        isGameOver = true;
        timerLevel = 0.0f;
        MobManager.Instance.ForceRemoveAll();
        OreManager.Instance.ForceRemoveAll();
        LootManager.Instance.ForceRemoveAll();
        MinerManager.Instance.GetMiner().SetFreezeScreen(true);
        int cgIdx = UnityEngine.Random.Range(0, 2);
        cgBehavior.StartCG(cgIdx);
        WeaponManager.Instance.GetCurrentWeapon().SetInWork(false);
        InputManager.Instance.SetFreezeInput(true);
    }

    public void StartGameOver_Win()
    {
        UITimer.StopAudioWarning();
        AchievementManager.Instance.ChangeValue(RecordType.winNum, 1);
        isGameOver = true;
        isGameWin = true;
        timerLevel = 0.0f;
        MobManager.Instance.ForceRemoveAll();
        OreManager.Instance.ForceRemoveAll();
        LootManager.Instance.ForceRemoveAll();
        MinerManager.Instance.GetMiner().SetFreezeScreen(true);
        int cgIdx = 2;
        cgBehavior.StartCG(cgIdx);
        WeaponManager.Instance.GetCurrentWeapon().SetInWork(false);
        InputManager.Instance.SetFreezeInput(true);
        asyncOperation = SceneManager.LoadSceneAsync(3);
        asyncOperation.allowSceneActivation = false;
    }

    public void SetSpaceRestart()
    {
        spaceRestart = true;
    }

    public void StartLevel()
    {
        //timerLevel = timeLevel + timeAddPerLevel * curLevel;
        //if (timerLevel > timeLevelMax) { timerLevel = timeLevelMax; }
        LoadLevelInfo();
        LevelInfo info = levelInfos[curLevel];
        MobManager.Instance.StartWork(info.MinMobGenNum, info.MaxMobGenNum, info.TimeMaxMobGen);
        OreManager.Instance.StartGenerate();
    }

    private void LoadLevelInfo()
    {
        LevelInfo info = levelInfos[curLevel];
        timerLevel = info.TimeLevel;
        if (curLevel != 0)
        {
            srInputHint.color = new Color(1, 1, 1, 0);
        }
        if (curLevel < 4)
        {
            srProspect.sprite = listProspectSprites[0];
            MinerManager.Instance.GetMiner().SetMaxFloat(0.25f);
        }
        else if (curLevel < 8)
        {
            srProspect.sprite = listProspectSprites[1];
            MinerManager.Instance.GetMiner().SetMaxFloat(0.5f);
        }
        else
        {
            srProspect.sprite = listProspectSprites[2];
            MinerManager.Instance.GetMiner().SetMaxFloat(0.8f);
        }
        srProspect.gameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void Init()
    {
        if (csv_name != "")
        {
            DataTable dataTable;
            try
            {
                dataTable = CSV_util.Read("./" + csv_name + ".csv");
            }
            catch (Exception)
            {
                Debug.LogError("load resources " + csv_name);
                dataTable = CSV_util.LoadFromResources(csv_name);
            }
            for (int i = 1; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][0].ToString()[0] == '/')
                    break;
                string _level = dataTable.Rows[i][0].ToString();
                int _minMobGenNum = int.Parse(dataTable.Rows[i][1].ToString());
                int _maxMobGenNum = int.Parse(dataTable.Rows[i][2].ToString());
                float _timeMaxMobGen = float.Parse(dataTable.Rows[i][3].ToString());
                float _timeLevel = float.Parse(dataTable.Rows[i][4].ToString());
                levelInfos.Add(new LevelInfo(_minMobGenNum, _maxMobGenNum, _timeMaxMobGen, _timeLevel));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Start()
    {
        if (txtLevel)
            txtLevel.text = "[F1]Level:" + (curLevel + 1).ToString();
        SetGamePause(false);
        //if(curLevelState == LevelState.battle)
        //    StartLevel();
    }
    private void Update()
    {
        if (pfbPageHelp)
        {
            if (timePageShow >= 0)
            {
                timePageShow -= Time.deltaTime;
                if (timePageShow < 0.0f)
                {
                    GameObject.Instantiate(pfbPageHelp, pageGenAnchor.position, Quaternion.identity, pageGenAnchor);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.LeftShift))
        {
            cheatMode = !cheatMode;
        }
        if (cheatMode && curLevelState == LevelState.battle && Input.GetKeyDown(KeyCode.F12))
        {
            MinerManager.Instance.GetMiner().StartDead();
        }
        if (isGameOver)
        {
            if (spaceRestart && (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Escape)))
            {
                if (isGameWin)
                    LoadEndCG();
                else
                    Restart();
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingMenu.GetEnable())
                CloseSettingMenu();
            else if (callSettingByESC)
                OpenSettingMenu();
        }

        if (curLevelState == LevelState.title)
        {
            return;
        }
        if (cheatMode && Input.GetKeyDown(KeyCode.F10))
        {
            if (startShop)
            {
                startShop = false;
                ShopManager.Instance.StartCloseShop();
            }
            else
            {
                StartShopState();
                startShop = true;
            }
        }
        if (cheatMode && Input.GetKeyDown(KeyCode.F8))
        {
            if (startShop)
            {
                startShop = false;
                ShopManager.Instance.StartCloseShop();
            }
            else
            {
                StartShopState(true);
                startShop = true;
            }
        }



        if (cheatMode && Input.GetKeyDown(KeyCode.F9))
        {
            MinerManager.Instance.ChangeMoney(1000);
        }
        if (cheatMode && Input.GetKeyDown(KeyCode.F7))
        {
            MinerManager.Instance.UpgradeGunner();
            MinerManager.Instance.UpgradeMiner();
        }
        //if (startShop)
        //{
        //    startShop = false;
        //    StartShopState();
        //}
        if (startLevel)
        {
            startLevel = false;
            StartLevel();
        }

        if (cheatMode && Input.GetKeyDown(KeyCode.F1))
        {
            curLevel++;
            if (curLevel >= maxLevel) curLevel = 0;
            if (txtLevel)
                txtLevel.text = "[F1]Level:" + (curLevel + 1).ToString();
        }

        if (curLevelState == LevelState.battle)
        {
            timerLevel -= Time.deltaTime;
            if (timerLevel < 0)
            {
                timerLevel = 0.0f;
                AchievementManager.Instance.ChangeValue(RecordType.levelClear, 1);
                if (curLevel == maxLevel - 1)
                {
                    StartGameOver_Win();
                    return;
                }
                StartShopState();
            }
            if (UITimer)
                UITimer.SetValue((int)(timerLevel / 60.0), (int)timerLevel % 60);
        }
        if (txtLevelTime)
            txtLevelTime.text = ((int)(timerLevel / 60.0)).ToString("D2") + ":" + ((int)timerLevel % 60).ToString("D2");
        
    }
}
